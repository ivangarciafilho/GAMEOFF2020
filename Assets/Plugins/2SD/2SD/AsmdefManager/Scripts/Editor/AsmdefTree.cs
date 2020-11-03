using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.IO;
using System.Linq;

namespace TSD.AsmdefManagement
{
	public class AsmdefTree : TreeView
	{

		public delegate void ToggleChanged(AsmdefTreeViewItem id);
		public ToggleChanged OnToggleChanged { get; set; }
		public Action OnAllToggleChanged { get; set; }

		public delegate void ItemCreated(AsmdefTreeViewItem item);
		public ItemCreated OnItemCreated { get; set; } //ehh, horrible name. It isn't called when it was created but when we loop through the elements. FIX

		List<TreeViewItem> tree;

		Color originalTextColor;

		//public TreeView(IMGUI.Controls.TreeViewState state);
		public AsmdefTree(TreeViewState state) : base(state)
		{
			useScrollView = true;
			Reload();
		}
		protected override TreeViewItem BuildRoot()
		{
			//cache original GUI color so we can revert to it later
			originalTextColor = GUI.contentColor;

			var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
			tree = new List<TreeViewItem>();
			
			int curId = 1;
			ProcessDirectory(Application.dataPath, ref tree, ref curId);

			//Root(Assets) should be enabled by default
			((AsmdefTreeViewItem)tree[0]).isMarked = false;
			
			// Utility method that initializes the TreeViewItem.children and .parent for all items.
			SetupParentsAndChildrenFromDepths(root, tree);
				
			// Return root of the tree
			return root;
		}

		//get each folder in directory
		//iterate through each folder and it to the tree
		//go back to step one
		//if there is no more subdirectory return

		// Process all files in the directory passed in, recurse on any directories 
		// that are found, and process the files they contain.
		public static void ProcessDirectory(string targetDirectory, ref List<TreeViewItem> refTree, ref int _id)
		{
			targetDirectory = targetDirectory.Replace('\\', '/');
			string m_fullPath = targetDirectory.Substring(Application.dataPath.Length - 6).Replace('\\', '/'); //-7 to keep the Assets part
			var str = targetDirectory.Contains('/') ? targetDirectory.Substring(targetDirectory.LastIndexOf('/') + 1) : targetDirectory;
			int d = m_fullPath.Count(f => f == '/');

			var temp = Directory.GetFiles(targetDirectory, "*.cs", SearchOption.TopDirectoryOnly);
			var asmItem = new AsmdefTreeViewItem
			{
				id = _id,
				depth = d,
				displayName = str,
				fullPath = m_fullPath,
				relativePath = str,
				absoluteFilePath = targetDirectory,
				isMarked = !AsmdefManagerDatabase.Instance.isPathExcluded("/" + m_fullPath + "/"),
				isSpecialFolder = temp.Length != 0 && !AsmdefUtils.IsPartOfBaseAssembly(temp[0].Replace('\\', '/'), AsmdefUtils.CompilationFolder.Any, false)
				//	asmdefPath = AsmdefManagerEditorWindow.Instance.dCombinedPass.ContainsKey(item) ? AsmdefManagerEditorWindow.Instance.dCombinedPass[item].Substring(AsmdefManagerEditorWindow.Instance.dCombinedPass[item].LastIndexOf("Assets") + 7) : ""
			};

			AsmdefManagerDatabase.Instance.AddDirectory(targetDirectory);
			refTree.Add(asmItem);
			_id++;

			// Recurse into subdirectories of this directory.
			string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory, "*", SearchOption.TopDirectoryOnly);
			foreach (string subdirectory in subdirectoryEntries)
			{
				if (subdirectory.EndsWith(".git")) { continue; } //skip .git and its subfolders
				ProcessDirectory(subdirectory, ref refTree, ref _id);
			}
		}

		public void LoopThroughCreatedItems()
		{
		//	Debug.LogWarning("Keep this in check");
		}

		public void SetAsmdefFolder()
		{
			var combinedPasses = AsmdefManagerEditorWindow.Instance.dCombinedPass;
			foreach (AsmdefTreeViewItem item in tree)
			{
				item.asmdefPath = combinedPasses.ContainsKey(item.absoluteFilePath) ?
					combinedPasses[item.absoluteFilePath].Substring(combinedPasses[item.absoluteFilePath].LastIndexOf("Assets") + 7) : "";
			}
		}

