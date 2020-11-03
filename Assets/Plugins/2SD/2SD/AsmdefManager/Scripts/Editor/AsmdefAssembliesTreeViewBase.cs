using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Linq;
using System.Text.RegularExpressions;

namespace TSD.AsmdefManagement
{
	public class AsmdefAssembliesTreeViewBase : TreeView
	{

		public delegate void ToggleChanged(string id);
		
		public System.Action<AsmdefIncludeAssembliesTreeViewItemBase> OnIsMarked;
		public System.Action<AsmdefIncludeAssembliesTreeViewItemBase> OnIsEditorMarked;
		public System.Action<AsmdefIncludeAssembliesTreeViewItemBase> OnIsRuntimeMarked;

		GUIContent[] buttons = new GUIContent[3] { new GUIContent("Both"), new GUIContent("Runtime"), new GUIContent("Editor") };

		List<TreeViewItem> tree;

		List<string> treeElements;
		
		//public TreeView(IMGUI.Controls.TreeViewState state);
		public AsmdefAssembliesTreeViewBase(TreeViewState state, List<string> elements = null) : base(state)
		{
			treeElements = elements;
			Reload();
		}
		protected override TreeViewItem BuildRoot()
		{

			var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
			tree = new List<TreeViewItem>();

			//AsmdefManagerDatabase.Instance.alwaysIncludeAssemblies; compare treeElement to this(so we can display the checkbox properly)
			var distinctList = treeElements;
			distinctList = treeElements.Where(item => !AsmdefManagerDatabase.Instance.createdAsmdefFiles.Contains(item)).ToList();

			//loop through each string, every time there is a dot add another element to the list before the currently checked element
			//Unity.Textmeshpro.Editor would become Unity, Unity.TextMeshPro, Unity.Textmeshpro.Editor. Only the original will have a 
			//checkbox beside it.
			List<string> updatedList = new List<string>();

			foreach (var item in distinctList)
			{
				string[] myArr = Regex.Split(item, "\\.").Where(srt=> srt != "").ToArray();
				foreach (var arrItem in myArr)
				{
					if (!updatedList.Contains(item))
					{
						updatedList.Add(item);
					}
				}
				updatedList.Add(item);
			}

			int curId = 1;
			foreach (var item in distinctList)
			{
				string m_fullPath = item;
				var str = item.Contains('.') ? item.Substring(item.LastIndexOf('.')) : item;
				int d = m_fullPath.Count(f => f == '.');
				//	if(d == 2) { d = 1; }

				var asmItem = new AsmdefIncludeAssembliesTreeViewItemBase {
					id = curId,
					depth = 1,
					displayName = m_fullPath,
					markerExists = distinctList.Contains(item),
					isMarked = AsmdefManagerDatabase.Instance.alwaysIncludeAssemblies.Contains(m_fullPath),
					isRuntimeMarked = AsmdefManagerDatabase.Instance.alwaysIncludeRuntimeAssemblies.Contains(m_fullPath),
					isEditorMarked = AsmdefManagerDatabase.Instance.alwaysIncludeEditorAssemblies.Contains(m_fullPath)
				};
				tree.Add(asmItem);
				curId++;
			}
			//Root(Assets) should be enabled by default
		//	((AsmdefIncludeAssembliesTreeViewItemBase)tree[0]).isMarked = true;

			// Utility method that initializes the TreeViewItem.children and .parent for all items.
			SetupParentsAndChildrenFromDepths(root, tree);

			// Return root of the tree
			return root;
		}

