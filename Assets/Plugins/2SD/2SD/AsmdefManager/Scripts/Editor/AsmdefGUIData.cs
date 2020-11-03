using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

namespace TSD.AsmdefManagement
{
	/// <summary>
	/// Provides access to GUI related data(color, text, whatnot)
	/// </summary>
	public partial class AsmdefManagerEditorWindow
	{
		public void DrawColoredRectangle(Rect position, Color color, int horizontalOffset = 0)
		{
			Rect offsetRect = new Rect(position);
			offsetRect.height = offsetRect.height - 2;
			offsetRect.y += 2;
			offsetRect.x += horizontalOffset;

			
		/*	Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, color);
			texture.Apply();
			*/
			var texture = TstyleBoxEnabled;
			var originalTexture = GUI.skin.box.normal.background;
			GUI.skin.box.normal.background = texture;
			GUI.Box(offsetRect, GUIContent.none);
			GUI.skin.box.normal.background = originalTexture;
		}

		static Color textProColor = new Color(.1f, .1f, .1f);
		static Color textNonProColor = new Color(0.11f, 0.11f, 0.11f);

		public static Color textColor
		{
			get { return EditorGUIUtility.isProSkin ? textProColor : textNonProColor; }
		}

		static Color colorProManualAss = new Color(.25f, .45f, .71f);
		static Color colorNonProManualAss = Color.gray;

		public static Color colorInManualAssembly
		{
			get { return EditorGUIUtility.isProSkin ? colorProManualAss : colorNonProManualAss; }
		}

		static Color colorProIssue = new Color(1f, .8f, .0f);
		static Color colorNonProIssue = Color.gray;

		public static Color colorIssue
		{
			get { return EditorGUIUtility.isProSkin ? colorProIssue : colorNonProIssue; }
		}


		static Color toggleProColor = new Color(.1f, .1f, .1f);
		static Color toggleNonProColor = Color.gray;

		public Color toggleColor
		{
			get { return EditorGUIUtility.isProSkin ? toggleProColor : toggleNonProColor; }
		}

		static GUIStyle styleTextAlignedMiddle;

		public GUIStyle StyleTextAlignedMiddle
		{
			get
			{
				if (styleTextAlignedMiddle == null)
				{
					styleTextAlignedMiddle = new GUIStyle(GUI.skin.label);
					styleTextAlignedMiddle.alignment = TextAnchor.MiddleCenter;
				}

				return styleTextAlignedMiddle;
			}
		}

		public void LoadTextures()
		{
			
			ColorConnection styleBox = new ColorConnection(new Color(.0f, .45f, 1f, 1f), new Color(.0f, .2f, .5f, 1f));
			
			ReadSetupData();
		//	localResourcesFolder = "Assets/2SD/2SD/AsmdefManager/Scripts/Editor/Resources/";
			BtnBackTexture = LoadTextureAsset<Texture>("TSD_Icon_back1");
//				AssetDatabase.LoadAssetAtPath(localResourcesFolder + "Auto-ASMDEF/TSD_Icon_back1.png",
//					typeof(Texture)) as Texture;
			BtnForwardTexture = LoadTextureAsset<Texture>("TSD_Icon_forward1");
			BtnHierarchyTexture = LoadTextureAsset<Texture>("TSD_Icon_hierarchy");
			BtnReferencesTexture = LoadTextureAsset<Texture>("TSD_Icon_reference");
			BtnSettingsTexture = LoadTextureAsset<Texture>("TSD_Icon_settings");
			BtnTextboxTexture = LoadTextureAsset<Texture>("TSD_Icon_textbox");
			BtnYoutubeTexture = LoadTextureAsset<Texture>("TSD_Icon_youtube");
			BtnRescanTexture = LoadTextureAsset<Texture>("TSD_Icon_refresh");
			//	BtnToggleON				= AssetDatabase.LoadAssetAtPath(localResourcesFolder + "Auto-ASMDEF/TSD_Icon_toggle_ON.png", typeof(Texture)) as Texture;
			//	BtnToggleOff			= AssetDatabase.LoadAssetAtPath(localResourcesFolder + "Auto-ASMDEF/TSD_Icon_toggle_OFF.png", typeof(Texture)) as Texture;

			tLogo = LoadTextureAsset<Texture>("TSD_inspector_logo");

			TstyleBoxEnabled = LoadTextureAsset<Texture2D>("TSD_styleBoxEnabled", true);
			TstyleBoxDisabled = LoadTextureAsset<Texture2D>("TSD_styleBoxDisabled", true);
			TstyleBoxCheckbox = LoadTextureAsset<Texture2D>( "TSD_styleBoxCheckbox.png", true);
			
		//	var skin = AssetDatabase.LoadAssetAtPath(
		//		"Assets/2SD/2SD/AsmdefManager/Scripts/Editor/Resources/" + "Auto-ASMDEF/TSD_auto_asmdef_GUISkin.guiskin", typeof(GUISkin)) as GUISkin;
		var skin = AssetDatabase.LoadAssetAtPath(
			localResourcesFolder + "Auto-ASMDEF/TSD_auto_asmdef_GUISkin.guiskin", typeof(GUISkin)) as GUISkin;
			loadedCheckbox = skin.toggle;
		}

