using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScale : MonoBehaviour
{
    public Vector2 scaleRange;

    private void OnEnable()
    {
        transform.localScale = Vector3.one*Random.Range(scaleRange.x, scaleRange.y);
    }
}