		// Custom GUI
		protected override void RowGUI(RowGUIArgs args)
		{
			var item = ((AsmdefIncludeAssembliesTreeViewItemBase)args.item);
			extraSpaceBeforeIconAndLabel = 16 * 3 + 2;

			//first we offset by indentation, then add an offset to show the
			//arrow\triangle\whatever on the beginning
			if(item.markerExists)
			{
				Rect toggleRect = args.rowRect;
				toggleRect.x += GetContentIndent((args.item));
				toggleRect.y -= 2f;
				
				toggleRect.width = GetCustomRowHeight(args.row, item)-2f;
				toggleRect.height = GetCustomRowHeight(args.row, item) + 2f;

				var lastVal = item.isMarked;
				var lastRuntime = item.isRuntimeMarked;
				var lastEditor = item.isEditorMarked;
				
				item.isMarked = EditorGUI.Toggle(toggleRect, item.isMarked, AsmdefManagerEditorWindow.loadedCheckbox);
				toggleRect.x += toggleRect.width +1f;
				item.isRuntimeMarked = EditorGUI.Toggle(toggleRect, item.isRuntimeMarked, AsmdefManagerEditorWindow.loadedCheckbox);
				toggleRect.x += toggleRect.width +1f;
				item.isEditorMarked = EditorGUI.Toggle(toggleRect, item.isEditorMarked, AsmdefManagerEditorWindow.loadedCheckbox);
				if (lastVal != item.isMarked)
				{
					item.isRuntimeMarked = item.isMarked;
					item.isEditorMarked = item.isMarked;
					if (OnIsMarked != null) { OnIsMarked.Invoke(item); }
					if (Event.current != null && Event.current.alt)
					{
						markChildrenRecursively(item.isMarked, item);
					}
				}
				else if(lastRuntime != item.isRuntimeMarked)
				{
					if (OnIsRuntimeMarked != null) { OnIsRuntimeMarked.Invoke(item); }
				}
				else if (lastEditor != item.isEditorMarked)
				{
					if (OnIsEditorMarked != null) { OnIsEditorMarked.Invoke(item); }
				}

				item.isMarked = item.isRuntimeMarked && item.isEditorMarked;
			}
			base.RowGUI(args);
		}

		void markChildrenRecursively(bool mark, AsmdefIncludeAssembliesTreeViewItemBase item)
		{
			if (item.isMarked != mark)
			{
				item.isMarked = mark;
				if (OnIsMarked != null) { OnIsMarked.Invoke(item); }
			}
			else
			{
				item.isMarked = mark;
			}
			if (!item.hasChildren) { return; }
			foreach (AsmdefIncludeAssembliesTreeViewItemBase child in item.children)
			{
				markChildrenRecursively(mark, child);
			}
		}

		public void SetStateAll(bool newState)
		{
			foreach (var item in tree)
			{
				AsmdefIncludeAssembliesTreeViewItemBase aItem = (AsmdefIncludeAssembliesTreeViewItemBase)item;

				//means we will either add or remove an element
				bool willChange = aItem.isEditorMarked != newState || aItem.isRuntimeMarked != newState;
				if (aItem.isEditorMarked != newState)
				{
					aItem.isEditorMarked = newState;
					OnIsEditorMarked?.Invoke(aItem);
				}
				if (aItem.isRuntimeMarked != newState)
				{
					aItem.isRuntimeMarked = newState;
					OnIsRuntimeMarked?.Invoke(aItem);
				}
				if(willChange)
					OnIsMarked?.Invoke(aItem);
			}
			
			AsmdefManagerDatabase.Instance.SaveDatabase();
		}

		public void ToggleState(bool isRuntime)
		{
			AsmdefIncludeAssembliesTreeViewItemBase initialItem = (AsmdefIncludeAssembliesTreeViewItemBase) tree[0];
			bool initialValue = isRuntime ? initialItem.isRuntimeMarked : initialItem.isEditorMarked;
			initialValue = !initialValue;

			bool runtimeChanged = false;
			bool editorChanged = false;
			foreach (var item in tree)
			{
				AsmdefIncludeAssembliesTreeViewItemBase aItem = (AsmdefIncludeAssembliesTreeViewItemBase)item;

				//means we will either add or remove an element
				bool willChange = isRuntime? aItem.isRuntimeMarked != initialValue : aItem.isEditorMarked != initialValue;
				if (isRuntime)
				{
					if (aItem.isRuntimeMarked != initialValue)
					{
						aItem.isRuntimeMarked = initialValue;
						OnIsRuntimeMarked?.Invoke(aItem);
					}
				}
				else
				{
					if (aItem.isEditorMarked != initialValue)
					{
						aItem.isEditorMarked = initialValue;
						OnIsEditorMarked?.Invoke(aItem);
					}
				}

				if(willChange)
					OnIsMarked?.Invoke(aItem);
			}
			
			AsmdefManagerDatabase.Instance.SaveDatabase();
		}
	}

	public class AsmdefTreeViewItemBase : TreeViewItem
	{
		public bool markerExists { get; set; } = true;
		/// <summary>
		/// Was its toggle ticked?
		/// </summary>
		public bool isMarked { get; set; } //not called selected because it would be confusing as TreeViewItem already have a selected member
		public string fullPath { get; set; }
		public string relativePath { get; set; }
	}

	public class AsmdefIncludeAssembliesTreeViewItemBase : AsmdefTreeViewItemBase
	{
		public bool isEditorMarked { get; set; }
		public bool isRuntimeMarked { get; set; }
	}
}