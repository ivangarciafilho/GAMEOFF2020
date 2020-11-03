using UnityEngine;
using UltEvents;

public class BatchOfEvents : MonoBehaviour, IPackedEvents
{
	public bool disableUponMaximumExecution = true;
	[Range ( 0f, 1f )] public float triggeringChance = 1f;

	public int maximumAmountOfTimesTriggered = 0;
	protected int amountOfTimesTriggered;

	public float delayBetweenTriggers = 0;
	protected float nextTriggeringAvailability;

	public UltEvent onMaximumExecution;

	protected virtual bool customCondition	{ get { return true; } }
	public bool allowedToTrigger
	{
		get
		{
			var currrentTime = 0f;

			bool withinTimeFrame ( )
			{
				if ( delayBetweenTriggers != 0 )
				{
					currrentTime = Time.time;
					return currrentTime  > nextTriggeringAvailability;
				}
				else
				{
					return true;
				}
			}

			bool hasTriggeringTimesRemaining()
			{
				if ( maximumAmountOfTimesTriggered != 0 )
				{
					return (amountOfTimesTriggered < maximumAmountOfTimesTriggered);
				}
				else
				{
					return true;
				}
			}

			if ( withinTimeFrame() && hasTriggeringTimesRemaining ( ) && customCondition )
			{
				nextTriggeringAvailability = currrentTime + delayBetweenTriggers;

				if ( ++amountOfTimesTriggered >= maximumAmountOfTimesTriggered )
				{
					onMaximumExecution.Invoke ( );
					if ( disableUponMaximumExecution ) enabled = false;
				}

				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public  virtual void FireEvents ( bool forceTriggering )
	{
		throw new System.NotImplementedException ( );
	}

	public virtual void Reset ( )
	{
		ResetDelay ( );
		ResetTriggeringCounts ( );
	}

	public  virtual void ResetDelay ( ) => nextTriggeringAvailability = 0f;
	public virtual void ResetTriggeringCounts ( ) => amountOfTimesTriggered = 0;
}
