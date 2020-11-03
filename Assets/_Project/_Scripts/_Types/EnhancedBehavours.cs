/*===================================================================
Product:    ScriptingTest
Developer:  Ivan Garcia Filho - ivan.garcia.filho@gmail.com
Company:    The Spare Parts https://www.facebook.com/IvanGarciaFilho
Date:       30/05/2017 09:45

Please, don't distribute this code without asking me beforehand and
MY AGREEMENT! You SHALL give me the credits for using EnhancedBehaviours.cs!
Ps : EVEN FOR NON-COMMERCIAL PURPOSES! Thanks for playing fair!
====================================================================*/

using Bloodstone;
using System;
using System.Collections.Generic;
using System.Linq;
using UltEvents;
using UnityEngine;
using UnityEngine.AI;

namespace EnhancedBehaviours
{
	public class EnhancedBehaviour : MonoBehaviour
	{
		public interface IEnhancedEventBase<TDelegate> where TDelegate : class
		{
			bool AddListener ( TDelegate listener );
			bool RemoveListener ( TDelegate listener );
			void RemoveAll ( );
		}

		public abstract class EnhancedEventBase<TDelegate> : IEnhancedEventBase<TDelegate> where TDelegate : class
		{
			protected TDelegate [ ] listeners;
			protected uint count = 0;
			protected uint cap = 0;

			public bool AddListener ( TDelegate listener )
			{
				if ( count == cap )
				{
					if ( cap == 0 )
					{
						cap++;
					}

					cap *= 2;
					listeners = Expand ( listeners, cap, count );
				}
				listeners [ count ] = listener;
				++count;

				return true;
			}

			public void RemoveAll ( )
			{
				count = 0;
				Array.Clear ( listeners, 0, ( int ) cap );
			}

			public bool RemoveListener ( TDelegate listener )
			{
				var result = false;

				for ( uint i = 0; i < count; i++ )
				{
					if ( listeners [ i ].Equals ( listener ) )
					{
						RemoveAt ( i );
						result = true;
						break;
					}
				}

				return result;
			}

			protected void RemoveAt ( uint i )
			{
				count = RemoveAt ( listeners, count, i );
			}

			protected uint RemoveAt ( TDelegate [ ] arr, uint count, uint i )
			{
				--count;
				for ( uint j = i; j < count; ++j )
				{
					arr [ j ] = arr [ j + 1 ];
				}

				arr [ count ] = null;
				return count;
			}

			private TDelegate [ ] Expand ( TDelegate [ ] arr, uint cap, uint count )
			{
				TDelegate [ ] newArr = new TDelegate [ cap ];
				for ( int i = 0; i < count; ++i )
				{
					newArr [ i ] = arr [ i ];
				}
				return newArr;
			}

			private bool Contains ( TDelegate [ ] arr, uint c, TDelegate d )
			{
				for ( uint i = 0; i < c; ++i )
				{
					if ( arr [ i ].Equals ( d ) )
					{
						return true;
					}
				}
				return false;
			}

		}


		public class EnhancedEvent : EnhancedEventBase<Action>
		{
			public void Dispatch ( )
			{
				for ( uint i = count; i > 0; i-- )
				{
					if ( i > count )
					{
						throw new Exception ( "Index bigger than count, how is that???" );
					}

					if ( listeners [ i - 1 ] != null )
					{
						listeners [ i - 1 ] ( );
					}
					else
					{
						RemoveAt ( i - 1 );
					}
				}
			}
		}

		public class EnhancedEvent<T> : EnhancedEventBase<Action<T>>
		{
			public void Dispatch ( T t )
			{
				for ( uint i = count; i > 0; i-- )
				{
					if ( i > count )
					{
						throw new Exception ( "Index bigger than count, how is that???" );
					}

					if ( listeners [ i - 1 ] != null )
					{
						listeners [ i - 1 ] ( t );
					}
					else
					{
						RemoveAt ( i - 1 );
					}
				}
			}
		}

		public class EnhancedEvent<T, U> : EnhancedEventBase<Action<T, U>>
		{
			public void Dispatch ( T t, U u )
			{
				for ( uint i = count; i > 0; i-- )
				{
					if ( i > count )
					{
						throw new Exception ( "Index bigger than count, how is that???" );
					}

					if ( listeners [ i - 1 ] != null )
					{
						listeners [ i - 1 ] ( t, u );
					}
					else
					{
						RemoveAt ( i - 1 );
					}
				}
			}
		}

		public class EnhancedEvent<T, U, V> : EnhancedEventBase<Action<T, U, V>>
		{
			public void Dispatch ( T t, U u, V v )
			{
				for ( uint i = count; i > 0; i-- )
				{
					if ( i > count )
					{
						throw new Exception ( "Index bigger than count, how is that???" );
					}

					if ( listeners [ i - 1 ] != null )
					{
						listeners [ i - 1 ] ( t, u, v );
					}
					else
					{
						RemoveAt ( i - 1 );
					}
				}
			}
		}

		public class EnhancedEvent<T, U, V, W> : EnhancedEventBase<Action<T, U, V, W>>
		{
			public void Dispatch ( T t, U u, V v, W w )
			{
				for ( uint i = count; i > 0; i-- )
				{
					if ( i > count )
					{
						throw new Exception ( "Index bigger than count, how is that???" );
					}

					if ( listeners [ i - 1 ] != null )
					{
						listeners [ i - 1 ] ( t, u, v, w );
					}
					else
					{
						RemoveAt ( i - 1 );
					}
				}
			}
		}

		[DisallowMultipleComponent]
		public class EventsScheduler : EnhancedBehaviour
		{
			public static EventsScheduler eventsSchedulerInstance { get; private set; }
			[SerializeField] [Range ( 0, 30 )] private int defaultGlobalFrameskipping = 0;
			private static int currentFrameSkipping;
			private static int framesToNextGlobalUpdate;

			public static EnhancedEvent BeforeUpdate = new EnhancedEvent ( );
			public static EnhancedEvent AfterUpdate = new EnhancedEvent ( );

			[SerializeField] [Range ( 1, 90 )] private int slowUpdateFrameSkipping = 3;
			public static EnhancedEvent BeforeSlowUpdate = new EnhancedEvent ( );
			public static EnhancedEvent AfterSlowUpdate = new EnhancedEvent ( );
			private static int framesToSlowUpdate = 0;

			public static EnhancedEvent BeforeLateUpdate = new EnhancedEvent ( );
			public static EnhancedEvent AfterLateUpdate = new EnhancedEvent ( );

			[SerializeField] [Range ( 2, 180 )] private int slowLateUpdateFrameSkipping = 9;
			public static EnhancedEvent BeforeSlowLateUpdate = new EnhancedEvent ( );
			public static EnhancedEvent AfterSlowLateUpdate = new EnhancedEvent ( );
			private static int framesToSlowLateUpdate = 0;

			public static EnhancedEvent BeforeFixedUpdate = new EnhancedEvent ( );
			public static EnhancedEvent AfterFixedUpdate = new EnhancedEvent ( );

			[SerializeField] [Range ( 1f, 30f )] private float slowFixedUpdateDelay = 2f;
			public static EnhancedEvent BeforeSlowFixedUpdate = new EnhancedEvent ( );
			public static EnhancedEvent AfterSlowFixedUpdate = new EnhancedEvent ( );
			private static float nextSlowFixedUpdate = 0f;
			private bool fixedUpdateLogicAlreadyExecutedThisFrame = false;

