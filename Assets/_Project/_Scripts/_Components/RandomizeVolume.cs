using UnityEngine;

public class RandomizeVolume : AudioComponent, IRandomizable
{
	public Vector2 volumeRange = new Vector2 ( 0.8f, 1f);

	public void Randomize ( )
	{
		itsAudioSource.volume = Random.Range ( volumeRange.x, volumeRange.y );
	}
}
