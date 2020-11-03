using Bloodstone;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SetPostEffectVolumeWeight : FloatSetter
{
	public PostProcessVolume postProcessVolume;

	public override void Set ( float scale )
	{
		if ( postProcessVolume != null )
		{
			scale = Mathf.Clamp01 ( scale );
			postProcessVolume.weight = scale;
		}
	}


#if UNITY_EDITOR
	private void OnValidate ( )
	{
		if ( postProcessVolume == null )
			postProcessVolume = GetComponent<PostProcessVolume> ( );
	}
#endif
}