			[SerializeField] [Range ( 2f, 30f )] private float verySlowFixedUpdateDelay = 6f;
			public static EnhancedEvent BeforeVerySlowFixedUpdate = new EnhancedEvent ( );
			public static EnhancedEvent AfterVerySlowFixedUpdate = new EnhancedEvent ( );
			private static float nextVerySlowFixedUpdate = 0f;

			public static EnhancedEvent Execute15TimesPerSecond = new EnhancedEvent ( );
			public const float delayBetweenFramesTo15ExecutionsPerSecond = 1 / 15;
			private static float nextExecutionOf15FramesPerSecond;

			public static EnhancedEvent Execute30TimesPerSecond = new EnhancedEvent ( );
			public const float delayBetweenFramesTo30ExecutionsPerSecond = 1 / 30;
			private static float nextExecutionOf30FramesPerSecond;

			public static EnhancedEvent Execute60TimesPerSecond = new EnhancedEvent ( );
			public const float delayBetweenFramesTo60ExecutionsPerSecond = 1 / 60;
			private static float nextExecutionOf60FramesPerSecond;

			public static EnhancedEvent OnAlternateUpdate0 = new EnhancedEvent ( );
			public static EnhancedEvent OnAlternateUpdate1 = new EnhancedEvent ( );
			private static bool currentAlternateUpdate = false;

			public static List<EnhancedEvent> lazyUpdateDelegates = new List<EnhancedEvent> ( );
			private static int currentLazyUpdateDelegate = 0;

			[SerializeField] private float maximumLogicExecutionTime = 1 / 30f;
			public static float currentRenderRate { get; private set; }
			public static float currentLogicExecutionRate { get; private set; }
			private float previousLogicExecutionEndTime = 0f;
			private float currentEllapsedRealtime = 0f;

			private float currentLogicExecutionEllapsedTime
			{
				get
				{
					currentEllapsedRealtime = Time.realtimeSinceStartup;
					return currentEllapsedRealtime - previousLogicExecutionEndTime;
				}
			}

			private bool exceededMaximumLogicExecutionTime 
				=> currentLogicExecutionEllapsedTime > maximumLogicExecutionTime;

			protected virtual void Awake ( )
			{
				if ( eventsSchedulerInstance != null )
				{
					if ( eventsSchedulerInstance != this )
					{
						DestroyImmediate ( this );
					}
				}

				eventsScheduler = eventsSchedulerInstance = this;
				DontDestroyOnLoad ( gameObject );

				slowUpdateFrameSkipping = Mathf.Clamp ( slowUpdateFrameSkipping, 1, 90 );
				slowLateUpdateFrameSkipping = Mathf.Clamp ( slowLateUpdateFrameSkipping, 2, 180 );
				framesToSlowUpdate = Mathf.Clamp ( slowUpdateFrameSkipping, 1, framesToSlowLateUpdate );
				framesToSlowLateUpdate = Mathf.Clamp ( slowLateUpdateFrameSkipping, framesToSlowUpdate, 60 );

				slowFixedUpdateDelay = Mathf.Clamp ( slowFixedUpdateDelay, 1, 30 );
				verySlowFixedUpdateDelay = Mathf.Clamp ( verySlowFixedUpdateDelay, 1, 90 );
				nextSlowFixedUpdate = Time.time + Mathf.Clamp ( slowFixedUpdateDelay, 1, verySlowFixedUpdateDelay );
				nextVerySlowFixedUpdate = Time.time + Mathf.Clamp ( slowFixedUpdateDelay, slowFixedUpdateDelay, 90 );

				currentFrameSkipping = defaultGlobalFrameskipping;

				AfterLateUpdate.AddListener ( ClearGarbage );
			}

			protected virtual void Update ( ) { ManageOnUpdateHooks ( ); }
			protected virtual void LateUpdate ( ) { ManageOnLateUpdateHooks ( ); }
			protected virtual void FixedUpdate ( ) { ManageOnFixedUpdateHooks ( ); }

			protected virtual void ManageOnUpdateHooks ( )
			{
				timeScale = Time.timeScale;
				if ( timeScale == 0 )
				{
					return;
				}

				time = Time.time;

				deltatime = Time.deltaTime;
				halfDeltatime = deltatime / 2f;
				twiceDeltatime = deltatime * 2f;
				thriceDeltatime = deltatime * 3f;

				smoothDeltatime = Time.smoothDeltaTime;
				halfSmoothDeltatime = smoothDeltatime / 2f;
				twiceSmoothDeltatime = smoothDeltatime * 2f;
				thriceSmoothDeltatime = smoothDeltatime * 3f;

				currentRenderRate = 1f / deltatime;

				fixedUpdateLogicAlreadyExecutedThisFrame = false;

				if ( framesToNextGlobalUpdate > 0 ) { framesToNextGlobalUpdate--; return; }
				framesToNextGlobalUpdate = currentFrameSkipping;

				if ( BeforeUpdate != null )
				{
					BeforeUpdate.Dispatch ( );
				}

				if ( AfterUpdate != null )
				{
					AfterUpdate.Dispatch ( );
				}

				if ( exceededMaximumLogicExecutionTime )
				{
#if UNITY_EDITOR
					//Debug.Log("EXCEEDED MAXIMUM LOGIC EXECUTION TIME, Skipped AlternateUpdate");
#endif
					return;
				}

				if ( currentAlternateUpdate )
				{
					if ( OnAlternateUpdate1 != null )
					{
						OnAlternateUpdate1.Dispatch ( );
					}
				}
				else
				{
					if ( OnAlternateUpdate0 != null )
					{
						OnAlternateUpdate0.Dispatch ( );
					}
				}
				currentAlternateUpdate = !currentAlternateUpdate;

				if ( exceededMaximumLogicExecutionTime )
				{
#if UNITY_EDITOR
					Debug.Log("EXCEEDED MAXIMUM LOGIC EXECUTION TIME, Skipped SlowUpdate");
#endif
					return;
				}

				if ( framesToSlowUpdate > 0 ) { framesToSlowUpdate--; return; }
				framesToSlowUpdate = slowUpdateFrameSkipping;

				if ( BeforeSlowUpdate != null )
				{
					BeforeSlowUpdate.Dispatch ( );
				}

				if ( AfterSlowUpdate != null )
				{
					AfterSlowUpdate.Dispatch ( );
				}
			}

