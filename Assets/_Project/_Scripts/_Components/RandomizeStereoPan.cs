using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeStereoPan : AudioComponent, IRandomizable
{
	public Vector2 stereoPanRange = new Vector2 ( -0.333f, 0.333f );

	public void Randomize ( )
	{
		itsAudioSource.panStereo = Random.Range ( stereoPanRange.x, stereoPanRange.y);
	}
}
