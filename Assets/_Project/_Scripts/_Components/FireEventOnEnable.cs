using UltEvents;

public class FireEventOnEnable : BatchOfEvents
{
    public UltEvent onEnable;

    public void OnEnable ( ) 
        => FireEvents ( false ); 

    public override void FireEvents ( bool forceTriggering )
    {
        if ( !forceTriggering && !allowedToTrigger) return;

        if ( onEnable.HasCalls ) onEnable.Invoke ( );
    }
}