			protected virtual void ManageOnLateUpdateHooks ( )
			{
				previousLogicExecutionEndTime = Time.realtimeSinceStartup;

				var delegatesToExecuteOnCurrentFrame = ( int ) Mathf.Ceil ( currentRenderRate / lazyUpdateDelegates.Count );

				var i = 0;
				var totalAmountOfLazyDelegates = lazyUpdateDelegates.Count;
				for ( i = currentLazyUpdateDelegate; i < delegatesToExecuteOnCurrentFrame; i++ )
				{
					if ( i < totalAmountOfLazyDelegates && !exceededMaximumLogicExecutionTime )
					{
						lazyUpdateDelegates [ i ].Dispatch ( );
					}
					else
					{
						currentLazyUpdateDelegate = 0;
						break;
					}
				}
				currentLazyUpdateDelegate = i;

				if ( exceededMaximumLogicExecutionTime )
				{
#if UNITY_EDITOR
					Debug.Log ( "EXCEEDED MAXIMUM LOGIC EXECUTION TIME, Skipped LateUpdate" );
#endif
					return;
				}

				if ( framesToNextGlobalUpdate > 0 )
				{
					return;
				}

				if ( BeforeLateUpdate != null )
				{
					BeforeLateUpdate.Dispatch ( );
				}

				if ( AfterLateUpdate != null )
				{
					AfterLateUpdate.Dispatch ( );
				}

				if ( exceededMaximumLogicExecutionTime )
				{
#if UNITY_EDITOR
					Debug.Log ( "EXCEEDED MAXIMUM LOGIC EXECUTION TIME, Skipped SlowLateUpdate" );
#endif
					return;
				}

				if ( framesToSlowLateUpdate > 0 ) { framesToSlowLateUpdate--; return; }
				framesToSlowLateUpdate = slowLateUpdateFrameSkipping;

				if ( BeforeSlowLateUpdate != null )
				{
					BeforeSlowLateUpdate.Dispatch ( );
				}

				if ( AfterSlowLateUpdate != null )
				{
					AfterSlowLateUpdate.Dispatch ( );
				}
			}

			protected virtual void ManageOnFixedUpdateHooks ( )
			{
				fixedDeltatime = Time.fixedDeltaTime;

				if ( fixedUpdateLogicAlreadyExecutedThisFrame ) { return; }
				fixedUpdateLogicAlreadyExecutedThisFrame = true;


				if ( BeforeFixedUpdate != null )
				{
					BeforeFixedUpdate.Dispatch ( );
				}

				if ( AfterFixedUpdate != null )
				{
					AfterFixedUpdate.Dispatch ( );
				}

				if ( exceededMaximumLogicExecutionTime )
				{
#if UNITY_EDITOR
					Debug.Log("EXCEEDED MAXIMUM LOGIC EXECUTION TIME, Skipped ExceuteXTimesPerSecond");
#endif
					return;
				}

				if ( time > nextExecutionOf15FramesPerSecond )
				{
					nextExecutionOf15FramesPerSecond = time + delayBetweenFramesTo15ExecutionsPerSecond;
					if ( Execute15TimesPerSecond != null )
					{
						Execute15TimesPerSecond.Dispatch ( );
					}

					if ( time > nextExecutionOf30FramesPerSecond )
					{
						nextExecutionOf30FramesPerSecond = time + delayBetweenFramesTo30ExecutionsPerSecond;
						if ( Execute30TimesPerSecond != null )
						{
							Execute30TimesPerSecond.Dispatch ( );
						}

						if ( time > nextExecutionOf60FramesPerSecond )
						{
							nextExecutionOf60FramesPerSecond = time + delayBetweenFramesTo60ExecutionsPerSecond;
							if ( Execute60TimesPerSecond != null )
							{
								Execute60TimesPerSecond.Dispatch ( );
							}
						}
					}
				}

				if ( exceededMaximumLogicExecutionTime )
				{
#if UNITY_EDITOR
					Debug.Log ( "EXCEEDED MAXIMUM LOGIC EXECUTION TIME, Skipped SlowFixedUpdate" );
#endif
					return;
				}


				if ( time < nextSlowFixedUpdate )
				{
					return;
				}

				nextSlowFixedUpdate = time + slowFixedUpdateDelay;

				if ( BeforeSlowFixedUpdate != null ) { BeforeSlowFixedUpdate.Dispatch ( ); }
				if ( AfterSlowFixedUpdate != null )
				{
					AfterSlowFixedUpdate.Dispatch ( );
				}

				if ( exceededMaximumLogicExecutionTime )
				{
#if UNITY_EDITOR
					Debug.Log ( "EXCEEDED MAXIMUM LOGIC EXECUTION TIME, Skipped VerySlowFixedUpdate" );
#endif
					return;
				}

				if ( time < nextVerySlowFixedUpdate )
				{
					return;
				}

				nextVerySlowFixedUpdate = time + verySlowFixedUpdateDelay;

				if ( BeforeVerySlowFixedUpdate != null )
				{
					BeforeVerySlowFixedUpdate.Dispatch ( );
				}

				if ( AfterVerySlowFixedUpdate != null )
				{
					AfterVerySlowFixedUpdate.Dispatch ( );
				}
			}

			protected virtual void SetFrameskipping ( int frames = 1 )
			{
				if ( frames < 0 )
				{
					return;
				}

				currentFrameSkipping = frames;
			}

			private void ClearGarbage ( )
			{
				System.GC.Collect ( );
				AfterLateUpdate.RemoveListener ( ClearGarbage );
			}
		}

		[DisallowMultipleComponent]
		public class Protagonist : EnhancedBehaviour
		{
			public static Protagonist protagonistInstance { get; private set; }
			protected virtual void Awake ( )
			{
				if ( protagonistInstance != null )
				{
					if ( protagonistInstance != this )
					{
						DestroyImmediate ( this );
					}
				}

				protagonist = protagonistInstance = this;
				DontDestroyOnLoad ( gameObject );
			}
		}

		[DisallowMultipleComponent]
		public class MainCamera : EnhancedBehaviour
		{
			public static MainCamera viewportInstance { get; private set; }
			public static Camera itsCamera { get; private set; }

			protected virtual void Awake ( )
			{
				if ( viewportInstance != null )
				{
					if ( viewportInstance != this )
					{
						DestroyImmediate ( this );
					}
				}

				mainViewport = viewportInstance = this;
				DontDestroyOnLoad ( gameObject );

				itsCamera = GetComponentsInChildren<Camera> ( ) [ 0 ];
				itsCamera.eventMask = 0;
			}
		}

		[DisallowMultipleComponent]
		public class InputsManager : EnhancedBehaviour
		{
			public static InputsManager inputManagerIntance { get; private set; }

			protected virtual void Awake ( )
			{
				if ( inputManagerIntance != null )
				{
					if ( inputManagerIntance != this )
					{
						DestroyImmediate ( this );
					}
				}

				inputManagerIntance = this;
				inputManager = this;
				DontDestroyOnLoad ( gameObject );
			}

			protected virtual void Update ( ) { UpdateUserInput ( ); }

			protected virtual void UpdateUserInput ( )
			{

				currentVerticalAxisInput = Input.GetAxis ( "Vertical" );
				currentHorizontalAxisInput = Input.GetAxis ( "Horizontal" );

				vector3InputKeyAxis.x = currentVerticalAxisInput;
				vector3InputKeyAxis.z = currentHorizontalAxisInput;

				verticalAxisInputMouse = Input.GetAxis ( "Mouse Y" );
				horizontalAxisInputmouse = Input.GetAxis ( "Mouse X" );

				vector2InputMouseAxis.x = horizontalAxisInputmouse;
				vector2InputMouseAxis.y = verticalAxisInputMouse;

				mouseWheelScrollAxis = Input.GetAxis ( "Mouse ScrollWheel" );

				currentLeftAxisInput = Input.GetKey ( KeyCode.Q ) ? 1 : Input.GetKey ( KeyCode.A ) ? -1 : 0;
				currentMidleAxisInput = Input.GetKey ( KeyCode.W ) ? 1 : Input.GetKey ( KeyCode.S ) ? -1 : 0;
				currentRightAxisInput = Input.GetKey ( KeyCode.E ) ? 1 : Input.GetKey ( KeyCode.D ) ? -1 : 0;
				currentTopAxisInput = Input.GetKey ( KeyCode.E ) ? 1 : Input.GetKey ( KeyCode.Q ) ? -1 : 0;
				currentBottomAxisInput = Input.GetKey ( KeyCode.D ) ? 1 : Input.GetKey ( KeyCode.A ) ? -1 : 0;
			}
		}

