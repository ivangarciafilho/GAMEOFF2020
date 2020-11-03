using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public static partial class Extensions
{
	public static float Interpolate(this Vector2 range, float step)
	{
		return Mathf.Lerp(range.x, range.y, step);
	}

	public static int Interpolate(this Vector2Int range, float step)
	{
		return Mathf.RoundToInt(Mathf.Lerp(range.x, range.y, step));
	}

	public static float InterpolateFromZero ( this float max, float step )
	{
		return Mathf.Lerp ( 0, max, step );
	}

	public static int InterpolateFromZero ( this int max, float step )
	{
		return Mathf.RoundToInt ( Mathf.Lerp ( 0, max, step ) );
	}

	public static float InterpolateFromInverseValue ( this float range, float step )
	{
		return Mathf.Lerp ( -range, range, step );
	}

	public static int InterpolateFromInverseValue ( this int range, float step )
	{
		return Mathf.RoundToInt ( Mathf.Lerp ( -range, range, step ) );
	}

	public static int ToHash ( this string name)
	{
		return Animator.StringToHash ( name );
	}


	public enum SmoothType { none, smoothstep, smootherstep, exponential, easeIn, easeOut };

	public static float Smooth01 ( this float floatValue, SmoothType smoothType )

	{
		floatValue = Mathf.Clamp01 ( floatValue );
		switch ( smoothType )
		{
			case SmoothType.none:
				break;

			case SmoothType.smoothstep:
				floatValue = floatValue * floatValue * ( 3f - 2f * floatValue );
				break;

			case SmoothType.smootherstep:
				floatValue = floatValue * floatValue * floatValue * ( floatValue * ( 6f * floatValue - 15f ) + 10f );
				break;

			case SmoothType.exponential:
				floatValue = floatValue * floatValue;
				break;

			case SmoothType.easeIn:
				floatValue = 1f - Mathf.Cos ( floatValue * Mathf.PI * 0.5f );
				break;

			case SmoothType.easeOut:
				floatValue = Mathf.Sin ( floatValue * Mathf.PI * 0.5f );
				break;
		}
		return floatValue;
	}

	public static float Remap ( this float value, float lowEndOfRange1, float highEndOfRange1, float lowEndOfRange2, float highEndOfRange2 )
	{
		return ( value - lowEndOfRange1 ) / ( highEndOfRange1 - lowEndOfRange1 ) * ( highEndOfRange2 - lowEndOfRange2 ) + lowEndOfRange2;
	}

	unsafe public static float FastInvSqrt ( this float number )
	{
		var i = *( int* ) &number;
		i = 0x5f3759df - ( i >> 1 );
		var y = *( float* ) &i;
		return y * ( 1.5F - 0.5F * number * y * y );
	}

	/// <summary>
	/// Takes the inverse square root of x using Newton-Raphson
	/// approximation with 1 pass after clever inital guess using
	/// bitshifting.
	/// See http://betterexplained.com/articles/understanding-quakes-fast-inverse-square-root/ for more information.
	/// </summary>
	/// <param name="number">The value.</param>
	/// <param name="iterations">The number of iterations to make. Higher means more precision.</param>
	/// <returns>The inverse square root of x</returns>
	/// 
	public static float FastInvSqrt ( this float number, int iterations = 0 )
	{
		var convert = new Convert { x = number };

		var xhalf = 0.5f * number;

		convert.i = 0x5f3759df - ( convert.i >> 1 );

		number = convert.x;

		number = number * ( 1.5f - xhalf * number * number );

		for ( int i = 0; i < iterations; i++ )
			number *= ( 1.5f - xhalf * number * number );

		return number;
	}

	[StructLayout ( LayoutKind.Explicit )]
	private struct Convert
	{
		[FieldOffset ( 0 )]
		public float x;

		[FieldOffset ( 0 )]
		public int i;
	}
}

