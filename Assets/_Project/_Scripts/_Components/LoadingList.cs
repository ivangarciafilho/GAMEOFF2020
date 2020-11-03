using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LoadingList : Service<LoadingList>
{
	[Serializable]
	public struct SceneData
	{
		public string sceneName;
		public int copies;
		public bool activeScene;
	}

	public bool loadInEditor = false;
	[ReorderableList] public SceneData[] scenes;
	private int loadingScenes;
	public UnityEvent OnFinish = new UnityEvent();
	public bool resetTimeScale = true;

	protected override void Awake()
	{
		base.Awake();

		if (!loadInEditor && Application.isEditor) return;
		Load();
	}

	public IEnumerator ReloadAll(Scene sceneToKeepWhileLoading, Action onFinish)
	{
		resetTimeScale = false;

		SceneManager.SetActiveScene( sceneToKeepWhileLoading );

		List<int> buildIndexes = new List<int>();
		var count = SceneManager.sceneCount;
		//while (count > 1)
		{
			for (int i = 0; i < count; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if(scene != sceneToKeepWhileLoading)
				{
					buildIndexes.Add(scene.buildIndex);
					//AsyncOperation op = SceneManager.UnloadSceneAsync(scene.buildIndex);

					//if (op != null)
					{
						//while (op.progress < 0.89f)
						//	yield return null;
					}
				}
			}
		}

		for (int i = 0; i < buildIndexes.Count; i++)
		{
			var op = SceneManager.UnloadSceneAsync(buildIndexes[i]);
		}

		while (SceneManager.sceneCount > 1) yield return null;

		Load(sceneToKeepWhileLoading.name);

		OnFinish.RemoveAllListeners();
		OnFinish.AddListener(() => onFinish());

	}

	public void Load ( string ignoreSceneByName = null )
	{
		//if ( ! loadInEditor && Application.isEditor ) return;

		var amountOfScenes = scenes.Length;

		for ( int i = 0; i < amountOfScenes; i++ )
		{
			if ( ! String.IsNullOrEmpty ( scenes [ i ].sceneName ) )
			{
				if(!string.IsNullOrEmpty(ignoreSceneByName))
				{
					if (scenes[i].sceneName == ignoreSceneByName)
						continue;
				}

				var copies = scenes [ i ].copies;
				for ( int j = 0; j < copies; j++ )
				{

					var loadingScene = SceneManager.
					LoadSceneAsync ( scenes [ i ].sceneName,
					LoadSceneMode.Additive );

					loadingScene.completed 
						+= SetReady;

					if ( scenes [ i ].activeScene )
						loadingScene.completed 
							+= SetActiveScene;

					loadingScenes++;
				}
			}
		}

		if ( loadingScenes > 0 ) 
			StartCoroutine ( WaitForAllToCompleteLoading ( ) );
	}

	private IEnumerator WaitForAllToCompleteLoading ( )
	{
		while ( true )
		{
			Time.timeScale = 0;
			Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;

			if ( loadingScenes < 1 ) break;

			yield return null;
		}

		if(resetTimeScale)
			Time.timeScale = 1;

		resetTimeScale = true;

		OnFinish.Invoke();

		SceneManager.UnloadSceneAsync("Loader");
	}
	private void SetReady ( AsyncOperation scene )
		=> loadingScenes--;

	private void SetActiveScene (AsyncOperation scene ) 
		=> StartCoroutine ( WaitAndSetTheActiveScene ( scene ) );

	private IEnumerator WaitAndSetTheActiveScene ( AsyncOperation scene )
	{
		var sceneToSetAsActive = scenes.
			First ( _item => _item.activeScene ).sceneName;

		bool IsLoadedAndActiveInHyerarchy ( )
		{
			var amountOfLoadedScenes = SceneManager.sceneCount;
			var loadedScenes = new Scene [ amountOfLoadedScenes ];

			for ( int i = 0; i < amountOfLoadedScenes; i++ )
				loadedScenes [ i ] = SceneManager.GetSceneAt ( i );

			loadedScenes = loadedScenes.Where (
				_scene => _scene.IsValid ( )
				&& _scene.isLoaded ).
				ToArray ( );

			return loadedScenes.
				Any ( _scene => _scene.name == sceneToSetAsActive );
		}

		while ( scene.progress < 0.8999 
			|| scene.isDone == false 
			|| IsLoadedAndActiveInHyerarchy() == false
			)
			yield return null;

		//yield return new  WaitForSecondsRealtime (2f);

		while ( Time.timeScale == 0 ) yield return null;
		
		//var skippedFrames = 0;
		//while ( skippedFrames < 5 ) yield return null;

		SceneManager.SetActiveScene (SceneManager.
			GetSceneByName(sceneToSetAsActive));
	}
}