		//Solution Agnostic
		[HideInInspector] public int itsInstanceID;

		[Header ( "Behaviour Properties : " )]
		//Shortcut to main Entities
		protected static EventsScheduler eventsScheduler;
		protected static Protagonist protagonist;
		protected static MainCamera mainViewport;
		protected static InputsManager inputManager;

		//Shortcuts to gameobject's statistics
		[HideInInspector] public GameObject itsGameobject;
		[HideInInspector] public int itsGameobjectID;
		protected static Dictionary<GameObject, EnhancedBehaviour> behaviourByGameobject;

		[HideInInspector] public Transform itsTransform;
		[HideInInspector] public int itsChildCount;
		[HideInInspector] public int itsTransformID;
		protected static Dictionary<Transform, EnhancedBehaviour> behaviourByTransform;

		[HideInInspector] public Vector3 itsPosition;
		[HideInInspector] public Vector2 its2DPosition;
		[HideInInspector] public Vector3 itsLocalPosition;
		[HideInInspector] public Vector2 its2DLocalPosition;
		[HideInInspector] public Quaternion itsRotation;
		[HideInInspector] public Quaternion itsYaw;
		[HideInInspector] public Quaternion itsPitch;
		[HideInInspector] public Quaternion itsRoll;
		[HideInInspector] public Quaternion itsLocalRotation;
		[HideInInspector] public Quaternion itsLocalYaw;
		[HideInInspector] public Quaternion itsLocalPitch;
		[HideInInspector] public Quaternion itsLocalRoll;
		[HideInInspector] public Vector3 itsEulerRotation;
		[HideInInspector] public Vector3 itsYawEulerRotation;
		[HideInInspector] public Vector3 itsPitchEulerRotation;
		[HideInInspector] public Vector3 itsRollEulerRotation;
		[HideInInspector] public Vector3 itsEulerLocalRotation;
		[HideInInspector] public Vector3 itsYawEulerLocalRotation;
		[HideInInspector] public Vector3 itsPitchEulerLocalRotation;
		[HideInInspector] public Vector3 itsRollEulerLocalRotation;
		[HideInInspector] public Vector3 itsForward;
		[HideInInspector] public Vector3 itsRight;
		[HideInInspector] public Vector3 itsUp;

		[HideInInspector] public bool hasRigidbody;
		[HideInInspector] public Rigidbody itsRigidbody;
		[HideInInspector] public int itsRigidbodyID;
		protected static Dictionary<Rigidbody, EnhancedBehaviour> behaviourByRigidbodies;
		protected static Dictionary<Collider, Rigidbody> rigidbodyByCollider;
		protected static Dictionary<Collider, Rigidbody> rigidbodyByTrigger;
		protected static Dictionary<NavMeshAgent, Rigidbody> rigidbodyByNavmeshAgent;
		protected static Dictionary<Animator, Rigidbody> rigidbodyByAnimator;
		protected static Dictionary<GameObject, Rigidbody> rigidbodyByGameobject;
		protected static Dictionary<Transform, Rigidbody> rigidbodyByTransform;

		[HideInInspector] public Vector3 itsCenterOfMass;
		[HideInInspector] public Vector3 itsVelocity;
		[HideInInspector] public Vector3 itsAngularVelocity;

		[HideInInspector] public bool hasColliders;
		[HideInInspector] public Collider [ ] itsColliders;
		[HideInInspector] public int [ ] itsCollidersIDs;
		[HideInInspector] public int amountOfColliders;
		protected static Dictionary<Collider, EnhancedBehaviour> behaviourByColliders;
		protected static Dictionary<Rigidbody, Collider [ ]> collidersByRigidbody;
		protected static Dictionary<NavMeshAgent, Collider [ ]> collidersByNavmeshAgent;
		protected static Dictionary<Animator, Collider [ ]> collidersByAnimator;
		protected static Dictionary<GameObject, Collider [ ]> collidersByGameobject;
		protected static Dictionary<Transform, Collider [ ]> collidersByTransform;

		[HideInInspector] public bool hasTriggers;
		[HideInInspector] public Collider [ ] itsTriggers;
		[HideInInspector] public int [ ] itsTriggersIDs;
		[HideInInspector] public int amountOfTriggers;
		protected static Dictionary<Collider, EnhancedBehaviour> behaviourByTrigger;
		protected static Dictionary<Rigidbody, Collider [ ]> triggersByRigidbody;
		protected static Dictionary<NavMeshAgent, Collider [ ]> triggersByNavmeshAgent;
		protected static Dictionary<Animator, Collider [ ]> triggersByAnimator;
		protected static Dictionary<GameObject, Collider [ ]> triggersByGameobject;
		protected static Dictionary<Transform, Collider [ ]> triggersByTransform;

		[HideInInspector] public bool hasAnimator;
		[HideInInspector] public Animator itsAnimator;
		[HideInInspector] public int itsAnimatorID;
		protected static Dictionary<Animator, EnhancedBehaviour> behaviourByAnimator;
		protected static Dictionary<Rigidbody, Animator> animatorByRigidbody;
		protected static Dictionary<Collider, Animator> animatorByCollider;
		protected static Dictionary<Collider, Animator> animatorByTrigger;
		protected static Dictionary<NavMeshAgent, Animator> animatorByNavmeshAgent;
		protected static Dictionary<GameObject, Animator> animatorByGameobject;
		protected static Dictionary<Transform, Animator> animatorByTransform;

		[HideInInspector] public bool hasNavmeshAgent;
		[HideInInspector] public NavMeshAgent itsNavmeshAgent;
		[HideInInspector] public int itsNavMeshAgentID;
		protected static Dictionary<NavMeshAgent, EnhancedBehaviour> behaviourByNavmeshAgent;
		protected static Dictionary<Rigidbody, NavMeshAgent> navmeshAgentByRigidbody;
		protected static Dictionary<Collider, NavMeshAgent> navmeshAgentByCollider;
		protected static Dictionary<Collider, NavMeshAgent> navmeshAgentByTrigger;
		protected static Dictionary<Animator, NavMeshAgent> navmeshAgentByAnimator;
		protected static Dictionary<GameObject, NavMeshAgent> navmeshAgentByGameobject;
		protected static Dictionary<Transform, NavMeshAgent> navmeshAgentByTransform;

		[HideInInspector] public bool hasNavmeshObstacle;
		[HideInInspector] public NavMeshObstacle itsNavmeshObstacle;
		[HideInInspector] public int itsNavmeshObstacleID;