		// Custom GUI
		protected override void RowGUI(RowGUIArgs args)
		{
			AsmdefTreeViewItem item = ((AsmdefTreeViewItem)args.item);
			//	Event evt = Event.current;
			extraSpaceBeforeIconAndLabel = GetCustomRowHeight(args.row, item);
			
			//first we offset by indentation, then add an offset to show the
			//arrow\triangle\whatever on the beginning
			if(item.isConflicting)
			{
				AsmdefManagerEditorWindow.Instance.DrawColoredRectangle(args.rowRect, AsmdefManagerEditorWindow.colorIssue, args.item.depth * 14 + 32);
			}
			if(item.isSpecialFolder)
			{
				AsmdefManagerEditorWindow.Instance.DrawColoredRectangle(args.rowRect, AsmdefManagerEditorWindow.colorInManualAssembly, args.item.depth * 14 +16 );
			}

			//offset the togglerect(this has been precisely tweeked for this specific toggle texture, hence the weird -1, -2)
			Rect toggleRect = args.rowRect;
			toggleRect.x -= 1f;
			toggleRect.x += GetContentIndent((args.item));
			toggleRect.y -= 2f;
			toggleRect.width = GetCustomRowHeight(args.row, item);
			toggleRect.height = GetCustomRowHeight(args.row, item);

			//	EditorGUI.BeginDisabledGroup( !isParentEnabledRecursive(item));

			bool lastVal = item.isMarked;
			item.isMarked = EditorGUI.Toggle(toggleRect, item.isMarked, AsmdefManagerEditorWindow.loadedCheckbox);
			
			if (lastVal != item.isMarked)
			{
				if (OnToggleChanged != null)
				{
					OnToggleChanged.Invoke(item);
			//		SetAsmdefFolder();
				}
				if (Event.current != null && Event.current.alt)
				{
					markChildrenRecursively( item.isMarked, item);
			//		SetAsmdefFolder();
				}
				AsmdefManagerDatabase.Instance.SaveDatabase();
			}

			if(item.isSpecialFolder || item.isConflicting)
			{
				GUI.contentColor = AsmdefManagerEditorWindow.textColor;
			}
			if(item.isConflicting)
				args.label += "["+item.asmdefPath+"]";

			base.RowGUI(args);
			GUI.contentColor = originalTextColor;
		//	EditorGUI.EndDisabledGroup();
		}

		void markChildrenRecursively(bool mark, AsmdefTreeViewItem item)
		{
			if(item.isMarked != mark)
			{
				item.isMarked = mark;
				if (OnToggleChanged != null) { OnToggleChanged.Invoke(item); }
			}
			else
			{
				item.isMarked = mark;
			}
			if (!item.hasChildren) { return; }
			foreach (AsmdefTreeViewItem child in item.children)
			{
				markChildrenRecursively(mark, child);
			}
			
			AsmdefManagerDatabase.Instance.SaveDatabase();
		}

		//From now on we can enable children while their parents are exluded, but I'll leave this here if later on I'll need the functionality 
		bool isParentEnabledRecursive(AsmdefTreeViewItem tvItem)
		{
			if(tvItem.depth == 0) { return true; }
			TreeViewItem parent = tvItem;
			while (parent.parent.depth >= 0)
			{
				parent = parent.parent;
				return ((AsmdefTreeViewItem)parent).isMarked;
			}
			return false;
		}

		public void SetStateAll(bool newState)
		{
			bool changed = false;
			foreach (var item in tree)
			{
				AsmdefTreeViewItem aItem = (AsmdefTreeViewItem)item;

				bool lastVal = aItem.isMarked;
				aItem.isMarked = newState;
				if (lastVal != aItem.isMarked)
				{
					changed = true;
					if (OnToggleChanged != null) { OnToggleChanged.Invoke(aItem); }
				}
			}
			
			((AsmdefTreeViewItem)tree[0]).isMarked = false; //root Assets folder should always be unselected
			
			if(changed)
				OnAllToggleChanged?.Invoke();
		}
	}

	public class AsmdefTreeViewItem : TreeViewItem
	{
		/// <summary>
		/// Was its toggle ticked?
		/// </summary>
		public bool isMarked{ get; set; } //not called selected because it would be confusing as TreeViewItem already have a selected member
		/// <summary>
		/// in the tree, not on the hard drive
		/// </summary>
		public string fullPath { get; set; } 
		public string relativePath { get; set; }
		public string absoluteFilePath { get; set; } //in the hard drive, not on the tree

		/// <summary>
		/// asmdef containing the file will be placed here
		/// </summary>
		public string asmdefPath { get; set; }

		public bool isSpecialFolder { get; set; }
		public bool isConflicting { get; set; }
	}
}