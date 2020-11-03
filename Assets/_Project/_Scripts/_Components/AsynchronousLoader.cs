using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EnhancedBehaviours;


public class AsynchronousLoader : EnhancedBehaviour
{
    public static AsynchronousLoader instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
            if (instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

        instance = this;
    }

    public enum OperationType
    {
        Load,
        Unload
    }

    public static int amountOfOperationsEnqueued { get; private set; }
    public static int amountOfOperationsRunning { get; private set; }
    public static bool idle { get { return amountOfOperationsEnqueued < 1 && amountOfOperationsRunning < 1; } }
    public static ThreadPriority threadPriorityWhileIdle = ThreadPriority.Normal;
    public static ThreadPriority threadPriorityWhileRunning = ThreadPriority.Low;
    private static bool previouslyIdle;


    private const float maximumDeltatime = 1/30f;
    private const int maximumAmountOfOperationsRunning = 16;
    private const int priorityThreshold = 8;


    private struct Operation
    {
        public OperationType operationType;
        public string sceneName;
        public Action<AsyncOperation> operationCallback;
        public AsyncOperation asyncOperation;
    }

    private List<Operation> enqueuedOperations = new List<Operation>(900);
    private Operation[] runningOperations = new Operation[maximumAmountOfOperationsRunning];

    private void Update()
    {
        if (idle) return;
        UpdateRunningOperations();
    }

    private void LateUpdate()
    {
        if (idle  != previouslyIdle) ToggleState();
    }

    private void UpdateRunningOperations()
    {
        for (int i = 0; i < maximumAmountOfOperationsRunning; i++)
        {
            var operation = runningOperations[i];
            if (operation.asyncOperation != null)
            {
                if (operation.asyncOperation.progress > 0.88f)
                {
                    //if (operation.operationType == OperationType.Load)
                    //    operation.asyncOperation.allowSceneActivation = true;

                    if (operation.asyncOperation.isDone)
                    {
                        operation.asyncOperation = null;
                        amountOfOperationsRunning--;
                    }
                }
            }
            else
            {
                if (enqueuedOperations.Count > 0)
                {
                    operation = enqueuedOperations[0];
                    amountOfOperationsRunning++;

                    switch (operation.operationType)
                    {
                        case OperationType.Load:
                            operation.asyncOperation = SceneManager.LoadSceneAsync(operation.sceneName, LoadSceneMode.Additive);

                            //operation.asyncOperation.allowSceneActivation = false;

                            if (operation.operationCallback != null) operation.asyncOperation.completed
                                    += operation.operationCallback;

                            operation.asyncOperation.priority
                                = amountOfOperationsRunning > priorityThreshold ? 0 : 8;

                            break;

                        case OperationType.Unload:
                            operation.asyncOperation = SceneManager.UnloadSceneAsync(operation.sceneName, UnloadSceneOptions.None);
                            if ( operation.asyncOperation != null ) operation.asyncOperation.priority = 8;

                            break;

                        default:
                            break;
                    }

                    enqueuedOperations.RemoveAt(0);
                    amountOfOperationsEnqueued--;
                }
            }
        }
    }

    private void ToggleState()
    {
        previouslyIdle = idle;

        TogglePhysics();
        ToggleBackgroundThreadsPriority();
        ToggleResolution();
    }

    public void EnqueueAsynchronousOperation
        (
            OperationType operationType,
            string sceneName,
            Action<AsyncOperation> callbackOnComplete = null
        )
    {
       // return;

        var newOperation = new Operation
        {
            operationType = operationType,
            sceneName = sceneName
        };

        if (callbackOnComplete != null) newOperation.operationCallback += callbackOnComplete;

        enqueuedOperations.Add(newOperation);
        amountOfOperationsEnqueued++;
    }

    private void ToggleBackgroundThreadsPriority()
    {
        if (idle)
        {
            Application.backgroundLoadingPriority = threadPriorityWhileIdle;
        }
        else
        {
            Application.backgroundLoadingPriority = threadPriorityWhileRunning;
        }
    }

    private void ToggleResolution()
    {
        if (idle)
        {
            Screen.SetResolution(960, 540, false, 60);
            Application.targetFrameRate = 60;
        }
        else
        {
            Screen.SetResolution(640, 360, false, 30);
            Application.targetFrameRate = 30;
        }
    }

    private void TogglePhysics()
    {
        if (idle)
        {
            Physics.autoSimulation = true;
        }
        else
        {
            Physics.autoSimulation = false;
        }
    }
}
