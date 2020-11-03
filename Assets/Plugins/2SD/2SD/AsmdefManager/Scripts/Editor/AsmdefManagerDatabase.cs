using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace TSD.AsmdefManagement
{
	public class AsmdefManagerDatabase : ScriptableObject
	{
		public enum SessionProgress
		{
			WaitingForInput,
			LookForPasses,
			SelectAsmdefFolder,
			CreateAutoPassAssembliesAndReferences,
			Ready,
			Finished
		}

		[System.Serializable]
		public struct SessionData
		{

			public bool working;
			public SessionProgress sessionProgress;

			public List<string> scriptsInAssets;

			public List<string> firstPass;
			public List<string> firstPassEditor;
			public List<string> secondPass;
			public List<string> secondPassEditor;

			public SessionData(List<string> sA, List<string> fp, List<string> fpe, List<string> sp, List<string> spe, SessionProgress prog, bool isWorking = false)
			{
				scriptsInAssets = sA;
				firstPass = fp;
				firstPassEditor = fpe;
				secondPass = sp;
				secondPassEditor = spe;

				sessionProgress = prog;
				working = isWorking;
			}
		}

		[SerializeField]
		public SessionData sessionData = new SessionData();

		public List<string> alwaysIncludeAssemblies = new List<string>();
		public List<string> alwaysIncludeEditorAssemblies = new List<string>();
		public List<string> alwaysIncludeRuntimeAssemblies = new List<string>();
		
	//	public TreeViewState alwaysIncludeTreeViewState;
	//	public TreeViewState alwaysIncludeAssembliesTreeViewState;

		public bool initialSetupDone = false; //if false(~running for the first time) we will include every asmdef file found in the project
		
	//	public AsmdefAssembliesTreeViewBase alwaysIncAssTreeView;
		
		public string baseAsmdefFolder = "";
		public AutoASMDEFAssembliesGUID autoASMDEFAssembliesGUID;

		//settings
		internal bool showDebugMessages { get; set; } = false;
		internal bool showClassDebugMessages { get; set; } = false;

		[SerializeField]
		internal string manualAsmdefFile = ""; //this contains a reference to every auto-asmdef file, should be placed in a "myGame" directory or similar
		/// <summary>
		/// Contains every assembly
		/// </summary>
		[SerializeField]
		internal List<string> createdAsmdefFiles = new List<string>();
		/// <summary>
		/// Contains every created assembly reference
		/// </summary>
		[SerializeField]
		internal List<string> createdAsmrefFiles = new List<string>();
		/// <summary>
		/// Contains both First and Second pass Runtime assemblies
		/// </summary>
		[SerializeField]
		internal List<string> createdAsmdefFilesRuntime =  new List<string>();
		/// <summary>
		/// Contains both First and Second pass Editor assemblies
		/// </summary>
		[SerializeField]
		internal List<string> createdAsmdefFilesEditor = new List<string>();
		
		//1.1 and up
		[SerializeField]
		internal List<PathGUID> createdAsmrefFilesGUID = new List<PathGUID>();

		[SerializeField]
		internal List<PathGUID> createdAsmdefFilesGUID = new List<PathGUID>();
		//
		
		internal string excludedDirectoriesRegex { get; set; } = "";

		internal List<string> excludedDirectoryList { get; set; } = new List<string>();

		public HashSet<string> directories;

		public void AddDirectory(string path)
		{
			if(directories == null)
				directories = new HashSet<string>();
			directories.Add(path);
		}

		internal bool CanUseRegex = true;
		internal HashSet<string> DirectioriesThatPreventUsingRegex;
		public void CheckIfCanUseRegex()
		{
			if(DirectioriesThatPreventUsingRegex == null)
				DirectioriesThatPreventUsingRegex = new HashSet<string>();
			
			char[] forbiddenRegexCharacters = {'^', '$', '|', '*', '+', '(', ')', '[', ']', '{', '}' };
			CanUseRegex = directories.All(i => i.IndexOfAny(forbiddenRegexCharacters) == -1);
			
			DirectioriesThatPreventUsingRegex =
				new HashSet<string>(directories.Where(s => s.IndexOfAny(forbiddenRegexCharacters) != -1));
		}
		/// <summary>
		/// Creates a list from the excluded string
		/// </summary>
		internal void prepareExcluded()
		{
			excludedDirectoryList = Regex.Split(@excludedDirectoriesRegex, "\n").ToList();
		}

		//Refactor
		public bool isPathExcluded(string pathToCheck)
		{
		//	Debug.Log(pathToCheck);
			if (string.Equals(pathToCheck, "")) 
				return false;
			string[] lines = Regex.Split(excludedDirectoriesRegex, "\n"); //todo optimize, move this outside of the function
			
			pathToCheck = pathToCheck.Replace('\\', '/');
			foreach (var @line in lines)
			{
			//	Debug.Log("/Assets/" + @line);
			//	Debug.Log(pathToCheck);
				if ( CanUseRegex ?  Regex.IsMatch(pathToCheck, "^/Assets/" + @line + "$") : string.Equals(pathToCheck, "/Assets/" + @line))
				{
					return true;
				}
			}
			return false;
		}

		[ContextMenu("Find guids")]
		public void FindGUIDs()
		{
			AssetDatabase.Refresh();
			SetGUID(ref createdAsmrefFilesGUID, ref createdAsmrefFiles);
		}

		void SetGUID(ref List<PathGUID> combined, ref List<string> absolutePathOnly)
		{
			combined = new List<PathGUID>();
			foreach (var item in absolutePathOnly)
			{
				combined.Add(new PathGUID(item,AssetDatabase.AssetPathToGUID(item)));
			}
		}

		/// <summary>
		/// Call after the changes are done
		/// </summary>
		public void SaveDatabase()
		{
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			//	Debug.Log("save triggered");
		}

		public void tryAddIncluded(AsmdefIncludeAssembliesTreeViewItemBase viewItem)
		{

			if (viewItem.isMarked)
			{
				alwaysIncludeAssemblies.Add(viewItem.displayName);
			}
			else
			{
				if (alwaysIncludeAssemblies.Contains(viewItem.displayName)) { alwaysIncludeAssemblies.Remove(viewItem.displayName); }
			}
			if(viewItem.isRuntimeMarked)
			{
				alwaysIncludeRuntimeAssemblies.Add(viewItem.displayName);
			}
			else
			{
				if (alwaysIncludeRuntimeAssemblies.Contains(viewItem.displayName)) { alwaysIncludeRuntimeAssemblies.Remove(viewItem.displayName); }
			}
			if (viewItem.isEditorMarked)
			{
				alwaysIncludeEditorAssemblies.Add(viewItem.displayName);
			}
			else
			{
				if (alwaysIncludeEditorAssemblies.Contains(viewItem.displayName)) { alwaysIncludeEditorAssemblies.Remove(viewItem.displayName); }
			}

			alwaysIncludeAssemblies			= alwaysIncludeAssemblies.Distinct().ToList();
			alwaysIncludeRuntimeAssemblies	= alwaysIncludeRuntimeAssemblies.Distinct().ToList();
			alwaysIncludeEditorAssemblies	= alwaysIncludeEditorAssemblies.Distinct().ToList();

			if(showDebugMessages)
				Debug.Log(viewItem.displayName);

		//	EditorUtility.SetDirty(this);
		//	AssetDatabase.SaveAssets();
		}

		//Only add if it isn't there already
		public void TryAddExcludedPath(AsmdefTreeViewItem treeviewItem)
		{
			string pathToAdd = treeviewItem.fullPath + "/";
			excludedDirectoryList = Regex.Split(@excludedDirectoriesRegex, "\n").ToList();
			pathToAdd = pathToAdd.Replace('\\', '/');
			pathToAdd = pathToAdd.Substring(7);

			bool found = false;
			for (int i = 0; i < excludedDirectoryList.Count; i++)
			{
				if(excludedDirectoryList[i] == pathToAdd)
				{
					excludedDirectoryList[i] = string.Empty;
					found = true;
				}
			}
			if (!found) { excludedDirectoriesRegex += "\n" + pathToAdd; }
			else
			{
				excludedDirectoriesRegex = "";
				foreach (var item in excludedDirectoryList)
				{
					if( !Regex.IsMatch(@item, @"^\s+$[\r\n]*"))
					{
						excludedDirectoriesRegex += @item + "\n";
					}
				}
			}
			excludedDirectoriesRegex = RemoveEmptyLines(excludedDirectoriesRegex);
			
		//	SaveDatabase();
		}

		private string RemoveEmptyLines(string lines)
		{
			return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
		}

		[System.Serializable]
		internal struct PathGUID
		{
			public string absolutePath;
			public string GUID;

			public PathGUID(string absolutePath, string GUID)
			{
				this.absolutePath = absolutePath;
				this.GUID = GUID;
			}
		}
		
		[System.Serializable]
		public class AutoASMDEFAssembliesGUID
		{
			public string firstPass;
			public string firstPassEditor;
			public string secondPass;
			public string secondPassEditor;

			public bool containsGUID(string GUID)
			{
				string[] temp = {firstPass, firstPassEditor, secondPass, secondPassEditor};
				return temp.Contains(GUID);
			}

			/// <summary>
			/// Returns true if any of the created ASMDEF files are in the set folder(or child of the folder)
			/// </summary>
			/// <param name="localBasePath"></param>
			/// <returns></returns>
			public bool isAnyInFolder(string localBasePath)
			{
				var paths = new List<string>();
				paths.Add(AssetDatabase.GUIDToAssetPath(firstPass));
				paths.Add(AssetDatabase.GUIDToAssetPath(firstPassEditor));
				paths.Add(AssetDatabase.GUIDToAssetPath(secondPass));
				paths.Add(AssetDatabase.GUIDToAssetPath(secondPassEditor));
				Debug.Log(paths[0]);
				Debug.Log(localBasePath);
				return paths.Any(item => item.StartsWith(localBasePath));
			}
		}

		static AsmdefManagerDatabase instance;
		public static AsmdefManagerDatabase Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (AsmdefManagerDatabase)Resources.LoadAll("", typeof(AsmdefManagerDatabase)).FirstOrDefault();
					if (instance == null)
					{
						instance = (AsmdefManagerDatabase)CreateInstance(typeof(AsmdefManagerDatabase));

					//	Debug.Log(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(AsmdefManagerEditorWindow.Instance)));
						string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(AsmdefManagerEditorWindow.Instance));
						path = path.Replace('\\', '/');
						path = path.Substring(0, path.LastIndexOf("/"));
						path += "/Resources/";
						if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
						AssetDatabase.Refresh();
						AssetDatabase.CreateAsset(instance, path + instance.GetType().ToString() + ".asset");
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						
						AsmdefManagerEditorWindow.Instance.RunCleanup();
					}
				}
				return instance;
			}
		}
	}
}