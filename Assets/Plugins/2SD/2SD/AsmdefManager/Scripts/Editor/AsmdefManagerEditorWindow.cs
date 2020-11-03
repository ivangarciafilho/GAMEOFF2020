using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.IMGUI.Controls;

namespace TSD.AsmdefManagement
{
	/* Creates a First Pass Runtime, First Pass Editor, Second Pass Runtime, Second Pass Editor equivalent based on the old Script Compilation Order
		* Compilation order 
		* Phase 1
		* Standard Assets, Pro Standard Assets, Plugins
		* Phase 2
		* Editor (when it's located in the folders compiled in Phase 1)
		* Phase 3
		* Everything else as long as the folder's name isn't Editor
		* Phase 4
		* Editor folders inside folders compiled in Phase 3
	*/

	//Make sure we only change folders that aren't using the default Assembly-CSharp assembly(~so we don't change stuff that already in its own assembly)

	public partial class AsmdefManagerEditorWindow : EditorWindow
	{
		/// <summary>
		/// Default Unity assemblies
		/// </summary>
		readonly List<string> baseAssemblies = new List<string>() { "Assembly-CSharp", "Assembly-CSharp-Editor" };

		private const string assemblyFirstPassRuntime = "Assembly-CSharp-firstpass";
		private const string assemblyFirstPassEditor = "Assembly-CSharp-Editor-firstpass";
		private const string assemblySecondEditorAssembly = "Assembly-CSharp-Editor";
		private const string assemblySecondRuntimeAssembly = "Assembly-CSharp";

		private const string editorFolders = "Editor";

		/// <summary>
		/// Added before the saved asmdef
		/// </summary>
		string customAsmdefPrefix = "Auto-asmdef";

		private string asmdefFirstPass = "FirstPass";
		private string asmdefFirstPassEditor = "FirstPassEditor";
		private string asmdefSecondPass = "SecondPass";
		private string asmdefSecondPassEditor = "SecondPassEditor";
		

		System.Action OnFolderLookupFinished;

		List<string> scriptsInAssets	= new List<string>();

		List<string> firstPassRoots		= new List<string>();
		List<string> firstPassEditors	= new List<string>();
		List<string> secondPassRuntime	= new List<string>();
		List<string> secondPassEditor	= new List<string>();

		public Dictionary<string, string> dCombinedPass = new Dictionary<string, string>();
		
		AsmdefManagerDatabase database { get { return AsmdefManagerDatabase.Instance; } }

		//Toggles
		bool toggleShowSettings;
		bool toggleShowAlwaysReferenced;
		bool toggleShowFolderHierarchy;
		bool toggleShowExcludeTextField;

		Vector2 scroll;

		[SerializeField] TreeViewState m_TreeViewState;
		AsmdefTree folderStructureTreeView;

		[SerializeField] TreeViewState alwaysIncludeAssembliesTreeViewState;
		AsmdefAssembliesTreeViewBase m_IncAssTreeView;
		
		List<UnityEditor.Compilation.Assembly> loadedAssemblies;

