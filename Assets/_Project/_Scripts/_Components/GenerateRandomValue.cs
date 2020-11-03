using UnityEngine;
using Bloodstone;
using Random = UnityEngine.Random;


public class GenerateRandomValue : MonoBehaviour
{
	public Vector2 randomValue;

	[ReadOnly, SerializeField] private float _generatedValue;
	public float generatedValue => _generatedValue; 

	public FloatValueEvent onGenerate;

	public void Generate ( )
	{
		_generatedValue = Random.Range ( randomValue.x, randomValue.y);
		if ( onGenerate.HasCalls ) onGenerate.Invoke ( _generatedValue );
	}
}