		protected static readonly Vector3 vector3Zero = new Vector3 ( 0f, 0f, 0f );
		protected static readonly Vector3 vector3One = new Vector3 ( 1f, 1f, 1f );
		protected static readonly Vector3 vector3NegativeOne = new Vector3 ( -1f, -1f, -1f );
		protected static readonly Vector3 vector3Up = new Vector3 ( 0f, 1f, 0f );
		protected static readonly Vector3 vector3UpForward = new Vector3 ( 0f, 1f, 1f );
		protected static readonly Vector3 vector3UpBack = new Vector3 ( 0f, 1f, -1f );
		protected static readonly Vector3 vector3UpRight = new Vector3 ( 1f, 1f, 0f );
		protected static readonly Vector3 vector3UpLeft = new Vector3 ( -1f, 1f, 0f );
		protected static readonly Vector3 vector3Down = new Vector3 ( 0f, -1f, 0f );
		protected static readonly Vector3 vector3DownForward = new Vector3 ( 0f, -1f, 1f );
		protected static readonly Vector3 vector3DownBack = new Vector3 ( 0f, -1f, -1f );
		protected static readonly Vector3 vector3DownLeft = new Vector3 ( -1f, -1f, 0f );
		protected static readonly Vector3 vector3DownRight = new Vector3 ( 1f, -1f, 0f );
		protected static readonly Vector3 vector3Forward = new Vector3 ( 0f, 0f, 1f );
		protected static readonly Vector3 vector3ForwardLeft = new Vector3 ( -1f, 0f, 1f );
		protected static readonly Vector3 vector3ForwardRight = new Vector3 ( 1f, 0f, 1f );
		protected static readonly Vector3 vector3Back = new Vector3 ( 0f, 0f, -1f );
		protected static readonly Vector3 vector3BackLeft = new Vector3 ( -1f, 0f, -1f );
		protected static readonly Vector3 vector3BackRight = new Vector3 ( 1f, 0f, -1f );
		protected static readonly Vector3 vector3Right = new Vector3 ( 1f, 0f, 0f );
		protected static readonly Vector3 vector3Left = new Vector3 ( -1f, 0f, 0f );
		protected static readonly Quaternion quaternionIdentity = new Quaternion ( 0f, 0f, 0f, 0f );

		protected static float timeScale;
		protected static float time;
		protected static float halfDeltatime;
		protected static float deltatime;
		protected static float twiceDeltatime;
		protected static float thriceDeltatime;
		protected static float halfSmoothDeltatime;
		protected static float smoothDeltatime;
		protected static float twiceSmoothDeltatime;
		protected static float thriceSmoothDeltatime;
		protected static float fixedDeltatime;
		protected static float currentVerticalAxisInput;
		protected static float currentHorizontalAxisInput;
		protected static float currentLeftAxisInput;
		protected static float currentMidleAxisInput;
		protected static float currentRightAxisInput;
		protected static float currentTopAxisInput;
		protected static float currentBottomAxisInput;
		protected static float verticalAxisInputMouse;
		protected static float horizontalAxisInputmouse;
		protected static Vector3 vector3InputKeyAxis;
		protected static Vector3 vector2InputMouseAxis;
		protected static Vector3 gravity;
		protected static float mouseWheelScrollAxis;

		protected float executionTimeframe = 1 / 30f;
		protected float ellapsedDelay;
		protected static int executionFrameInterval = 1;
		protected float ellapsedFrames;

		protected EnhancedEvent OnEnableEvent = new EnhancedEvent ( );
		protected virtual void TriggerListenersOnEnable ( )
		{
			if ( OnEnableEvent != null )
			{
				OnEnableEvent.Dispatch ( );
			}
		}

		protected EnhancedEvent OnDisableEvent = new EnhancedEvent ( );
		protected virtual void TriggerListenersOnDisable ( )
		{
			if ( OnDisableEvent != null )
			{
				OnDisableEvent.Dispatch ( );
			}
		}

		protected EnhancedEvent OnDestroyEvent = new EnhancedEvent ( );
		protected virtual void TriggerListenersOnDestroy ( )
		{
			if ( OnDestroyEvent != null )
			{
				OnDestroyEvent.Dispatch ( );
			}
		}

		protected EnhancedEvent<Collision> OnCollisionEnterEvent = new EnhancedEvent<Collision> ( );
		protected virtual void CallOnCollisionEnterCallbacks ( Collision _collision )
		{
			if ( OnCollisionEnterEvent != null )
			{
				OnCollisionEnterEvent.Dispatch ( _collision );
			}
		}

		protected EnhancedEvent<Collision> OnCollisionExitEvent = new EnhancedEvent<Collision> ( );
		protected virtual void CallOnCollisionExitCallbacks ( Collision _collision )
		{
			if ( OnCollisionExitEvent != null )
			{
				OnCollisionExitEvent.Dispatch ( _collision );
			}
		}

		protected EnhancedEvent<Collider> OnTriggerEnterEvent = new EnhancedEvent<Collider> ( );
		protected virtual void CallOnTriggerEnterCallbacks ( Collider _collider )
		{
			if ( OnTriggerEnterEvent != null )
			{
				OnTriggerEnterEvent.Dispatch ( _collider );
			}
		}

		protected EnhancedEvent<Collider> OnTriggerExitEvent = new EnhancedEvent<Collider> ( );
		protected virtual void CallOnTriggerExitCallbacks ( Collider _collider )
		{
			if ( OnTriggerExitEvent != null )
			{
				OnTriggerExitEvent.Dispatch ( _collider );
			}
		}