		T LoadTextureAsset<T>(string localPath, bool lookForIndieVersion = false) where T : Object
		{
			string indie = !EditorGUIUtility.isProSkin && lookForIndieVersion? "_indie" : "";
			return AssetDatabase.LoadAssetAtPath($"{localResourcesFolder}Auto-ASMDEF/{localPath}{indie}.png", typeof(T)) as T;
		}

		private Texture BtnBackTexture;
		private Texture BtnForwardTexture;
		private Texture BtnHierarchyTexture;
		private Texture BtnReferencesTexture;
		private Texture BtnSettingsTexture;
		private Texture BtnTextboxTexture;
		private Texture BtnYoutubeTexture;

		private Texture BtnRescanTexture;

		private Texture2D TstyleBoxEnabled;
		private Texture2D TstyleBoxDisabled;
		private Texture2D TstyleBoxCheckbox;
		//	public static Texture BtnToggleON;
		//	public static Texture BtnToggleOff;

		private Texture tLogo;

		public int BtnSizeBig { get; set; } = 76;
		public int BtnSizeMed { get; set; } = 48;

		#region gui styles
		ColorConnection styleBox = new ColorConnection(new Color(.0f, .45f, 1f, 1f), new Color(.0f, .2f, .5f, 1f));
	//	static GUIStyle styleBoxCheckbox;

		
		public static GUIStyle loadedCheckbox { get; private set; }

		private GUIStyle sBoxCheckbox
		{
			get
			{
				styleBoxCheckbox = new GUIStyle(GUI.skin.toggle);
				Texture2D normalBg = TstyleBoxDisabled;// MakeTex(2, 2, styleBox.Color);
				styleBoxCheckbox.normal.background = TstyleBoxDisabled;
				styleBoxCheckbox.active.background = TstyleBoxEnabled;// MakeTex(2, 2, new Color(.0f, .55f, 1f, 1f));
				styleBoxCheckbox.hover.background  = TstyleBoxEnabled; 

				styleBoxCheckbox.onNormal.background = normalBg;
				styleBoxCheckbox.onHover.background	 = normalBg;
				styleBoxCheckbox.onActive.background = normalBg;

				styleBoxCheckbox.onNormal.textColor	= textColor;
				styleBoxCheckbox.onActive.textColor	= textColor;
				styleBoxCheckbox.onHover.textColor	= textColor;
				styleBoxCheckbox.alignment = TextAnchor.MiddleCenter;

				return styleBoxCheckbox;
			}
		}

		private Texture2D MakeTex(int width, int height, Color col)
		{
			return null;
			Color[] pix = new Color[width * height];
			for (int i = 0; i < pix.Length; ++i)
			{
				pix[i] = col;
			}
			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}

		#endregion

		/// <summary>
		/// Returns either the pro or indie color scheme
		/// </summary>
		class ColorConnection
		{
			public Color proColor = new Color(.1f, .1f, .1f);
			public Color nonProColor = Color.gray;

			public Color Color
			{
				get
				{
					return EditorGUIUtility.isProSkin ? proColor : nonProColor;
				}
			}

			public ColorConnection(Color _pro, Color _free)
			{
				proColor = _pro;
				nonProColor = _free;
			}
		}

		private string m_ScriptFilePath;
		private string localResourcesFolder;
		private void ReadSetupData()
		{
			MonoScript ms = MonoScript.FromScriptableObject(AsmdefManagerDatabase.Instance);
			m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

			FileInfo fi = new FileInfo(m_ScriptFilePath);
			localResourcesFolder = fi.Directory.ToString();
			
			localResourcesFolder = localResourcesFolder.Replace('\\', '/');
			
			localResourcesFolder += "/Resources/";
			localResourcesFolder = localResourcesFolder.Substring(Application.dataPath.Length - 6);

		//	Debug.Log(localResourcesFolder);
		}
	}
}