		string fileSearchPattern = "*.cs";
		private static string[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
		{
			return filters.Split('|').SelectMany(filter => Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();
		}

		#region window

		static AsmdefManagerEditorWindow window;
		public static AsmdefManagerEditorWindow Instance { get { if (window == null) { Init(); } return window; } }
		// Add menu named "My Window" to the Window menu
	//	[MenuItem("Window/2SD/Auto Assembly Definition Manager")]
		[MenuItem("Window/2SD/Auto Assembly Definition Manager &q")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			window = (AsmdefManagerEditorWindow)EditorWindow.GetWindow(typeof(AsmdefManagerEditorWindow));
			window.LoadTextures();
			window.Show();
		}

		#endregion

		void OnEnable()
		{
		//	Debug.Log(AsmdefGUIDataScriptibleObject.Instance);
			LoadTextures();
			RunCleanup();
			ScanProject();
		}

		/// <summary>
		/// Looks for folders, scripts and assemblies. Takes a while, it's normal.
		/// </summary>
		private void ScanProject()
		{
			lookForScripts();
			//init always include assemblies

			/*if(database.alwaysIncludeTreeViewState == null)
				database.alwaysIncludeTreeViewState = new TreeViewState(); 
			folderStructureTreeView = new AsmdefTree(database.alwaysIncludeTreeViewState);*/
			if (m_TreeViewState == null)
				m_TreeViewState = new TreeViewState(); 
			folderStructureTreeView = new AsmdefTree(m_TreeViewState);

			folderStructureTreeView.SetExpanded(1, true); //the Assets folder should be unfolded by default
			folderStructureTreeView.OnToggleChanged += database.TryAddExcludedPath;
			folderStructureTreeView.OnAllToggleChanged += database.SaveDatabase;
			folderStructureTreeView.OnItemCreated += setTreeviewItemData;

			//Loops through the created list and calls OnItemCreated. It's slow, but it is necessary to see if a folder's content is in a custom assembly
			folderStructureTreeView.LoopThroughCreatedItems(); 
			
			loadedAssemblies = CompilationPipeline.GetAssemblies().ToList();
			loadedAssemblies.Sort((item1, item2) => string.Compare(item1.name, item2.name, StringComparison.Ordinal));
			loadedAssemblies = loadedAssemblies.Where(ass => !baseAssemblies.Contains(ass.name) && !ass.name.StartsWith("UnityEditor")).ToList();
			
			//get loaded asmdef files
			List<string> loadedSrt = new List<string>();
			foreach (var item in loadedAssemblies)
			{
				var path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(item.name);
				if(path != null)
				{
					bool isInAssets = false;
					AsmdefObject temp = null;
					path = path.Replace("\\", "/");
					var fullpath = Application.dataPath + "/" + path.Substring(7);
					//			Debug.Log(fullpath);
					if (File.Exists(fullpath))
					{
						var sr = new StreamReader(fullpath);
						var fileContents = sr.ReadToEnd();
						sr.Close();
						//		Debug.Log(fullpath);
						//		Debug.Log(path);
						
						temp = JsonUtility.FromJson<AsmdefObject>(fileContents);
						isInAssets = true;
					}
					
					if(!isInAssets)
						loadedSrt.Add(item.name );
					else
					{
						if(temp.additionalData != "AutoASMDEF")
							loadedSrt.Add(item.name );
						else
							Debug.LogWarning("Tried to add assembly" + item.name);
						
					}
					if(database.showDebugMessages)
						Debug.Log(CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(item.name));
				}
			}

			/*if (database.alwaysIncludeAssembliesTreeViewState == null)
				database.alwaysIncludeAssembliesTreeViewState = new TreeViewState();
			m_IncAssTreeView = new AsmdefAssembliesTreeViewBase(database.alwaysIncludeAssembliesTreeViewState, loadedSrt);*/
			if (alwaysIncludeAssembliesTreeViewState == null)
				alwaysIncludeAssembliesTreeViewState = new TreeViewState();
			m_IncAssTreeView = new AsmdefAssembliesTreeViewBase(alwaysIncludeAssembliesTreeViewState, loadedSrt);
			
			m_IncAssTreeView.OnIsMarked += database.tryAddIncluded;
			m_IncAssTreeView.OnIsRuntimeMarked += database.tryAddIncluded;
			m_IncAssTreeView.OnIsEditorMarked += database.tryAddIncluded;

			if (!database.initialSetupDone)
			{
				m_IncAssTreeView.SetStateAll(true);
				database.initialSetupDone = true;
			}
			
			AsmdefManagerDatabase.Instance.CheckIfCanUseRegex();
		}

		#region Update Database

		internal void UpdateDatabase()
		{
			//looks for scripts that are not included
			lookForScripts();
			//puts them into passes
			lookForPasses();
			//gets the local folders for the existing asmref holder folders
			var existing = GetCreatedASMREFPathFolders();
			
			//loop through each new script and see in which asmdef do they belong
			//create the new asmref and asmdef files if necessary
			//then merge the new file list with the old
			foreach (var VARIABLE in secondPassRuntime)
			{
				if(!existing.Contains(VARIABLE.Substring(0, VARIABLE.LastIndexOf('/'))))
					Debug.Log("[Add]" + VARIABLE);
			}
		}

		List<string> GetCreatedASMREFPathFolders()
		{
			List<string> _paths = new List<string>();
			foreach (var VARIABLE in database.createdAsmrefFilesGUID)
			{
				var p = AssetDatabase.GUIDToAssetPath(VARIABLE.GUID);
				if(File.Exists(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + p))
					_paths.Add(p.Substring(0, p.LastIndexOf('/'))); //-".asmref"
			}
			return _paths;
		}
		
		#endregion
		
		//give this some more thought
		internal void isConflicting(AsmdefTreeViewItem item)
		{
			var path = item.fullPath.Substring(6, item.fullPath.Length - 6) + "/";
			var fullpath = Application.dataPath + path;
			
			var m_path = fullpath;
			if (m_path.Length > m_path.LastIndexOf('/') + 2)
			{
				m_path = m_path.Substring(0, m_path.LastIndexOf('/') + 1);
			}
			if (item.isMarked)
			{
				item.isConflicting = isSubpathExistsMultipleTimes(m_path); //do some magic here
			}
			else
			{
				item.isConflicting = false; //do some magic here
			}
		}

		#region GUI

		float maxWidth;
		float currentHeight;

		void setTreeviewItemData(AsmdefTreeViewItem item)
		{
			//for easier treeview formatting this also contains the Assets folder, which we don't need
			string path = item.fullPath.Substring(6, item.fullPath.Length - 6) + "/";
			string fullpath = Application.dataPath + path;

			if (!Directory.Exists(fullpath)) { return; }
			var dir = GetFiles(fullpath, fileSearchPattern, SearchOption.TopDirectoryOnly);// Directory.GetFiles(fullpath, "*.cs", SearchOption.TopDirectoryOnly);
			if (dir == null || dir.Length == 0) { return; }
			item.isSpecialFolder = !isInBaseAssembly(fullpath, CompilationFolder.Any) && !isPartOfAssembly(fullpath, database.createdAsmdefFiles);
			var m_path = fullpath;
			if (m_path.Length > m_path.LastIndexOf('/') + 2)
			{
				m_path = m_path.Substring(0, m_path.LastIndexOf('/') + 1);
			}
			if (item.isMarked)
			{
				item.isConflicting = !isSubpathExistsMultipleTimes(m_path); //do some magic here
			}
			else
			{
				item.isConflicting = false; //do some magic here
			}
		}
		
		void OnGUI()
		{
			
			LoadTextures();
		//	GUILayout.Label(AsmdefGUIDataScriptibleObject.Instance.tLogo);
		//	GUILayout.Label(AsmdefGUIData.tLogo, AsmdefGUIData.sBoxCheckbox);
		
			maxWidth = currentHeight > Screen.height ? position.width - 24f : position.width - 8f;
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(tLogo);//, sBoxCheckbox);//, sBoxCheckbox); 
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			if (EditorApplication.isCompiling)
			{
				EditorGUILayout.HelpBox("Compiling..", MessageType.Warning, true);
				return;
			}

			/*if (GUILayout.Button("look for scripts"))
			{
			//	lookForScripts();
			//	lookForPasses();
				ScanProject();
			}*/
			
			InitStyles();
			
			//	minSize = new Vector2(500, 350);
			
			folderScroll = EditorGUILayout.BeginScrollView(folderScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none);
			showMainASMDEFFolder();
			
			showProgressBar();

			showSetupGuide();

			showSettings();
			showAlwaysReferencedAssemblies();
			showFolderHierarchy();
			
		//	scroll = EditorGUILayout.BeginScrollView(scroll);

			showExcludedFoldersTextbox();

			showControls2019_2();
			showErrorMessages();
			currentHeight = GUILayoutUtility.GetLastRect().height;
			EditorGUILayout.EndScrollView();

			/*
			var t = GUILayoutUtility.GetLastRect().height.ToString();
			GUILayout.Label(t);
			GUILayout.Label(position.size.ToString());*/

			//	EditorGUILayout.EndScrollView();
		}

		private void Update()
		{

			Repaint();
			if (doNextStep)
			{
				createAsmdef();
			}
		}

		void showControls2019_2()
		{
			if (database.sessionData.sessionProgress == AsmdefManagerDatabase.SessionProgress.Finished)
			{
				
				//UpdateDatabase
				if (GUILayout.Button( new GUIContent( "Update Assembly Definitions", BtnRescanTexture, "Looks for new files and adds them to their desired assembly definition without the need to revert first"), styleBoxButton, GUILayout.Width(maxWidth), GUILayout.Height(BtnSizeBig)))
				{
					//UpdateDatabase();
					database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.WaitingForInput;
					createAsmdef();
				}
				/*if (GUILayout.Button(new GUIContent("Create Assembly With References To Every Auto-Assembly", AsmdefGUIData.BtnForwardTexture), styleBoxButton, GUILayout.Width(maxWidth), GUILayout.Height(AsmdefGUIData.BtnSizeBig)))
				{
					createAsmdefInCustomFolder();
				}*/
				if (GUILayout.Button( new GUIContent("Revert created Assembly Definition files", BtnBackTexture), styleBoxButton, GUILayout.Width(maxWidth), GUILayout.Height(BtnSizeBig)))
				{
					revertCreatedFiles();
				//	removeCreatedAsmdefFiles();
				}
			}
			else
			{
				GUILayout.BeginVertical();
				if (GUILayout.Button( new GUIContent( "Start Process", BtnForwardTexture, "Creates asmdef and asmref files"), styleBoxButton, GUILayout.Width(maxWidth), GUILayout.Height(BtnSizeBig)))
				{
					createAsmdef();
				}

				GUILayout.EndVertical();

				switch (database.sessionData.sessionProgress)
				{
					case AsmdefManagerDatabase.SessionProgress.WaitingForInput:
						break;
					case AsmdefManagerDatabase.SessionProgress.LookForPasses:
						break;
					case AsmdefManagerDatabase.SessionProgress.SelectAsmdefFolder:
						EditorGUILayout.HelpBox("Select an empty folder(click on the button) which will be used to store the generated .asmdef files", MessageType.Info);
						break;
					case AsmdefManagerDatabase.SessionProgress.CreateAutoPassAssembliesAndReferences:
						break;
					case AsmdefManagerDatabase.SessionProgress.Ready:
						break;
					case AsmdefManagerDatabase.SessionProgress.Finished:
						break;
					default:
						break;
				}
			}
		}

		void showMainASMDEFFolder()
		{
			GUILayout.Box(database.baseAsmdefFolder, GUILayout.Width(maxWidth), GUILayout.Height(32f));
		}

		GUIStyle styleBoxEnabled;
		GUIStyle styleBoxDisabled;
		GUIStyle styleBoxButton;
		GUIStyle styleBoxButtonSmallFont;
		GUIStyle styleBoxButtonSmallFontCenterText;
		GUIStyle styleBoxCheckbox;

		private void InitStyles()
		{
		//	if(styleBoxEnabled == null)
			{
				styleBoxEnabled = new GUIStyle(GUI.skin.box);
				styleBoxEnabled.normal.background = TstyleBoxEnabled;// MakeTex(2, 2, new Color(.0f, .45f, 1f, 1f));
				styleBoxEnabled.hover.background = styleBoxEnabled.normal.background;
				styleBoxEnabled.alignment = TextAnchor.MiddleLeft;
				styleBoxEnabled.normal.textColor = textColor;
			}
			
		//	if(styleBoxDisabled == null)
			{
				styleBoxDisabled = new GUIStyle(styleBoxEnabled);
				styleBoxDisabled.normal.background = TstyleBoxDisabled;// MakeTex(2, 2, new Color(.3f, .3f, .3f, 0.5f));
				styleBoxDisabled.normal.textColor = textColor;// new Color(.9f, .9f, .9f, 1f);
				styleBoxDisabled.hover.background = styleBoxEnabled.normal.background;
			}
		//	if (styleBoxButton == null)
			{
				styleBoxButton = new GUIStyle(GUI.skin.button);
				styleBoxButton.normal.background = styleBoxDisabled.normal.background;
				styleBoxButton.active.background = styleBoxEnabled.normal.background;
				styleBoxButton.hover.background = styleBoxEnabled.normal.background;
				styleBoxButton.alignment = TextAnchor.MiddleLeft;
				styleBoxButton.fontSize = 14;
				styleBoxButton.normal.textColor = textColor;
			}
		//	if (styleBoxButtonSmallFont == null)
			{
				styleBoxButtonSmallFont = new GUIStyle(styleBoxButton);
				styleBoxButtonSmallFont.normal.background = styleBoxDisabled.normal.background;
				styleBoxButtonSmallFont.active.background = styleBoxEnabled.normal.background;
				styleBoxButtonSmallFont.fontSize = 12;
			}
			{
				styleBoxButtonSmallFontCenterText = new GUIStyle(styleBoxButtonSmallFont);
				styleBoxButtonSmallFontCenterText.alignment = TextAnchor.MiddleCenter;
			}
			{
				styleBoxCheckbox = new GUIStyle(GUI.skin.toggle);
				styleBoxCheckbox.normal.background = styleBoxDisabled.normal.background;
				styleBoxCheckbox.active.background = TstyleBoxEnabled;// MakeTex(2, 2, new Color(.0f, .55f, 1f, 1f));
				styleBoxCheckbox.hover.background = styleBoxEnabled.normal.background;

				styleBoxCheckbox.onNormal.background = styleBoxEnabled.normal.background;
				styleBoxCheckbox.onHover.background = styleBoxEnabled.normal.background;
				styleBoxCheckbox.onActive.background = styleBoxEnabled.normal.background;

				styleBoxCheckbox.onNormal.textColor = new Color(.0f, .0f, 0f, 1f);
				styleBoxCheckbox.onActive.textColor = new Color(.0f, .0f, 0f, 1f);
				styleBoxCheckbox.onHover.textColor = new Color(.0f, .0f, 0f, 1f);
				styleBoxCheckbox.alignment = TextAnchor.MiddleCenter;
			}
		}

		void showSetupGuide()
		{
			if (GUILayout.Button(new GUIContent("Open setup guide in browser", BtnYoutubeTexture), styleBoxButtonSmallFont, GUILayout.Height(38f), GUILayout.Width(maxWidth)))
			{
				Application.OpenURL("https://youtu.be/HmKaWJ_KW8U?t=438");
			}
		}

		void showSettings()
		{
			if ( GUILayout.Button(new GUIContent("Debug Settings", BtnSettingsTexture), toggleShowSettings ? styleBoxEnabled : styleBoxDisabled, GUILayout.Height(38f), GUILayout.Width(maxWidth)))
			{
				toggleShowSettings = !toggleShowSettings;
			}
		//	toggleShowSettings = EditorGUILayout.Foldout(toggleShowSettings, new GUIContent( "Debug Settings", AsmdefGUIData.BtnSettingsTexture), customButton);
			if (toggleShowSettings)
			{
				EditorGUILayout.HelpBox("You usually don't want to turn these settings on - but if for some reason the proccess fails turning these settings on will help identify where the problem is.", MessageType.Info);
				GUILayout.BeginVertical();
				
				GUILayout.BeginHorizontal();
				GUILayout.Space(16f);//indent
				var doubleButtonMaxWidth = maxWidth / 2 - 15;
				database.showDebugMessages = GUILayout.Toggle( database.showDebugMessages, "Show Debug Messages", sBoxCheckbox, GUILayout.Height(32f), GUILayout.Width(doubleButtonMaxWidth));
				database.showClassDebugMessages = GUILayout.Toggle(database.showClassDebugMessages, "Show Class Debug Messages", styleBoxCheckbox, GUILayout.Height(32f), GUILayout.Width(doubleButtonMaxWidth));
				GUILayout.Space(21f);//indent
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Space(16f);//indent
				if (GUILayout.Button(new GUIContent("Rescan folders", BtnRescanTexture, "Runs the folder discovery again from scratch, useful if you changed folders and don't want to reopen the window."), styleBoxButton, GUILayout.Height(38f), GUILayout.Width(maxWidth - 26)))
				{
					ScanProject();
					//if (GUILayout.Button(new GUIContent("Always Referenced Assemblies", AsmdefGUIData.BtnReferencesTexture), toggleShowAlwaysReferenced ? styleBoxEnabled : styleBoxDisabled, GUILayout.Height(38f), GUILayout.Width(maxWidth)))
					//todo
				}
				GUILayout.Space(12f);//indent
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				GUILayout.Space(16f);//indent
				if (GUILayout.Button(new GUIContent("RunCleanup", BtnRescanTexture, "If you accidentally delete the database created by the tool it won't be able to remove the created files. By running a cleanup it'll attempt to sweep through the project looking for both asmref and asmdef files that has the additionalData 'AutoASMDEF' added to them - however to avoid losing any data it'll not touch any of the folders, so you might end up with a Assets/AutoASMDEF with a couple of subfolders. You can safely delete these by hand."), styleBoxButton, GUILayout.Height(38f), GUILayout.Width(maxWidth - 26)))
				{
					RunCleanup();
				}
				GUILayout.Space(12f);//indent
				GUILayout.EndHorizontal();
				
				
				GUILayout.EndVertical();
			}
		}

		void showProgressBar()
		{
			GUILayout.Box("", GUILayout.Width(maxWidth), GUILayout.Height(48f));

			float progress = ((float)database.sessionData.sessionProgress) / 4f; //there is 4 steps

			EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), progress, "");

			GUI.Label(GUILayoutUtility.GetLastRect(), database.sessionData.sessionProgress.ToString(), StyleTextAlignedMiddle);
		}

