using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using NaughtyAttributes;

public class MenuButtonsSelection : MonoBehaviour
{
	[ReorderableList]
	public Button[] buttons;
	int selectedIndex = 0;

	public void Set(GameObject obj)
	{
		var itemHover = Array.FindIndex(buttons, it => it.gameObject == obj);
		if (itemHover != -1)
		{
			selectedIndex = itemHover;
		}
	}

	private void Update()
	{
		EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);

		var dir = 0;
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) dir--;
		else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) dir++;

		void NextItem()
		{
			selectedIndex += dir;
			if (selectedIndex >= buttons.Length) selectedIndex = 0;
			if (selectedIndex < 0) selectedIndex = buttons.Length - 1;
		}

		NextItem();

		var selectedButton = buttons[selectedIndex];
		if (!selectedButton.gameObject.activeInHierarchy)
		{
			if (dir == 0) dir = 1;
			NextItem();
		}

		for (int i = 0; i < buttons.Length; i++)
		{
			var button = buttons[i];
			button.transform.localScale = Vector3.Lerp(button.transform.localScale, Vector3.one, Time.unscaledDeltaTime * 3.0f);
		}
		
		 //if (Input.GetKeyDown(KeyCode.Return))
		 //{
		 //	selectedButton.onClick.Invoke();
		 //}

		selectedButton.transform.localScale = Vector3.Lerp(selectedButton.transform.localScale, Vector3.one * 1.2f, Time.unscaledDeltaTime * 3.0f);

	}

	private void OnDisable()
	{
		selectedIndex = 0;
		for (int i = 0; i < buttons.Length; i++)
		{
			var button = buttons[i];
			button.transform.localScale = Vector3.one;
		}
	}
}