		[ReadOnly, SerializeField] protected bool _initialized = false;
		public bool initialized => _initialized;
		protected virtual void Initialize ( )
		{
			itsInstanceID = GetInstanceID ( );

			//Cache GameObject
			if ( itsGameobject == null )
			{
				itsGameobject = gameObject;
			}

			if ( behaviourByGameobject == null )
			{
				behaviourByGameobject = new Dictionary<GameObject, EnhancedBehaviour> ( );
			}

			itsGameobjectID = itsGameobject.GetInstanceID ( );

			if ( ( behaviourByGameobject.ContainsKey ( itsGameobject ) == false ) )
			{
				behaviourByGameobject.Add ( itsGameobject, this );
			}


			//Cache Transform
			if ( behaviourByTransform == null )
			{
				behaviourByTransform = new Dictionary<Transform, EnhancedBehaviour> ( );
			}

			itsTransform = itsGameobject.GetComponent<Transform> ( );
			itsTransformID = itsTransform.GetInstanceID ( );
			itsChildCount = itsTransform.childCount;

			if ( ( behaviourByTransform.ContainsKey ( itsTransform ) == false ) )
			{
				behaviourByTransform.Add ( itsTransform, this );
			}


			//Cache Gravity
			gravity = Physics.gravity;


			//Cache Rigidbody
			if ( behaviourByRigidbodies == null )
			{
				behaviourByRigidbodies = new Dictionary<Rigidbody, EnhancedBehaviour> ( );
			}

			if ( hasRigidbody = itsRigidbody = itsGameobject.GetComponent<Rigidbody> ( ) )
			{
				itsRigidbodyID = itsRigidbody.GetInstanceID ( );
				if ( ( behaviourByRigidbodies.ContainsKey ( itsRigidbody ) == false ) )
				{
					behaviourByRigidbodies.Add ( itsRigidbody, this );
				}
			}


			//Cache Colliders
			if ( behaviourByColliders == null )
			{
				behaviourByColliders = new Dictionary<Collider, EnhancedBehaviour> ( );
			}

			itsColliders = itsGameobject.GetComponentsInChildren<Collider> ( ).
				Where ( _collider => _collider.isTrigger == false ).ToArray ( );

			amountOfColliders = itsColliders.Length;
			if ( hasColliders = ( amountOfColliders > 0 ) )
			{
				itsCollidersIDs = new int [ amountOfColliders ];
				for ( int i = 0; i < amountOfColliders; i++ )
				{
					itsCollidersIDs [ i ] = itsColliders [ i ].GetInstanceID ( );
					if ( ( behaviourByColliders.ContainsKey ( itsColliders [ i ] ) == false ) )
					{
						behaviourByColliders.Add ( itsColliders [ i ], this );
					}
				}
			}


			//Cache Triggers
			if ( behaviourByTrigger == null )
			{
				behaviourByTrigger = new Dictionary<Collider, EnhancedBehaviour> ( );
			}

			itsTriggers = itsGameobject.GetComponentsInChildren<Collider> ( ).
				Where ( _collider => _collider.isTrigger == true ).ToArray ( );

			amountOfTriggers = itsTriggers.Length;
			if ( hasTriggers = ( amountOfTriggers > 0 ) )
			{
				itsTriggersIDs = new int [ amountOfTriggers ];
				for ( int i = 0; i < amountOfTriggers; i++ )
				{
					itsTriggersIDs [ i ] = itsTriggers [ i ].GetInstanceID ( );
					if ( ( behaviourByTrigger.ContainsKey ( itsTriggers [ i ] ) == false ) )
					{
						behaviourByTrigger.Add ( itsTriggers [ i ], this );
					}
				}
			}


			//Cache NavMeshAgents
			if ( behaviourByNavmeshAgent == null )
			{
				behaviourByNavmeshAgent = new Dictionary<NavMeshAgent, EnhancedBehaviour> ( );
			}

			if ( hasNavmeshObstacle = itsNavmeshObstacle = itsGameobject.GetComponent<NavMeshObstacle> ( ) )
			{
				itsNavmeshObstacleID = itsNavmeshObstacle.GetInstanceID ( );
			}

			if ( hasNavmeshAgent = itsNavmeshAgent = itsGameobject.GetComponent<NavMeshAgent> ( ) )
			{
				itsNavMeshAgentID = itsNavmeshAgent.GetInstanceID ( );
				if ( ( behaviourByNavmeshAgent.ContainsKey ( itsNavmeshAgent ) == false ) )
				{
					behaviourByNavmeshAgent.Add ( itsNavmeshAgent, this );
				}
			}


			//Cache Animators
			if ( behaviourByAnimator == null )
			{
				behaviourByAnimator = new Dictionary<Animator, EnhancedBehaviour> ( );
			}

			if ( hasAnimator = itsAnimator = itsGameobject.GetComponent<Animator> ( ) )
			{
				itsAnimatorID = itsAnimator.GetInstanceID ( );
				if ( ( behaviourByAnimator.ContainsKey ( itsAnimator ) == false ) )
				{
					behaviourByAnimator.Add ( itsAnimator, this );
				}
			}


			//Create cross refference dictionaries

			//rigidbody
			if ( rigidbodyByCollider == null )
			{
				rigidbodyByCollider = new Dictionary<Collider, Rigidbody> ( );
			}

			if ( rigidbodyByTrigger == null )
			{
				rigidbodyByTrigger = new Dictionary<Collider, Rigidbody> ( );
			}

			if ( rigidbodyByNavmeshAgent == null )
			{
				rigidbodyByNavmeshAgent = new Dictionary<NavMeshAgent, Rigidbody> ( );
			}

			if ( rigidbodyByAnimator == null )
			{
				rigidbodyByAnimator = new Dictionary<Animator, Rigidbody> ( );
			}

			if ( rigidbodyByGameobject == null )
			{
				rigidbodyByGameobject = new Dictionary<GameObject, Rigidbody> ( );
			}

			if ( rigidbodyByTransform == null )
			{
				rigidbodyByTransform = new Dictionary<Transform, Rigidbody> ( );
			}

			if ( hasRigidbody )
			{
				if ( hasColliders )
				{
					for ( int i = 0; i < amountOfColliders; i++ )
					{
						if ( rigidbodyByCollider.ContainsKey ( itsColliders [ i ] ) == false )
						{
							rigidbodyByCollider.Add ( itsColliders [ i ], itsRigidbody );
						}
					}
				}

				if ( hasTriggers )
				{
					for ( int i = 0; i < amountOfTriggers; i++ )
					{
						if ( rigidbodyByTrigger.ContainsKey ( itsTriggers [ i ] ) == false )
						{
							rigidbodyByTrigger.Add ( itsTriggers [ i ], itsRigidbody );
						}
					}
				}

				if ( hasNavmeshAgent )
				{
					if ( rigidbodyByNavmeshAgent.ContainsKey ( itsNavmeshAgent ) == false )
					{
						rigidbodyByNavmeshAgent.Add ( itsNavmeshAgent, itsRigidbody );
					}
				}

				if ( hasAnimator )
				{
					if ( rigidbodyByAnimator.ContainsKey ( itsAnimator ) == false )
					{
						rigidbodyByAnimator.Add ( itsAnimator, itsRigidbody );
					}
				}

				if ( rigidbodyByGameobject.ContainsKey ( itsGameobject ) == false )
				{
					rigidbodyByGameobject.Add ( itsGameobject, itsRigidbody );
				}

				if ( rigidbodyByTransform.ContainsKey ( itsTransform ) == false )
				{
					rigidbodyByTransform.Add ( itsTransform, itsRigidbody );
				}
			}

			//Colliders
			if ( collidersByRigidbody == null )
			{
				collidersByRigidbody = new Dictionary<Rigidbody, Collider [ ]> ( );
			}

			if ( collidersByNavmeshAgent == null )
			{
				collidersByNavmeshAgent = new Dictionary<NavMeshAgent, Collider [ ]> ( );
			}

			if ( collidersByAnimator == null )
			{
				collidersByAnimator = new Dictionary<Animator, Collider [ ]> ( );
			}

			if ( collidersByGameobject == null )
			{
				collidersByGameobject = new Dictionary<GameObject, Collider [ ]> ( );
			}

			if ( collidersByTransform == null )
			{
				collidersByTransform = new Dictionary<Transform, Collider [ ]> ( );
			}

			if ( hasColliders )
			{
				if ( hasRigidbody )
				{
					if ( collidersByRigidbody.ContainsKey ( itsRigidbody ) == false )
					{
						collidersByRigidbody.Add ( itsRigidbody, itsColliders );
					}
				}

				if ( hasNavmeshAgent )
				{
					if ( collidersByNavmeshAgent.ContainsKey ( itsNavmeshAgent ) == false )
					{
						collidersByNavmeshAgent.Add ( itsNavmeshAgent, itsColliders );
					}
				}

				if ( hasAnimator )
				{
					if ( collidersByAnimator.ContainsKey ( itsAnimator ) == false )
					{
						collidersByAnimator.Add ( itsAnimator, itsColliders );
					}
				}

				if ( collidersByGameobject.ContainsKey ( itsGameobject ) == false )
				{
					collidersByGameobject.Add ( itsGameobject, itsColliders );
				}

				if ( collidersByTransform.ContainsKey ( itsTransform ) == false )
				{
					collidersByTransform.Add ( itsTransform, itsColliders );
				}
			}

			//Triggers
			if ( triggersByRigidbody == null )
			{
				triggersByRigidbody = new Dictionary<Rigidbody, Collider [ ]> ( );
			}

			if ( triggersByNavmeshAgent == null )
			{
				triggersByNavmeshAgent = new Dictionary<NavMeshAgent, Collider [ ]> ( );
			}

			if ( triggersByAnimator == null )
			{
				triggersByAnimator = new Dictionary<Animator, Collider [ ]> ( );
			}

			if ( triggersByGameobject == null )
			{
				triggersByGameobject = new Dictionary<GameObject, Collider [ ]> ( );
			}

			if ( triggersByTransform == null )
			{
				triggersByTransform = new Dictionary<Transform, Collider [ ]> ( );
			}

			if ( hasTriggers )
			{
				if ( hasRigidbody )
				{
					if ( triggersByRigidbody.ContainsKey ( itsRigidbody ) == false )
					{
						triggersByRigidbody.Add ( itsRigidbody, itsTriggers );
					}
				}

				if ( hasNavmeshAgent )
				{
					if ( triggersByNavmeshAgent.ContainsKey ( itsNavmeshAgent ) == false )
					{
						triggersByNavmeshAgent.Add ( itsNavmeshAgent, itsTriggers );
					}
				}

				if ( hasAnimator )
				{
					if ( triggersByAnimator.ContainsKey ( itsAnimator ) == false )
					{
						triggersByAnimator.Add ( itsAnimator, itsTriggers );
					}
				}

				if ( triggersByGameobject.ContainsKey ( itsGameobject ) == false )
				{
					triggersByGameobject.Add ( itsGameobject, itsTriggers );
				}

				if ( triggersByTransform.ContainsKey ( itsTransform ) == false )
				{
					triggersByTransform.Add ( itsTransform, itsTriggers );
				}
			}

			//Navmesh Agents
			if ( navmeshAgentByRigidbody == null )
			{
				navmeshAgentByRigidbody = new Dictionary<Rigidbody, NavMeshAgent> ( );
			}

			if ( navmeshAgentByCollider == null )
			{
				navmeshAgentByCollider = new Dictionary<Collider, NavMeshAgent> ( );
			}

			if ( navmeshAgentByTrigger == null )
			{
				navmeshAgentByTrigger = new Dictionary<Collider, NavMeshAgent> ( );
			}

			if ( navmeshAgentByAnimator == null )
			{
				navmeshAgentByAnimator = new Dictionary<Animator, NavMeshAgent> ( );
			}

			if ( navmeshAgentByGameobject == null )
			{
				navmeshAgentByGameobject = new Dictionary<GameObject, NavMeshAgent> ( );
			}

			if ( navmeshAgentByTransform == null )
			{
				navmeshAgentByTransform = new Dictionary<Transform, NavMeshAgent> ( );
			}

			if ( hasNavmeshAgent )
			{
				if ( hasRigidbody )
				{
					if ( navmeshAgentByRigidbody.ContainsKey ( itsRigidbody ) == false )
					{
						navmeshAgentByRigidbody.Add ( itsRigidbody, itsNavmeshAgent );
					}
				}

				if ( hasColliders )
				{
					for ( int i = 0; i < amountOfColliders; i++ )
					{
						if ( navmeshAgentByCollider.ContainsKey ( itsColliders [ i ] ) == false )
						{
							navmeshAgentByCollider.Add ( itsColliders [ i ], itsNavmeshAgent );
						}
					}
				}

				if ( hasTriggers )
				{
					for ( int i = 0; i < amountOfTriggers; i++ )
					{
						if ( navmeshAgentByTrigger.ContainsKey ( itsTriggers [ i ] ) == false )
						{
							navmeshAgentByTrigger.Add ( itsTriggers [ i ], itsNavmeshAgent );
						}
					}
				}

				if ( hasAnimator )
				{
					if ( navmeshAgentByAnimator.ContainsKey ( itsAnimator ) == false )
					{
						navmeshAgentByAnimator.Add ( itsAnimator, itsNavmeshAgent );
					}
				}

				if ( navmeshAgentByGameobject.ContainsKey ( itsGameobject ) == false )
				{
					navmeshAgentByGameobject.Add ( itsGameobject, itsNavmeshAgent );
				}

				if ( navmeshAgentByTransform.ContainsKey ( itsTransform ) == false )
				{
					navmeshAgentByTransform.Add ( itsTransform, itsNavmeshAgent );
				}
			}

			//Animators
			if ( animatorByRigidbody == null )
			{
				animatorByRigidbody = new Dictionary<Rigidbody, Animator> ( );
			}

			if ( animatorByCollider == null )
			{
				animatorByCollider = new Dictionary<Collider, Animator> ( );
			}

			if ( animatorByTrigger == null )
			{
				animatorByTrigger = new Dictionary<Collider, Animator> ( );
			}

			if ( animatorByNavmeshAgent == null )
			{
				animatorByNavmeshAgent = new Dictionary<NavMeshAgent, Animator> ( );
			}

			if ( animatorByGameobject == null )
			{
				animatorByGameobject = new Dictionary<GameObject, Animator> ( );
			}

			if ( animatorByTransform == null )
			{
				animatorByTransform = new Dictionary<Transform, Animator> ( );
			}

			if ( hasAnimator )
			{
				if ( hasRigidbody )
				{
					if ( animatorByRigidbody.ContainsKey ( itsRigidbody ) == false )
					{
						animatorByRigidbody.Add ( itsRigidbody, itsAnimator );
					}
				}

				if ( hasColliders )
				{
					for ( int i = 0; i < amountOfColliders; i++ )
					{
						if ( animatorByCollider.ContainsKey ( itsColliders [ i ] ) == false )
						{
							animatorByCollider.Add ( itsColliders [ i ], itsAnimator );
						}
					}
				}

				if ( hasTriggers )
				{
					for ( int i = 0; i < amountOfTriggers; i++ )
					{
						if ( animatorByTrigger.ContainsKey ( itsTriggers [ i ] ) == false )
						{
							animatorByTrigger.Add ( itsTriggers [ i ], itsAnimator );
						}
					}
				}

				if ( hasNavmeshAgent )
				{
					if ( animatorByNavmeshAgent.ContainsKey ( itsNavmeshAgent ) == false )
					{
						animatorByNavmeshAgent.Add ( itsNavmeshAgent, itsAnimator );
					}
				}

				if ( animatorByGameobject.ContainsKey ( itsGameobject ) == false )
				{
					animatorByGameobject.Add ( itsGameobject, itsAnimator );
				}

				if ( animatorByTransform.ContainsKey ( itsTransform ) == false )
				{
					animatorByTransform.Add ( itsTransform, itsAnimator );
				}
			}

			_initialized = true;
		}


