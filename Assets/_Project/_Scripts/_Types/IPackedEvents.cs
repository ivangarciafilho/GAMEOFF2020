public interface IPackedEvents
{
	bool allowedToTrigger { get; }
	void FireEvents (bool forceTriggering);
}