		void showAlwaysReferencedAssemblies()
		{
			if (GUILayout.Button(new GUIContent("Always Referenced Assemblies", BtnReferencesTexture), toggleShowAlwaysReferenced ? styleBoxEnabled : styleBoxDisabled, GUILayout.Height(38f), GUILayout.Width(maxWidth)))
			{
				toggleShowAlwaysReferenced = !toggleShowAlwaysReferenced;
			}
		//	toggleShowAlwaysReferenced = EditorGUILayout.Foldout(toggleShowAlwaysReferenced, "Always Referenced Assemblies");
			if (toggleShowAlwaysReferenced)
			{
				GUILayout.BeginHorizontal();
				
				EditorGUILayout.HelpBox("If you are not sure what to include, include everything. This will lead to the exact same behaviour as without using asmdef files.", MessageType.Warning);
				GUILayout.Space(16);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("Checboxes: Both, Runtime Only, Editor Only", MessageType.Info);
				GUILayout.Space(16);
				GUILayout.EndHorizontal();
				
				//	GUILayout.Label("Checboxes: Both, Runtime Only, Editor Only");
				var height = Mathf.Min(Screen.height - 340, Mathf.RoundToInt(Screen.height * 0.65f));
				height = Mathf.Abs(height);
				var pos = new Rect(0, GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height, maxWidth, height);
				
				m_IncAssTreeView.OnGUI(pos);

				GUILayout.Space(Mathf.Min(m_IncAssTreeView.totalHeight, height));
				
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Toggle All Runtime", styleBoxButtonSmallFontCenterText)) { m_IncAssTreeView.ToggleState(true);}
				if (GUILayout.Button("Toggle All Editor", styleBoxButtonSmallFontCenterText)) { m_IncAssTreeView.ToggleState(false); }
				GUILayout.EndHorizontal();
				
			}
		}
		