		public enum ResetCallType
		{
			OnEnable,
			OnDisable,
			OnDestroy,
		}

		protected virtual void ResetBehaviour ( ResetCallType callType )
		{
			switch ( callType )
			{
				case ResetCallType.OnEnable:
					ResetOnEnable ( );
					break;
				case ResetCallType.OnDisable:
					ResetOnDisable ( );
					break;
				case ResetCallType.OnDestroy:
					ResetOnDestroy ( );
					break;
				default:
					break;
			}
		}

		protected virtual void ResetOnEnable ( ) { }
		protected virtual void ResetOnDisable ( ) { }
		protected virtual void ResetOnDestroy ( ) { }

		//Caching Methods
		protected virtual void CacheLocalPosition ( ) { itsLocalPosition = itsTransform.localPosition; }
		protected virtual void CacheForward ( ) { itsForward = itsTransform.forward; }

		[HideInInspector] public Vector3 previousLocalPosition;
		[HideInInspector] public float deltaDistance;
		[HideInInspector] public Vector3 velocity;
		[HideInInspector] public float velocityMagnitude;
		[HideInInspector] public Vector3 velocityNormalized;
		/// <summary>
		/// Equivalent to CacheLocalPosition
		/// </summary>
		protected virtual void CacheVelocity ( )
		{
			itsLocalPosition = itsTransform.localPosition;
			velocity.x = itsLocalPosition.x - previousLocalPosition.x;
			velocity.y = itsLocalPosition.y - previousLocalPosition.y;
			velocity.z = itsLocalPosition.z - previousLocalPosition.z;

			velocityMagnitude = velocity.x * velocity.x;
			velocityMagnitude += velocity.y * velocity.y;
			velocityMagnitude += velocity.z * velocity.z;

			deltaDistance = Mathf.Sqrt ( velocityMagnitude );

			velocityNormalized.x /= deltaDistance;
			velocityNormalized.y /= deltaDistance;
			velocityNormalized.z /= deltaDistance;

			previousLocalPosition = itsLocalPosition;
		}

