using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace TSD.AsmdefManagement
{
	/// <summary>
	/// Used for formatting input string to asmdef like syntax
	/// </summary>
	public class AsmdefUtils
	{
		private const string assemblyFirstPassRuntime = "Assembly-CSharp-firstpass";
		private const string assemblyFirstPassEditor = "Assembly-CSharp-Editor-firstpass";
		private const string assemblySecondEditorAssembly = "Assembly-CSharp-Editor";
		private const string assemblySecondRuntimeAssembly = "Assembly-CSharp";

		public enum CompilationFolder
		{
			Editor,
			Runtime,
			Any
		}

		public enum UnityPlatforms
		{
			Android,
			Editor,
			iOS,
			LinuxStandalone32,
			LinuxStandalone64,
			LinuxStandaloneUniversal,
			macOSStandalone,
			Nintendo3DS,
			PS4,
			PSVita,
			Switch,
			tvOS,
			WSA,
			WebGL,
			WindowsStandalone32,
			WindowsStandalone64,
			XboxOne
		}
		public static List<string> GetPlatforms(UnityPlatforms unityPlatforms)
		{
			return null;
		}

		/// <summary>
		/// Returns a formatted text used by Unity's asmdef files
		/// </summary>
		/// <returns></returns>
		public static string StringToASMDefFormat(string name, List<string> references = null, List<string> optionalUnityReferences = null, List<string> includePlatforms = null, List<string> excludePlatforms = null, bool allowUnsafeCode = false)
		{
			string result = "";
			string filename = name + ".asmdef";

			//name
			result = string.Format("\t'name': '{0}',", name);
			result += "\n";
			//references
			result += string.Format("\t'references': {0},", GetFormattedStringFromList(references));
			result += "\n";
			//optionalUnityReferences
			result += string.Format("\t'optionalUnityReferences': {0},", GetFormattedStringFromList(optionalUnityReferences));
			result += "\n";
			//includePlatforms
			result += string.Format("\t'includePlatforms': {0},", GetFormattedStringFromList(includePlatforms));
			result += "\n";
			//excludePlatforms
			result += string.Format("\t'excludePlatforms': {0},", GetFormattedStringFromList(excludePlatforms));
			result += "\n";
			//allowUnsafeCode
			result += string.Format("\t'allowUnsafeCode': {0}", allowUnsafeCode ? "true" : "false"); //it returns False or True instead of false or true, that's why 

			result = "{\n " + result + "\n}";
			result = result.Replace("'", "\"");
			return result;
		}

		static string GetFormattedStringFromList(List<string> input = null)
		{
			var str = "";
			if (input != null)
			{
				for (int i = 0; i < input.Count; i++)
				{
					str += string.Format("\t\t'{0}'{1}", input[i], i < input.Count - 1 ? ",\n" : "");
				}
				str = "[\n" + str + "\n\t]";
			}
			else { str = "[]"; }
			return str;
		}

		public class TypeUtility
		{
			public static Type GetTypeByName(string name)
			{
				foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (type.Name == name)
							return type;
					}
				}
				return null;
			}
		}
		/// <summary>
		/// Returns true if pathToCheck starts with any line of multilinePathsToCheckAgainst
		/// </summary>
		/// <param name="pathToCheck"></param>
		/// <param name="multilinePathsToCheckAgainst"></param>
		/// <returns></returns>
		public static bool isPathStartsWith(string pathToCheck, string multilinePathsToCheckAgainst)
		{
			string[] lines = Regex.Split(multilinePathsToCheckAgainst, "\n");
			pathToCheck = pathToCheck.Replace('\\', '/');
			foreach (var line in lines)
			{
				if (pathToCheck.StartsWith(string.Format("{0}/{1}", Application.dataPath, line)))
				{
					return true;
				}
			}
			return false;
		}

		public static bool isPathStartsWith(string path, List<string> pathsToCheckAgainst)
		{
			path = path.Replace('\\', '/');
			foreach (var item in pathsToCheckAgainst)
			{
				if (path.StartsWith(item))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// input as regex
		/// </summary>
		/// <param name="inputToSearchFor"></param>
		/// <param name="patternsToCheckAgainst"></param>
		/// <returns></returns>
		public static bool IsMatch(string inputToSearchFor, List<string> patternsToCheckAgainst)
		{
			foreach (var pattern in patternsToCheckAgainst)
			{
				if(pattern == "") { continue; }
				if (AsmdefManagerDatabase.Instance.CanUseRegex ? 
					Regex.IsMatch(inputToSearchFor, string.Format(@"{0}/{1}",Application.dataPath, pattern)) 
					: string.Equals(inputToSearchFor,string.Format(@"{0}/{1}",Application.dataPath, pattern) )) 
				{ return true; }
			}
			return false;
		}
		/*
		public bool isPathPartOfSpecialAssembly(string pathToCheck)
		{

		}*/

		public static string FormatGUID(string GUID)
		{
			if (GUID == null)
			{
				throw new ArgumentNullException(nameof(GUID));
			}

			return "GUID:" + GUID;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <param name="compilationFolder">Base assembly we are looking for</param>
		/// <param name="isLocalPath">Local means the <paramref name="filePath"/> is Assets/myFolder</param> instead of C:\Whatever\Assets\myFolder</param>
		/// <returns></returns>
		public static bool IsPartOfBaseAssembly(string filePath, CompilationFolder compilationFolder, bool isLocalPath = true)
		{
			if (!isLocalPath)
			{
				filePath = filePath.Substring(Application.dataPath.Length - 6).Replace('\\', '/');
			}
			var myAssName = GetAssembly(filePath);
			switch (compilationFolder)
			{
				case CompilationFolder.Editor:
					return string.Equals(assemblyFirstPassEditor, myAssName) || string.Equals(assemblySecondEditorAssembly, myAssName);
				case CompilationFolder.Runtime:
					return string.Equals(assemblyFirstPassRuntime, myAssName) || string.Equals(assemblySecondRuntimeAssembly, myAssName);
				case CompilationFolder.Any:
					return string.Equals(assemblyFirstPassEditor, myAssName) || string.Equals(assemblySecondEditorAssembly, myAssName) || string.Equals(assemblyFirstPassRuntime, myAssName) || string.Equals(assemblySecondRuntimeAssembly, myAssName);
				default:
					Debug.LogError("Did the CompilationFolder enum change? This message shouldn't ever pop up.");
					return false;
			}
		}

		public static bool IsPartOfAssembly(string filePath, string assemblyTolookFor)
		{
			return string.Equals(GetAssembly(filePath), assemblyTolookFor);
		}

		/// <summary>
		/// Returns the assembly Unity put the provided file in. Path must be local, with extension(Assets/myFile.cs)
		/// </summary>
		/// <param name="filePath">Local path with extension</param>
		/// <returns></returns>
		public static string GetAssembly(string filePath)
		{
			foreach (var item in LoadedAssemblies)
			{
				foreach (var item2 in item.sourceFiles)
				{
					if (string.Equals(item2, filePath))
					{
						return item.name;
					}
				}
			}
			return null;
		}

		static List<UnityEditor.Compilation.Assembly> _loadedAssemblies;
		static List<UnityEditor.Compilation.Assembly> LoadedAssemblies
		{
			get
			{
				if (_loadedAssemblies == null)
				{
					_loadedAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies().ToList();
				}
				return _loadedAssemblies;
			}
		}
	}

	internal class AsmdefObject : IAutoAssemblyData
	{
		public string name;
		public string[] references;
		public string[] includePlatforms;
		public string[] excludePlatforms;
		public bool allowUnsafeCode;
		public bool autoReferenced;
		public bool overrideReferences;
		public string[] precompiledReferences;
		public string[] defineConstraints;
		public string[] optionalUnityReferences;
		public string additionalData;

		public AsmdefObject(
			string _name,
			string[] _references = null,
			string[] _includePlatforms = null,
			string[] _excludePlatforms = null,
			bool _allowUnsafeCode = false,
			bool _autoReferenced = true,
			bool _overrideReferences = false,
			string[] _precompiledReferences = null,
			string[] _defineConstraints = null,
			string[] _optionalUnityReferences = null,
			string _additionalData = null)
		{
			name = _name;
			references = _references;
			includePlatforms = _includePlatforms;
			excludePlatforms = _excludePlatforms;
			allowUnsafeCode = _allowUnsafeCode;
			autoReferenced = _autoReferenced;
			overrideReferences = _overrideReferences;
			precompiledReferences = _precompiledReferences;
			defineConstraints = _defineConstraints;
			optionalUnityReferences = _optionalUnityReferences;
			additionalData = _additionalData;
		}
		
		public string AdditionalData { get=>additionalData; set=>additionalData = value; }
	}

	internal class AsmRefObject : IAutoAssemblyData
	{
		public string reference;
		public string _additionalData;

		public AsmRefObject(string reference, string additionalData)
		{
			this.reference = reference;
			this.AdditionalData = additionalData;
		}

		public string AdditionalData { get=>_additionalData; set=>_additionalData = value; }
	}

	public interface IAutoAssemblyData
	{
		string AdditionalData { get; set; }
	}
}