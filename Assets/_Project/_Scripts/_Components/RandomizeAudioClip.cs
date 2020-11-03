using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bloodstone;
using UnityEngine;

public class RandomizeAudioClip : AudioComponent, IRandomizable
{
    public AudioClip[] variants;

    public void Randomize ( )
    {
        variants = variants.Shuffle ( ).ToArray();
        itsAudioSource.clip = variants [ Random.Range ( 0, variants.Length ) ];
    }
}
