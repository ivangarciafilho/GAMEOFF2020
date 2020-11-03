using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static partial class Extensions
{
	public static void SetEnabled(this IEnumerable<Component> components, bool enabled = true)
	{
		var componentsAsList = components as List<Behaviour>;
		var amountOfBehaviours = componentsAsList.Count();

		Behaviour currentBehavior = null;
		for (int i = 0; i < amountOfBehaviours; i++)
		{
			currentBehavior = componentsAsList[ i ];

			if (currentBehavior != null)
			{

				if ( enabled == false && currentBehavior is MonoBehaviour)
					( currentBehavior as MonoBehaviour ).StopAllCoroutines ( );

				currentBehavior.enabled = enabled;
			}      
		}
	}
}