		void showLabelWithColoredBackground(string label, Color backgroundColor)
		{
			GUILayout.Label(label);
			DrawColoredRectangle(GUILayoutUtility.GetLastRect(), backgroundColor);

			Color originalColor = GUI.contentColor;
			GUI.contentColor = textColor;
			GUI.Label(GUILayoutUtility.GetLastRect(), label, EditorStyles.boldLabel);
			GUI.contentColor = originalColor;
		}
		Vector2 folderScroll;
		void showFolderHierarchy()
		{

			if (GUILayout.Button(new GUIContent("Folder Hierarchy", BtnHierarchyTexture), toggleShowFolderHierarchy ? styleBoxEnabled : styleBoxDisabled, GUILayout.Height(38f), GUILayout.Width(maxWidth)))
			{
				toggleShowFolderHierarchy = !toggleShowFolderHierarchy;
			}
		//	toggleShowFolderHierarchy = EditorGUILayout.Foldout(toggleShowFolderHierarchy, "Folder Hierarchy");

			if(toggleShowFolderHierarchy)
			{

				GUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("To get the best compilation times make sure to select every 3rd party asset, but deselect your own code. This way your code will be compiled after everything else, so when you iterate over it in the editor you only have to wait for your code to recompile, the 3rd party code will be compiled already.", MessageType.Warning);
				GUILayout.Space(16);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("Alt+click to select/deselect recursively", MessageType.Info);
				GUILayout.Space(16);
				GUILayout.EndHorizontal();

				string customAssemblyMarked = "Already in a custom assembly, will be skipped";
			//	string issuesMarked = "Conflict";
				//Header
				GUILayout.BeginHorizontal();
				GUILayout.Space(14);
				GUILayout.Label("Include Folders", EditorStyles.boldLabel);
				//
				showLabelWithColoredBackground(customAssemblyMarked, colorInManualAssembly);
			//	showLabelWithColoredBackground(issuesMarked, AsmdefGUIData.colorIssue);

				GUILayout.EndHorizontal();

				//Tree
				var height = Mathf.Min(Screen.height - 160, Mathf.RoundToInt(Screen.height * 0.65f));
				height = Mathf.Abs(height) - 160;
			//	height = height - Mathf.RoundToInt(GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height);
				var pos = new Rect(14, GUILayoutUtility.GetLastRect().y + GUILayoutUtility.GetLastRect().height, maxWidth - 28, height);
				
				folderStructureTreeView.OnGUI(pos);

				GUILayout.Space(Mathf.Min(folderStructureTreeView.totalHeight, height));// Mathf.Clamp(m_SimpleTreeView.totalHeight, 0, height));

			//	EditorGUILayout.EndScrollView();
				//Select\deselect all btns		
				GUILayout.BeginHorizontal();
		//		GUILayout.Space(16f);//indent
				if (GUILayout.Button("Select All", styleBoxButtonSmallFontCenterText)) { folderStructureTreeView.SetStateAll(true); }
				if (GUILayout.Button("Deselect All", styleBoxButtonSmallFontCenterText)) { folderStructureTreeView.SetStateAll(false); }
		//		GUILayout.Space(16f);//indent
				GUILayout.EndHorizontal();
			}
		}