		protected virtual void CacheLocalRotation ( ) { itsLocalRotation = itsTransform.localRotation; }

		public interface ISpawnable
		{
			bool spawnable { get; }
			void Spawn ( Vector3 spawnPosition, Quaternion spawnRotation, bool toggleOn, Transform newParent );
		}

		public class PoolItem<T> : EnhancedBehaviour, ISpawnable where T : MonoBehaviour
		{
			private float previousSpawn;

			private static readonly List<PoolItem<T>> _instances = new List<PoolItem<T>> ( 90 );
			public static List<PoolItem<T>> instances => _instances;

			private static bool enoughMinimumAmountOfInstances => instances != null && instances.Count > 1;

			public virtual bool spawnable => true;

			[SerializeField] private readonly UltEvent onSpawn;
			public delegate void PoolItemDelegate ( PoolItem<T> triggeringItem );
			public static event PoolItemDelegate anyPoolItemSpawned;

			private void Awake ( )
			{
				instances.AddIfNotContains ( this );
			}

			private void OnDestroy ( )
			{
				instances.RemoveIfContains ( this );
			}

			public static PoolItem<T> FirstSpawnableInstance ( )
			{
				if ( enoughMinimumAmountOfInstances == false )
				{
					return null;
				}

				return instances.Where ( _instance
					  => _instance.spawnable ).ToArray ( ) [ 0 ];
			}

			public static PoolItem<T> FirstDisabledInstance ( )
			{
				if ( enoughMinimumAmountOfInstances == false )
				{
					return null;
				}

				PoolItem<T> eligibleInstance = instances.Where ( _instance
					  => _instance.gameObject.IsActive ( ) == false && _instance.spawnable )
					.ToArray ( ) [ 0 ];

				return eligibleInstance;
			}

			public static PoolItem<T> OldestSpawnedInstance ( )
			{
				if ( enoughMinimumAmountOfInstances == false )
				{
					return null;
				}

				IEnumerable<PoolItem<T>> spawnables = instances.Where ( _instance => _instance.spawnable );
				var amountOfEligibleInstances = spawnables.Count ( );

				if ( amountOfEligibleInstances > 2 )
				{
					var oldestPreviousSpawn = spawnables.Min ( _instance => _instance.previousSpawn );
					PoolItem<T> eligibleInstance = spawnables.Where ( _instance
						  => Mathf.Approximately ( _instance.previousSpawn, oldestPreviousSpawn ) )
						.ToArray ( ) [ 0 ];

					return eligibleInstance;
				}
				else
				{
					return ( amountOfEligibleInstances > 0 )
						? spawnables.ToArray ( ) [ 0 ] : null;
				}
			}

			public static PoolItem<T> OldestDisabledInstance ( )
			{
				if ( enoughMinimumAmountOfInstances == false )
				{
					return null;
				}

				IEnumerable<PoolItem<T>> disabledInstances = instances.Where ( _instance
					  => _instance.gameObject.IsActive ( ) == false
					  && _instance.spawnable );

				var amountOfEligibleInstances = disabledInstances.Count ( );
				if ( amountOfEligibleInstances > 2 )
				{
					var oldestPreviousSpawn = disabledInstances.Min ( _instance => _instance.previousSpawn );
					PoolItem<T> eligibleInstance = disabledInstances.Where ( _instance
						  => Mathf.Approximately ( _instance.previousSpawn, oldestPreviousSpawn ) )
						.ToArray ( ) [ 0 ];

					return eligibleInstance;
				}
				else
				{
					return ( amountOfEligibleInstances > 0 )
						? disabledInstances.ToArray ( ) [ 0 ] : null;
				}
			}

			public void Spawn ( Vector3 spawnPosition, Quaternion spawnRotation, bool toggleOn = false, Transform newParent = null )
			{
				BeforebeingSpawned ( );

				itsTransform.SetPositionAndRotation ( spawnPosition, spawnRotation );
				if ( newParent != null )
				{
					itsTransform.SetParent ( newParent );
				}

				if ( toggleOn )
				{
					gameObject.SetActive ( true );
				}

				previousSpawn = time;
				anyPoolItemSpawned?.Invoke ( this );

				AfterBeingSpawned ( );
			}

			public static PoolItem<T> SpawnFirstDisabled ( Vector3 spawnPosition, Quaternion spawnRotation, bool toggleOn = true, Transform newParent = null )
			{
				PoolItem<T> eligibleInstance = FirstDisabledInstance ( );
				if ( eligibleInstance == null )
				{
					return null;
				}

				eligibleInstance.Spawn ( spawnPosition, spawnRotation, toggleOn, newParent );
				return eligibleInstance;
			}

			public static PoolItem<T> SpawnOldest ( Vector3 spawnPosition, Quaternion spawnRotation, bool toggleOn = true, Transform newParent = null )
			{
				PoolItem<T> eligibleInstance = OldestSpawnedInstance ( );
				if ( eligibleInstance == null )
				{
					return null;
				}

				eligibleInstance.Spawn ( spawnPosition, spawnRotation, toggleOn, newParent );
				return eligibleInstance;
			}

			public static PoolItem<T> SpawnOldestDisabled ( Vector3 spawnPosition, Quaternion spawnRotation, bool toggleOn = true, Transform newParent = null )
			{
				PoolItem<T> eligibleInstance = OldestDisabledInstance ( );
				if ( eligibleInstance == null )
				{
					return null;
				}

				eligibleInstance.Spawn ( spawnPosition, spawnRotation, toggleOn, newParent );
				return eligibleInstance;
			}

			public static PoolItem<T> SpawnFirstSpawnable ( Vector3 spawnPosition, Quaternion spawnRotation, bool toggleOn = true, Transform newParent = null )
			{
				PoolItem<T> eligibleInstance = FirstSpawnableInstance ( );
				if ( eligibleInstance == null )
				{
					return null;
				}

				eligibleInstance.Spawn ( spawnPosition, spawnRotation, toggleOn, newParent );
				return eligibleInstance;
			}

			protected virtual void BeforebeingSpawned ( ) { }
			protected virtual void AfterBeingSpawned ( ) { }
		}
	}
}
