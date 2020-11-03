using UnityEngine;
using System.Collections.Generic;
using System;

public static partial class Extensions
{
	public static bool AddIfNotContains<T> ( this IList<T> list, T item, bool allowNullEntries = false )
	{
		if ( allowNullEntries == false && item == null )
		{
#if UNITY_EDITOR
			Debug.Log ( list.ToString ( ) + ", not allowed to add null entries by default!" );
#endif
			return false;
		}

		if ( list == null )
		{
			list = new List<T> ( );
#if UNITY_EDITOR
			Debug.Log ( list.ToString ( ) + ", list had to be created" );
#endif
		}

		if ( list.Contains ( item ) )
		{
#if UNITY_EDITOR
			Debug.Log ( item.ToString() + ", already on the list");
#endif
			return false;
		}

		var isAnArrayWithFixedSize
			= list is Array && ( list as Array ).IsFixedSize;

		var size = list.Count;
		var nullElementReplaced = false;

		for ( int i = 0; i < size; i++ )
		{
			if ( list [ i ] == null )
			{
				list [ i ] = item;

#if UNITY_EDITOR
				Debug.Log ( item + ", replaced index " + i + " with null value from the list" );
#endif
				nullElementReplaced = true;
				break;
			}
		}

		if ( nullElementReplaced ) return true;

		if ( isAnArrayWithFixedSize )
		{
#if UNITY_EDITOR
			Debug.Log ( "Overriding last item due to fixed size array" );
#endif
			list [ size - 1 ] = item;
			return true;
		}
		else
		{
			list.Add ( item );
		}

		return true;
	}

	public static bool RemoveIfContains<T> (this IList<T> list, T item)
	{
		if ( item == null )
		{
#if UNITY_EDITOR
			Debug.Log ( list.ToString ( ) + ", not allowed to remove null entries by default!" );
#endif
			return false;
		}

		if ( list == null )
		{
#if UNITY_EDITOR
			Debug.Log ("list not initialized, there's nothing to remove" );
#endif
			return false;
		}

		if ( list.Contains ( item ) )
		{
#if UNITY_EDITOR
			Debug.Log ( item.ToString ( ) + ", removed from the list" );
#endif
			return true;
		}
		return false;
	}

	public static int Remove<T> (this IList<T> thisList, IList<T> toRemove )
	{
		var amountOfItemsToRemove = toRemove.Count;
		var amountRemoved = 0;

		for ( int i = 0; i < amountOfItemsToRemove; i++ )
		{
			var itemToRemove = toRemove [ i ];

			if ( thisList.RemoveIfContains ( itemToRemove ) )
				amountRemoved++;
		}

		return amountRemoved;
	}

	public static int Add<T> ( this IList<T> thisList, IList<T> toAdd)
	{
		var amountOfItemsToAdd = toAdd.Count;
		var amountAdded = 0;

		for ( int i = 0; i < amountOfItemsToAdd; i++ )
		{
			var itemToRemove = toAdd [ i ];

			if ( thisList.AddIfNotContains ( itemToRemove ) )
				amountAdded++;
		}

		return amountAdded;
	}
}