public interface IConstantDispatcher
{
	Timeframe schedule { get; }
	IConstantReceiver[] receivers { get; }
}