		void showExcludedFoldersTextbox()
		{
			if (GUILayout.Button(new GUIContent("Exclude Textbox", BtnTextboxTexture), toggleShowExcludeTextField ? styleBoxEnabled : styleBoxDisabled, GUILayout.Height(38f), GUILayout.Width(maxWidth)))
			{
				toggleShowExcludeTextField = !toggleShowExcludeTextField;
			}
		//	toggleShowExcludeTextField = EditorGUILayout.Foldout(toggleShowExcludeTextField, "Exclude Textbox");
			if(toggleShowExcludeTextField)
			{
				if (database.CanUseRegex)
				{
					EditorGUILayout.HelpBox("You can use regex or relative path to manually ignore folders - this is what's actually used, the folders set in the Folder Hierarchy tab will end up here as well", MessageType.Info);
					EditorGUILayout.HelpBox("Each excluded folder must be in a new line", MessageType.Info);
					EditorGUILayout.HelpBox("Regex example, to deselect MyFolder and its subfolders type \nMyFolder/?.*", MessageType.Info);
				}
				else
				{
					EditorGUILayout.HelpBox("These folders prevent you from using regex because they have at least one character in their path that's used by regex. If you don't want to use regex you can ignore this message.", MessageType.Warning);
					string temp = "";
					foreach (var VARIABLE in database.DirectioriesThatPreventUsingRegex)
					{
						temp += VARIABLE.Substring(Application.dataPath.Length +1, VARIABLE.Length - Application.dataPath.Length -1);
					}
					EditorGUILayout.TextArea(temp);
				}
				
				GUILayout.Label(database.excludedDirectoriesRegex.Split('\n').Length.ToString());

				GUILayout.Label("Assets/");
				database.excludedDirectoriesRegex = EditorGUILayout.TextArea(database.excludedDirectoriesRegex);
				if (GUILayout.Button("refresh", styleBoxButtonSmallFontCenterText)) { database.prepareExcluded(); }
			}
		}

		void showErrorMessages()
		{
			if(errorMsgs.Count > 0)
			{
				foreach (var item in errorMsgs)
				{
					EditorGUILayout.HelpBox(item, MessageType.Error);
				}
			}
		}

		List<string> conflictingFolders = new List<string>(); //not used anywhere??

		#endregion

		List<string> getDistinctFileListInAssembly(List<string> everyScript , string assemblyToLookFor)
		{
			var tempList = everyScript
				.Where(s => AsmdefUtils.GetAssembly(s) == assemblyToLookFor)
				.Select(s =>s.Remove(s.LastIndexOf('/')))
				.Distinct()
				.ToList();
			return tempList.Select(s => s + formatString(s) + ".asmref").ToList();
		}

		string formatString(string input)
		{
			//if it's in the root Assets folder we have to format it a bit differently 
			if( input.Count(c => c == '/') == 0)
				return "/" + input;
			else
				return input.Substring(input.LastIndexOf('/'));
		}

		string convertPathToFolder(string path)
		{
			var t = path.Substring(7);				//remove the "Assets/" from the beginning
			t = t.Remove(t.LastIndexOf('/') + 1);	//remove the file itself
			return t;
		}
/// <summary>
/// /////////////////////////////////////////////////////////////////////////////////////
/// </summary>
		void lookForScripts()
		{
			database.prepareExcluded();
			scriptsInAssets.Clear();
			scriptsInAssets = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories)
				.Select(s => s.Substring(s.LastIndexOf( Application.dataPath) + Application.dataPath.Length - ("Assets".Length))) //.Select(s => s.Substring(s.LastIndexOf("Assets")))
				.Select(s=> s.Replace(@"\", @"/"))
				.Where(s=> !AsmdefUtils.IsMatch(Application.dataPath + "/" + convertPathToFolder(s), database.excludedDirectoryList))
				.ToList();
		}

		void lookForPasses()
		{
			firstPassRoots = getDistinctFileListInAssembly(scriptsInAssets, assemblyFirstPassRuntime);
			firstPassEditors = getDistinctFileListInAssembly(scriptsInAssets, assemblyFirstPassEditor);
			secondPassRuntime = getDistinctFileListInAssembly(scriptsInAssets, assemblySecondRuntimeAssembly);
			secondPassEditor = getDistinctFileListInAssembly(scriptsInAssets, assemblySecondEditorAssembly);

			var updateFirstPass =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefFirstPass);
			var updateFirstPassEditor =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefFirstPassEditor);
			var updateSecondPass =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefSecondPass);
			var updateSecondPassEditor =
				getDistinctFileListInAssembly(scriptsInAssets, customAsmdefPrefix + "-" + asmdefSecondPassEditor);
			
