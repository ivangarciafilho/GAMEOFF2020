using UnityEngine;

public class RandomizePitch : AudioComponent, IRandomizable
{
    public Vector2 pitchRange = new Vector2(0.9f,1.1f);

    public void Randomize ( )
    {
        itsAudioSource.pitch = Random.Range (pitchRange.x,pitchRange.y);
    }
}
