public interface IScannerHandler
{
	ScannerController scannerProfile { get; }
	void OnEntityScanned ( Scanner scanner, PairSpacialRelationship triggeringEntity);
}