			firstPassRoots.AddRange(updateFirstPass);
			firstPassEditors.AddRange(updateFirstPassEditor);
			secondPassRuntime.AddRange(updateSecondPass);
			secondPassEditor.AddRange(updateSecondPassEditor);

		}

		/// <summary>
		/// Store the connection between the file absolute path and the .asmdef that will be created for the path
		/// </summary>
		/*void findFolderConnections()
		{
			dCombinedPass = new Dictionary<string, string>();
			combine(ref dCombinedPass, firstPassRoots);
			combine(ref dCombinedPass, firstPassEditors);
			combine(ref dCombinedPass, secondPassRuntime);
			combine(ref dCombinedPass, secondPassEditor);
		}

		void combine(ref Dictionary<string, string> result, List<string> inputA)
		{
			List<string> highestPath = GetHighestSubPaths(inputA);
			for (int i = 0; i < inputA.Count; i++)
			{
				if(!result.ContainsKey(@inputA[i]))
				{
					result.Add(@inputA[i], @inputA[i]);
				}
			}
		}*/

		bool isSubpathExistsMultipleTimes(string pathToLookUp)
		{
			if (!dCombinedPass.ContainsKey(pathToLookUp)) { return false; }
			return dCombinedPass.Where(x => x.Value == dCombinedPass[@pathToLookUp]).ToList().Count == 1 ? false : true;
		}
		/// <summary>
		/// Returns true if a script in the specified folder(top folder only!) is in a default Unity assembly.
		/// Only requires the folder, it will look for a script in it automatically.
		/// </summary>
		/// <param name="absoluteFolderPath">ie C:/MyProject/Assets/myFolder</param>
		/// <returns></returns>
		bool isInBaseAssembly(string absoluteFolderPath, CompilationFolder compilationFolder)
		{
			var temp = new List<string>();
			switch (compilationFolder)
			{
				case CompilationFolder.Editor:
					temp.Add(assemblyFirstPassEditor);
					temp.Add(assemblySecondEditorAssembly);
					break;
				case CompilationFolder.Runtime:
					temp.Add(assemblyFirstPassRuntime);
					temp.Add(assemblySecondRuntimeAssembly);
					break;
				case CompilationFolder.Any:
					temp.Add(assemblyFirstPassRuntime);
					temp.Add(assemblySecondRuntimeAssembly);
					temp.Add(assemblyFirstPassEditor);
					temp.Add(assemblySecondEditorAssembly);
					break;
			}
			return isPartOfAssembly(absoluteFolderPath, temp);
		}

		bool isPartOfAssembly(string absoluteFolderPath, List<string> assembliesToLook)
		{
			//Directory.GetFiles(absoluteFolderPath, "*.cs", SearchOption.TopDirectoryOnly);
			var scripts = GetFiles(absoluteFolderPath, fileSearchPattern, SearchOption.TopDirectoryOnly);
			if(scripts.Length == 0) { return false; } //skip empty folders 
			string str = scripts[0].Replace('\\', '/');
			str = str.Substring(str.LastIndexOf('/') + 1, str.Length - str.LastIndexOf('/') - 4);
			Type type = AsmdefUtils.TypeUtility.GetTypeByName(str);

			//should only ever happen if the type doesn't exist(usually when it's a platform dependent and it doesn't exist on the current platform)
			if (type == null) { return false; }

			string assemblyToLookFor = type.Assembly.GetName().Name;
			bool result = assembliesToLook.Contains(type.Assembly.GetName().Name);

			if (database.showDebugMessages && database.showClassDebugMessages)
			{ Debug.Log(string.Format("Class({0}), InAssembly({1}), IsBaseAssembly({2})", str, assemblyToLookFor, result)); }
			return result;
		}

		void setAutoAssemblyDefinitionsFolder()
		{
			if(database.autoASMDEFAssembliesGUID == null) { database.autoASMDEFAssembliesGUID = new AsmdefManagerDatabase.AutoASMDEFAssembliesGUID(); }

			var platformList = new List<string>(){ "Editor" };

			if(firstPassRoots.Count > 0)
			{
				var temp = database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeRuntimeAssemblies);
				temp = temp.Distinct().ToList();
				database.autoASMDEFAssembliesGUID.firstPass = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefFirstPass), ref database.createdAsmdefFilesRuntime, temp);
			}

			if(firstPassEditors.Count > 0)
			{
				var temp = database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeEditorAssemblies);
				temp = temp.Distinct().ToList();
				if (firstPassRoots.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.firstPass)); }
				database.autoASMDEFAssembliesGUID.firstPassEditor = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefFirstPassEditor), ref database.createdAsmdefFilesRuntime, temp, includePlatforms: platformList);
			}

			if(secondPassRuntime.Count > 0)
			{
				var temp = database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeRuntimeAssemblies);
				temp = temp.Distinct().ToList();
				if (firstPassRoots.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.firstPass)); }
				database.autoASMDEFAssembliesGUID.secondPass = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefSecondPass), ref database.createdAsmdefFilesRuntime, temp);
			}

			if(secondPassEditor.Count > 0)
			{
				var temp = database.alwaysIncludeAssemblies.ToList();
				temp.AddRange(database.alwaysIncludeEditorAssemblies);
				temp = temp.Distinct().ToList();
				if (firstPassRoots.Count > 0) { temp.Add(AsmdefUtils.FormatGUID( database.autoASMDEFAssembliesGUID.firstPass)); }
				if (firstPassEditors.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.firstPassEditor)); }
				if (secondPassRuntime.Count > 0) { temp.Add(AsmdefUtils.FormatGUID(database.autoASMDEFAssembliesGUID.secondPass)); }
				database.autoASMDEFAssembliesGUID.secondPassEditor = createAsmdefPass(string.Format("{0}-{1}", customAsmdefPrefix, asmdefSecondPassEditor), ref database.createdAsmdefFilesRuntime, temp, includePlatforms: platformList);
			}

			//?????
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "FirstPass"));
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "FirstPassEditor"));
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "SecondPass"));
			database.createdAsmdefFiles.Add(string.Format("{0}-{1}", customAsmdefPrefix, "SecondPassEditor"));
		}

		List<string> errorMsgs = new List<string>();
		bool doNextStep = false;
		
		void createAsmdef()
		{
			doNextStep = false;
			errorMsgs.Clear();
			switch (database.sessionData.sessionProgress)
			{
				case AsmdefManagerDatabase.SessionProgress.WaitingForInput:
					lookForScripts();
					database.sessionData = new AsmdefManagerDatabase.SessionData(scriptsInAssets, firstPassRoots, firstPassEditors, secondPassRuntime, secondPassEditor, AsmdefManagerDatabase.SessionProgress.LookForPasses, true);
					doNextStep = true;
					break;
				case AsmdefManagerDatabase.SessionProgress.LookForPasses:
					lookForPasses();
					database.sessionData = new AsmdefManagerDatabase.SessionData(scriptsInAssets, firstPassRoots, firstPassEditors, secondPassRuntime, secondPassEditor, AsmdefManagerDatabase.SessionProgress.SelectAsmdefFolder, true);
					doNextStep = true;
					break;
				case AsmdefManagerDatabase.SessionProgress.SelectAsmdefFolder:
					database.baseAsmdefFolder = GetFirstValidBaseFolder(); //todo option to UPDATE
					database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.CreateAutoPassAssembliesAndReferences;
					doNextStep = true;
					break;
				case AsmdefManagerDatabase.SessionProgress.CreateAutoPassAssembliesAndReferences:
					database.createdAsmdefFiles.Clear();
					setAutoAssemblyDefinitionsFolder(); //create asmdef files and store a reference to them
				
					createAsmrefPass(firstPassRoots, ref database.createdAsmdefFilesRuntime, database.autoASMDEFAssembliesGUID.firstPass);
					createAsmrefPass(firstPassEditors, ref database.createdAsmdefFilesEditor, database.autoASMDEFAssembliesGUID.firstPassEditor);
					createAsmrefPass(secondPassRuntime, ref database.createdAsmdefFilesRuntime, database.autoASMDEFAssembliesGUID.secondPass);
					createAsmrefPass(secondPassEditor, ref database.createdAsmdefFilesEditor, database.autoASMDEFAssembliesGUID.secondPassEditor);
				
					database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.Finished;
					EditorUtility.SetDirty(database);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
					
					database.FindGUIDs();
					break;
				case AsmdefManagerDatabase.SessionProgress.Finished:
					database.sessionData.working = false;
					break;
				case AsmdefManagerDatabase.SessionProgress.Ready:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		bool isExistingFolderValid(string _baseFolder, string _folderName)
		{
			var validFolderNames = new List<string>
				{asmdefFirstPass, asmdefFirstPassEditor, asmdefSecondPass, asmdefSecondPassEditor};
			return validFolderNames.Any(s => _baseFolder + customAsmdefPrefix + "-" + s == _folderName);
		}

		public void recece()
		{
			var baseFolderPath = Application.dataPath + "/AutoASMDEF";
			database.autoASMDEFAssembliesGUID.isAnyInFolder(baseFolderPath.Substring(Application.dataPath.Length - 6));
		}

		private string GetFirstValidBaseFolder()
		{
			var baseFolderPath = Application.dataPath + "/AutoASMDEF";
			
			var exists = Directory.Exists(baseFolderPath);
			var index = 0;
			
			if (exists)
				if(IsDirectoryEmpty(baseFolderPath) || database.autoASMDEFAssembliesGUID.isAnyInFolder(baseFolderPath.Substring(Application.dataPath.Length - 6)))
					return baseFolderPath;
			while (true)
			{
				var tempName = baseFolderPath + index;
				if (Directory.Exists(tempName))
				{
					if (IsDirectoryEmpty(tempName) || database.autoASMDEFAssembliesGUID.isAnyInFolder(baseFolderPath.Substring(Application.dataPath.Length - 6)))
						return tempName;
				}
				else
					return tempName;
				index++;
			}
		}
		
		/// <summary>
		/// look up asmref files in the project, then remove the ones that were created with the tool
		/// </summary>
		public void RunCleanup()
		{
			LookUpAndRemoveASMDEFFiles<AsmdefObject>("*.asmdef");
			LookUpAndRemoveASMDEFFiles<AsmRefObject>("*.asmref");
			
			AssetDatabase.Refresh();
		}

		void LookUpAndRemoveASMDEFFiles<T>(string fileExtension) where T : IAutoAssemblyData
		{
			var files = Directory.GetFiles(Application.dataPath, fileExtension, SearchOption.AllDirectories);
			bool foundLeftoverFiles = false;
			foreach (var VARIABLE in files)
			{
				var path = VARIABLE.Replace("\\", "/");
				var sr = new StreamReader(path);
				var fileContents = sr.ReadToEnd();
				sr.Close();
				
				var temp = JsonUtility.FromJson<T>(fileContents);
				var GUID = AssetDatabase.AssetPathToGUID("Assets" + path.Substring(Application.dataPath.Length, path.Length - Application.dataPath.Length));
				if (temp.AdditionalData == "AutoASMDEF" && (database.createdAsmrefFilesGUID.All(s => s.GUID != GUID) && !database.autoASMDEFAssembliesGUID.containsGUID(GUID)))
				{
					foundLeftoverFiles = true;
					if(database.showDebugMessages)
						Debug.Log("[AutoASMDEF cleaning up leftover data] " + path);
					removeFileWithMeta(path);
				}

				var dirPath = path.Substring(0, path.LastIndexOf('/'));
				removeFileWithMeta(dirPath);
			}
			
			if(!foundLeftoverFiles && database.showDebugMessages)
				Debug.Log("[AutoASMDEF] Nothing to clean up.");
		}

		private bool IsDirectoryEmpty(string path)
		{
			return !Directory.EnumerateFileSystemEntries(path).Any();
		}
		
		void createAsmrefPass(List<string> passPaths, ref List<string> databasePassReference, string referencedAssemblyGUID, bool lookForBaseAssembly = true)
		{
			foreach (var item in passPaths)
			{
				var path = Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets")) + item;
				if (File.Exists(path))
				{
					database.createdAsmrefFiles.Add(item);
					continue;
				}
				var asref = new AsmRefObject("GUID:" + referencedAssemblyGUID, "AutoASMDEF");
				if (database.showDebugMessages)
				{
					Debug.Log(path);
				}
				
				File.WriteAllText( path, JsonUtility.ToJson(asref, true));

				//database.createdAsmrefFiles.Add(fullPath); //todo change this to GUID as well 
				database.createdAsmrefFiles.Add(item); //todo change this to GUID as well 
			}
		}

		string createAsmdefPass(string passName, ref List<string> databasePassReference, List<string> referencedPasses, List<string> includePlatforms = null, List<string> excludePlatforms = null)
		{
			//creates a subfolder named passName
			//creates an asmdef file named passName in that subfolder
			//references passes that will exist before this pass 
			var fullPath = database.baseAsmdefFolder + "/" + passName;
			if(!Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}

			/*var referencedAssemblies = new List<string>();
			referencedAssemblies.AddRange(database.alwaysIncludeAssemblies);
			if(referencedPasses != null)
			{
				referencedAssemblies.AddRange(referencedPasses);
			}*/
			string pathWithExtension = $"{fullPath}/{passName}.asmdef";
			if (File.Exists(pathWithExtension))
			{
				var _p = pathWithExtension.Substring(pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) , pathWithExtension.Length - (pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) ));
				return AssetDatabase.AssetPathToGUID(_p);
			}

			AsmdefObject asmdef = new AsmdefObject(passName, 
				referencedPasses.ToArray(), 
				includePlatforms == null ? null : includePlatforms.ToArray(), 
				excludePlatforms == null? null : excludePlatforms.ToArray(),
				_additionalData: "AutoASMDEF"
				);
			string data = JsonUtility.ToJson(asmdef, true);

			
			Debug.Log(pathWithExtension);
			File.WriteAllText(pathWithExtension, data);
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

			var p = pathWithExtension.Substring(pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) , pathWithExtension.Length - (pathWithExtension.LastIndexOf("Assets", StringComparison.Ordinal) ));
			var nameWithoutExtension = p.Remove(p.LastIndexOf('.'));
			
			//todo save the folder GUID as well
			
			return AssetDatabase.AssetPathToGUID(p);
		}

		
		//Creates a asmdef file in a custom folder with a reference to every auto-asmdef file
		void createAsmdefInCustomFolder( bool refresh = false)
		{

			string path = "";
			if(refresh)
			{
				path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(database.manualAsmdefFile);
				path = path.Replace('\\', '/');
				path = path.Substring(0, path.LastIndexOf('/') + 1);
			}
			else
			{
				path = EditorUtility.SaveFolderPanel("Save asmdef to folder", "", "");
			}

			List<string> referencedAssemblies = new List<string>();
			/*todo
			//some sort of error check
		 	Debug.Log(getAssemblyName(path));
			if (!isInBaseAssembly(path))
				referencedAssemblies.Add(getAssemblyName(path));
			*/
			foreach (var item in database.createdAsmdefFilesRuntime)
			{
				referencedAssemblies.Add(item);
			}

			List<string> includedPlatforms = new List<string>() {  };
			var name = string.Format("{0}-{1}", customAsmdefPrefix, "ManualRuntime");
			string data = AsmdefUtils.StringToASMDefFormat(name, referencedAssemblies, includePlatforms: includedPlatforms);
			
			File.WriteAllText(path + "/" + name + ".asmdef", data);

			database.manualAsmdefFile = name;

			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		}

		void revertCreatedFiles()
		{
			database.sessionData.working = false;
			/*for (int i = 0; i < database.createdAsmrefFiles.Count; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(database.createdAsmrefFilesGUID[i].GUID);// database.createdAsmrefFiles[i];
				removeFileWithMeta(path);
			}*/
			
			for (int i = 0; i < database.createdAsmrefFilesGUID.Count; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(database.createdAsmrefFilesGUID[i].GUID);// database.createdAsmrefFiles[i];
				removeFileWithMeta(path);
			}
			
			database.createdAsmrefFiles.Clear();
			database.createdAsmrefFilesGUID.Clear();
			//s.LastIndexOf( Application.dataPath) + Application.dataPath.Length - ("Assets".Length)
			var p =Application.dataPath.Remove(Application.dataPath.LastIndexOf( Application.dataPath, StringComparison.Ordinal) + Application.dataPath.Length - ("Assets".Length));// Application.dataPath.Remove(Application.dataPath.LastIndexOf("Assets"));

			var asmdefHolderFolder = new List<string>();
			var fp = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.firstPass);
			var fpe = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.firstPassEditor);
			var sp = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.secondPass);
			var spe = AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.secondPassEditor);

			var datapath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/";
			if (!string.Equals(fp, ""))
			{
				asmdefHolderFolder.Add(datapath + fp.Substring(0, fp.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + fp); //AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.firstPass));
			}
			if (!string.Equals(fpe, ""))
			{
				asmdefHolderFolder.Add(datapath + fpe.Substring(0, fpe.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + fpe);
			}
			if (!string.Equals(sp, ""))
			{
				asmdefHolderFolder.Add(datapath + sp.Substring(0, sp.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + sp);
			}
			if (!string.Equals(spe, ""))
			{
				asmdefHolderFolder.Add(datapath + spe.Substring(0, spe.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + spe);
			}

			foreach (var VARIABLE in asmdefHolderFolder)
			{
				var dirinfo = new DirectoryInfo(@VARIABLE);
				if(dirinfo.Exists && IsDirectoryEmpty(@VARIABLE))
					dirinfo.Delete(true);
			}

			database.autoASMDEFAssembliesGUID.firstPass = "";
			database.autoASMDEFAssembliesGUID.firstPassEditor = "";
			database.autoASMDEFAssembliesGUID.secondPass =  "";
			database.autoASMDEFAssembliesGUID.secondPassEditor = "";

			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

			database.sessionData.sessionProgress = AsmdefManagerDatabase.SessionProgress.WaitingForInput;
		}

		//p??
		//asmdefHolderFolder??
		void removeAsmdefWithFolder(string p, string datapath, string _asmdefGUID, ref List<string> asmdefHolderFolder)
		{
			var fp = AssetDatabase.GUIDToAssetPath(_asmdefGUID);
			if (!string.Equals(fp, ""))
			{
				asmdefHolderFolder.Add(datapath + fp.Substring(0, fp.LastIndexOf("/", StringComparison.Ordinal)));
				removeFileWithMeta(p + fp); //AssetDatabase.GUIDToAssetPath(database.autoASMDEFAssembliesGUID.firstPass));
			}
			
		}

		/// <summary>
		/// Removes files or folders with metadata. It only removes empty folders!
		/// </summary>
		/// <param name="fullPath"></param>
		void removeFileWithMeta(string fullPath)
		{
			if(database.showDebugMessages)
				Debug.Log(fullPath);

			if (fullPath != null)
			{
				if(File.Exists(fullPath))
					File.Delete(fullPath);
				if (Directory.Exists(fullPath) && IsDirectoryEmpty(fullPath))
					Directory.Delete(fullPath);
				if(File.Exists(fullPath + ".meta"))
					File.Delete(fullPath + ".meta");
			}
		}

		/// <summary>
		/// Returns folders that have the specified name and a *.cs file(either at the top or in a sub directory)
		/// </summary>
		/// <param name="rootFolder">Search start folder</param>
		/// <param name="folderNameToSearchFor">Folder name to look for</param>
		/// <returns></returns>
		List<string> lookForFolders(string rootFolder = "", string folderNameToSearchFor = "Editor", bool addEverySubdirectoryIfParentIsNamedFolderNameToSearchFor = false)
		{
			string sDataPath = rootFolder;
			List<string> listOfFolders = Directory.GetDirectories(sDataPath, folderNameToSearchFor, SearchOption.AllDirectories).Where(
				folder => Directory.EnumerateFiles(folder.Replace('\\', '/'), "*.cs", SearchOption.AllDirectories).Any()).ToList();

			if (addEverySubdirectoryIfParentIsNamedFolderNameToSearchFor)
			{
				var temp = new List<string>();
				foreach (var item in listOfFolders)
				{
					temp.AddRange(Directory.GetDirectories(item, "*", SearchOption.AllDirectories).Where(
				folder => Directory.EnumerateFiles(folder.Replace('\\', '/'), "*.cs", SearchOption.AllDirectories).Any()).ToList());
				}
				listOfFolders.AddRange(temp);
			}

			//just to make sure we have consistent separators everywhere
			for (int i = 0; i < listOfFolders.Count; i++)
			{
				listOfFolders[i] = listOfFolders[i].Replace('\\', '/');
				if(folderNameToSearchFor == editorFolders)
				{
					if (database.showDebugMessages) { Debug.Log(listOfFolders[i]); }
				}
				
			}
			return listOfFolders;
		}

		//Utility
		List<string> GetHighestSubPaths(List<string> input)
		{
			for (int i = 0; i < input.Count; i++)
			{
				input[i] = input[i].Replace('\\', '/');
			}

			input = input.OrderBy(x => x).ThenBy(x => x.Length).ToList();
			for (int i = 0; i < input.Count; i++)
			{
				if (input[i].LastIndexOf('/') + 1 < input[i].Length)
				{
					input[i] += '/';
				}
			}

			//if everything is in the same subfolder we won't have any matches so just return the formatted input string
			if(input.Count == 1)
			{
				return input;
			}

			var selectivePath = new List<string>();
			foreach (var @item in input)
			{
				string highest = GetHighestParent(input.Where(x => x.StartsWith(item.Substring(0, item.IndexOf('/') + 1))).ToList());
				if (highest.Length > highest.LastIndexOf('/') + 2)
				{
					highest = highest.Substring(0, highest.LastIndexOf('/') + 1);
				}
				selectivePath.Add(highest);
			}
			return selectivePath;
		}

		string GetHighestParent(List<string> list)
		{
			var MatchingChars =
				from len in Enumerable.Range(0, list.Min(s => s.Length)).Reverse()
				let possibleMatch = list.First().Substring(0, len + 1)
				where list.All(f => f.StartsWith(possibleMatch))
				select possibleMatch;
			var LongestDir = MatchingChars.First();

			return LongestDir;
		}
	}

	public enum CompilationFolder
	{
		Editor,
		Runtime,
		Any
	}
}