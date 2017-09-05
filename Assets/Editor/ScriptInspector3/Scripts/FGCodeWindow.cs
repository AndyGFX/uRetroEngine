﻿/* SCRIPT INSPECTOR 3
 * version 3.0.18, May 2017
 * Copyright © 2012-2017, Flipbook Games
 * 
 * Unity's legendary editor for C#, UnityScript, Boo, Shaders, and text,
 * now transformed into an advanced C# IDE!!!
 * 
 * Follow me on http://twitter.com/FlipbookGames
 * Like Flipbook Games on Facebook http://facebook.com/FlipbookGames
 * Join discussion in Unity forums http://forum.unity3d.com/threads/138329
 * Contact info@flipbookgames.com for feedback, bug reports, or suggestions.
 * Visit http://flipbookgames.com/ for more info.
 */


namespace ScriptInspector
{

using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
	
	
	static class GenericMenuExtensions
	{
		private static string DecodeWinKey(string key)
		{
			if (key == "")
				return "";
			
			bool alt = false;
			bool ctrl = false;
			bool shift = false;
			
			string result = "\t";
			for (int i = 0; i < key.Length; i++)
			{
				switch (key[i])
				{
				case '&':
					alt = true;
					continue;
				case '%':
					ctrl = true;
					continue;
				case '#':
					shift = true;
					continue;
				default:
					if (alt)
						result += "Alt+";
					if (ctrl)
						result += "Ctrl+";
					if (shift)
						result += "Shift+";
					result += key.Substring(i).ToUpperInvariant();
					break;
				}
				break;
			}
			return result;
		}
		
		public static void AddItem(
			this GenericMenu menu,
			string osxText, string osxKey,
			string winText, string winKey,
			bool on,
			GenericMenu.MenuFunction func)
		{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			if (Application.platform == RuntimePlatform.OSXEditor)
				menu.AddItem(new GUIContent(osxText + " _" + osxKey), on, func);
			else
				menu.AddItem(new GUIContent(winText + " _" + winKey), on, func);
#else
			if (Application.platform == RuntimePlatform.OSXEditor)
				menu.AddItem(new GUIContent(osxText), on, func);
			else
				menu.AddItem(new GUIContent(winText + DecodeWinKey(winKey)), on, func);
#endif
		}
		
		public static void AddItem(
			this GenericMenu menu,
			string osxText, string osxKey,
			string winText, string winKey,
			bool on,
			GenericMenu.MenuFunction2 func,
			object userData)
		{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			if (Application.platform == RuntimePlatform.OSXEditor)
				menu.AddItem(new GUIContent(osxText + " _" + osxKey), on, func, userData);
			else
				menu.AddItem(new GUIContent(winText + " _" + winKey), on, func, userData);
#else
			if (Application.platform == RuntimePlatform.OSXEditor)
				menu.AddItem(new GUIContent(osxText), on, func, userData);
			else
				menu.AddItem(new GUIContent(winText + DecodeWinKey(winKey)), on, func, userData);
#endif
		}
	}

[System.Serializable, StructLayout(LayoutKind.Sequential), InitializeOnLoad]
public class FGCodeWindow : EditorWindow
#if !UNITY_3_5 && !UNITY_4_0
	, IHasCustomMenu
#endif
{
	[SerializeField, HideInInspector]
	private Object targetAsset;

	[SerializeField, HideInInspector]
	private string targetAssetGuid;
	public string TargetAssetGuid {
		get { return targetAssetGuid; }
	}

	private static Object useTargetAsset = null;

	[SerializeField, HideInInspector]
	private FGTextEditor textEditor = new FGTextEditor();
	public FGTextEditor TextEditor {
		get { return textEditor; }
	}

	[System.NonSerialized]
	private int pingLineWhenLoaded = -1;

	[System.NonSerialized]
	private int setCursorLineWhenLoaded = -1;
	[System.NonSerialized]
	private int setCursorCharacterIndexWhenLoaded = -1;
	[System.NonSerialized]
	private int setSelectionLengthWhenLoaded = 0;
	
	private static HashSet<FGCodeWindow> codeWindows = new HashSet<FGCodeWindow>();
	internal static HashSet<FGCodeWindow> CodeWindows {
		get { return codeWindows; }
	}
	
	private static string defaultDockNextTo;
	private static Rect defaultPosition;
	private static bool prefsLoaded;
	
	[System.NonSerialized]
	private object savedParent;
	
	private static class API
	{
		public static System.Type containerWindowType;
		public static System.Type viewType;
		public static System.Type dockAreaType;
		public static System.Type splitViewType;
		public static System.Type mainWindowType;
		public static FieldInfo panesField;
		public static PropertyInfo windowsField;
		public static PropertyInfo mainViewField;
		public static PropertyInfo allChildrenField;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		public static PropertyInfo cachedTitleContentField;
#endif
		public static MethodInfo addTabMethod;
		public static FieldInfo parentField;
		public static PropertyInfo parentProperty;
	//	public static EditorApplication.CallbackFunction windowsReordered;
		public static MethodInfo createAssetMethod;
		public static System.Type windowLayoutType;
		public static MethodInfo isMaximizedMethod;
		public static MethodInfo maximizeMethod;
		public static MethodInfo unMaximizeMethod;
		
		static API()
		{
			const BindingFlags instanceMember = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			const BindingFlags staticMember = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			
			var editorAssembly = typeof(EditorWindow).Assembly;
			
			containerWindowType = editorAssembly.GetType("UnityEditor.ContainerWindow");
			viewType = editorAssembly.GetType("UnityEditor.View");
			dockAreaType = editorAssembly.GetType("UnityEditor.DockArea");
			splitViewType = editorAssembly.GetType("UnityEditor.SplitView");
			mainWindowType = editorAssembly.GetType("UnityEditor.MainWindow")
				?? editorAssembly.GetType("UnityEditor.MainView");
			windowLayoutType = editorAssembly.GetType("UnityEditor.WindowLayout");
			
			parentField = typeof(EditorWindow).GetField("m_Parent", instanceMember);
			if (viewType != null)
				parentProperty = viewType.GetProperty("parent", instanceMember);
			if (dockAreaType != null)
			{
				panesField = dockAreaType.GetField("m_Panes", instanceMember);
				addTabMethod = dockAreaType.GetMethod("AddTab", new System.Type[] { typeof(EditorWindow) });
			}
			if (containerWindowType != null)
			{
				windowsField = containerWindowType.GetProperty("windows", staticMember);
				mainViewField = containerWindowType.GetProperty("mainView", instanceMember)
					?? containerWindowType.GetProperty("rootView", instanceMember);
			}
			if (viewType != null)
				allChildrenField = viewType.GetProperty("allChildren", instanceMember);
			
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
			cachedTitleContentField = typeof(EditorWindow).GetProperty("cachedTitleContent", instanceMember);
#endif
				
			System.Type projectWindowUtilType = editorAssembly.GetType("UnityEditor.ProjectWindowUtil");
			if (projectWindowUtilType != null)
				createAssetMethod = projectWindowUtilType.GetMethod("CreateAsset", new System.Type[] { typeof(Object), typeof(string) });

			if (windowLayoutType != null)
			{
				isMaximizedMethod = windowLayoutType.GetMethod("IsMaximized", staticMember);
				maximizeMethod = windowLayoutType.GetMethod("Maximize", staticMember);
				unMaximizeMethod = windowLayoutType.GetMethod("Unmaximize", staticMember);
				if (isMaximizedMethod == null || maximizeMethod == null || unMaximizeMethod == null)
					windowLayoutType = null;
			}
		}

		public static bool CreateAsset(Object asset, string pathName)
		{
			if (createAssetMethod == null)
				createAssetMethod.Invoke(null, new object[] { asset, pathName });
			return createAssetMethod == null;
		}
	};
	
	static FGCodeWindow()
	{
		EditorApplication.update -= InitOnLoad;
		EditorApplication.update += InitOnLoad;
	}
	
	private static void LoadPrefs()
	{
		prefsLoaded = true;
		
		var s = EditorPrefs.GetString("ScriptInspectorRecentGUIDs", "");
		var old = guidHistory;
		guidHistory = s.Split(new[] { ';' }, 50, System.StringSplitOptions.RemoveEmptyEntries).ToList();
		for (var i = old.Count; i --> 0; )
			AddMostRecentGuidHistory(old[i]);
		
		defaultDockNextTo = EditorPrefs.GetString("ScriptInspectorDefaultDockNextTo", "");
		if (defaultDockNextTo == "")
			defaultDockNextTo = null;
		
		defaultPosition = new Rect(
			EditorPrefs.GetFloat("ScriptInspectorDefaultPositionX", 100f),
			EditorPrefs.GetFloat("ScriptInspectorDefaultPositionY", 100f),
			Mathf.Max(400f, EditorPrefs.GetFloat("ScriptInspectorDefaultPositionW", 600f)),
			Mathf.Max(100f, EditorPrefs.GetFloat("ScriptInspectorDefaultPositionH", 380f))
		);
	}

	private static void InitOnLoad()
	{
		EditorApplication.update -= InitOnLoad;
		EditorApplication.projectWindowItemOnGUI -= OnProjectItemGUI;
		EditorApplication.projectWindowItemOnGUI += OnProjectItemGUI;

		if (!prefsLoaded)
			LoadPrefs();
		
		FGConsole.OpenIfConsoleIsOpen();
	}

	private static void OnProjectItemGUI(string item, Rect selectionRect)
	{
		bool alwaysOpenInSI = false;
		bool dontOpenInSI = false;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
		alwaysOpenInSI = SISettings.handleOpenAssets;
		dontOpenInSI = SISettings.dontOpenAssets;
#endif
		
		if (string.IsNullOrEmpty(item))
			return;

		if (Event.current.isMouse)
		{
			if (Event.current.type != EventType.MouseDown || Event.current.clickCount != 2 || Event.current.button != 0)
				return;
			
			if (selectionRect.height < 20f)
				selectionRect.xMin = 0f;
			if (!selectionRect.Contains(Event.current.mousePosition))
				return;
		}
		else
		{
			return;
		}

		string path = AssetDatabase.GUIDToAssetPath(item);
		if (path.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase) ||
			path.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		
		bool actionKey = EditorGUI.actionKey;
		
		if (!path.EndsWith(".cs", System.StringComparison.OrdinalIgnoreCase) &&
			!path.EndsWith(".js", System.StringComparison.OrdinalIgnoreCase) &&
			!path.EndsWith(".boo", System.StringComparison.OrdinalIgnoreCase))
		{
			// not a script

			Shader shader = null;
			TextAsset txt = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			if (txt == null)
				shader = AssetDatabase.LoadAssetAtPath(path, typeof(Shader)) as Shader;
			
			if (shader == null && txt == null)
				return;
			
			if (actionKey)
				Selection.objects = new Object[] { (Object)txt ?? shader };
			
			bool openTextAssets = !dontOpenInSI && (alwaysOpenInSI || SISettings.handleOpeningText);
			bool openShaders = !dontOpenInSI && (alwaysOpenInSI || SISettings.handleOpeningShaders);
			if (txt != null && openTextAssets != actionKey ||
				shader != null && openShaders != actionKey)
			{
				Event.current.Use();
				OpenAssetInTab(item);
				GUIUtility.ExitGUI();
			}
			
			return;
		}

		MonoScript script = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
		if (script != null)
		{
			if (actionKey)
			{
				Selection.objects = new Object[] { script };
			}
			
			bool openScripts = !dontOpenInSI && (alwaysOpenInSI || SISettings.handleOpeningScripts);
			if (openScripts == actionKey)
				return;
			
			Event.current.Use();
			OpenAssetInTab(item);
			GUIUtility.ExitGUI();
		}
	}

	private static string MaterialContextMenuToShaderGUID(MenuCommand mc)
	{
		Material target = mc.context as Material;
		if (target == null)
			return null;
		if (target.shader == null)
			return null;
		int shaderID = target.shader.GetInstanceID();
		if (shaderID == 0)
			return null;
		string assetPath = AssetDatabase.GetAssetPath(shaderID);
		if (string.IsNullOrEmpty(assetPath))
			return null;
		return AssetDatabase.AssetPathToGUID(assetPath);
	}

	[MenuItem("CONTEXT/Material/Edit in Script Inspector...", true, 101)]
	private static bool ValidateEditMaterialShader(MenuCommand mc)
	{
		return !string.IsNullOrEmpty(MaterialContextMenuToShaderGUID(mc));
	}

	[MenuItem("CONTEXT/Material/Edit in Script Inspector...", false, 101)]
	private static void EditMaterialShader(MenuCommand mc)
	{
		string guid = MaterialContextMenuToShaderGUID(mc);
		if (!string.IsNullOrEmpty(guid))
		{
			addRecentLocationForNextAsset = true;
			OpenAssetInTab(guid);
		}
	}

	[MenuItem("CONTEXT/MonoBehaviour/Edit in Script Inspector", false, 610)]
	private static void OpenBehaviourScript(MenuCommand mc)
	{
		MonoBehaviour target = mc.context as MonoBehaviour;
		if (target == null)
			return;
		var monoScript = MonoScript.FromMonoBehaviour(target);
		if (monoScript == null)
			return;
		string scriptPath = AssetDatabase.GetAssetPath(monoScript);
		if (!string.IsNullOrEmpty(scriptPath))
		{
			if (scriptPath.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase) ||
				scriptPath.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			addRecentLocationForNextAsset = true;
			OpenAssetInTab(AssetDatabase.AssetPathToGUID(scriptPath));
		}
	}
	
	[MenuItem("Assets/Create/Text", false, 90)]
	public static void CreateTextAsset()
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
		if (!AssetDatabase.Contains(Selection.activeInstanceID))
			path = "Assets";

		if (!System.IO.Directory.Exists(path))
			path = System.IO.Path.GetDirectoryName(path);
		path = System.IO.Path.Combine(path, "New Text.txt");
		path = AssetDatabase.GenerateUniqueAssetPath(path);

		System.IO.StreamWriter writer = System.IO.File.CreateText(path);
		writer.Close();
		writer.Dispose();

		AssetDatabase.ImportAsset(path);
		Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
	}
	
	public static void RepaintAllWindows()
	{
		foreach (FGCodeWindow wnd in codeWindows)
			if (wnd)
				wnd.Repaint();
	}
	
	public static bool addRecentLocationForNextAsset;
	
	public static bool openInExternalIDE;
	
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1
	[UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
	public static bool OnOpenAsset(int instanceID, int line)
	{
		var allowInvert = Event.current.keyCode != KeyCode.DownArrow;
		
		if (openInExternalIDE || (allowInvert && EditorGUI.actionKey ? SISettings.handleOpenAssets : SISettings.dontOpenAssets))
		{
			openInExternalIDE = false;
			return false;
		}
		
		if (allowInvert && EditorGUI.actionKey ? SISettings.dontOpenAssets : SISettings.handleOpenAssets)
		{
			var asset = EditorUtility.InstanceIDToObject(instanceID);
			if (asset is MonoScript || asset is TextAsset || asset is Shader)
			{
				var assetPath = AssetDatabase.GetAssetPath(instanceID);
				if (assetPath.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase) ||
					assetPath.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				var guid = AssetDatabase.AssetPathToGUID(assetPath);
				addRecentLocationForNextAsset = true;
				OpenAssetInTab(guid, line);
				return true;
			}
		}
		return false;
	}
#endif
	
	public static FGCodeWindow OpenAssetInTab(string guid)
	{
		return OpenAssetInTab(guid, -1, -1, 0);
	}
	
	public static FGCodeWindow OpenAssetInTab(string guid, int line)
	{
		return OpenAssetInTab(guid, line, -1, 0);
	}
	
	public static FGCodeWindow OpenAssetInTab(string guid, int line, int characterIndex)
	{
		return OpenAssetInTab(guid, line, characterIndex, 0);
	}
	
	public static FGCodeWindow OpenAssetInTab(string guid, int line, int characterIndex, int length)
	{
		FGTextEditor.nextF12GoesBack = false;
		
	startAgain:
		foreach (var codeWindow in codeWindows)
		{
			if (codeWindow && codeWindow.targetAsset == null)
			{
				codeWindows.Remove(codeWindow);
				try { codeWindow.Close(); } catch {}
				goto startAgain;
			}
		}
		
		foreach (FGCodeWindow codeWindow in codeWindows)
		{
			if (codeWindow && codeWindow.textEditor.targetGuid == guid)
			{
				codeWindow.Show();
				codeWindow.Focus();
				if (characterIndex < 0)
				{
					if (line >= 0)
					{
						codeWindow.PingLine(line);
					}
				}
				else if (line >= 0)
				{
					codeWindow.SetCursorPosition(line, characterIndex);
					codeWindow.setSelectionLengthWhenLoaded = length;
				}
				
				if (addRecentLocationForNextAsset)
					codeWindow.textEditor.AddRecentLocation(1, false);
				addRecentLocationForNextAsset = false;
				
				return codeWindow;
			}
		}
		
		string path = AssetDatabase.GUIDToAssetPath(guid);
		Object target = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
		if (target == null)
		{
			target = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
			if (target == null)
			{
				target = AssetDatabase.LoadAssetAtPath(path, typeof(Shader)) as Shader;
				if (target == null)
					return null;
			}
		}
		
		var recentTab = GetMostRecentlyActive(guid);
		FGCodeWindow newWindow = OpenNewWindow(target, recentTab, true);
		if (newWindow != null)
		{
			if (characterIndex < 0)
			{
				if (line >= 0)
				{
					newWindow.PingLine(line);
				}
			}
			else if (line >= 0)
			{
				newWindow.SetCursorPosition(line, characterIndex);
				newWindow.setSelectionLengthWhenLoaded = length;
			}
			
			if (addRecentLocationForNextAsset)
				newWindow.textEditor.AddRecentLocation(1, false);
			addRecentLocationForNextAsset = false;
		}
		return newWindow;
	}
	
	private void PingLine(int line)
	{
		pingLineWhenLoaded = line;
		EditorApplication.update -= PingLineWhenLoaded;
		EditorApplication.update += PingLineWhenLoaded;
	}
	
	private void SetCursorPosition(int line, int characterIndex)
	{
		setCursorLineWhenLoaded = line;
		setCursorCharacterIndexWhenLoaded = characterIndex;
		setSelectionLengthWhenLoaded = 0;
		EditorApplication.update -= SetCursorWhenLoaded;
		EditorApplication.update += SetCursorWhenLoaded;
	}

	public static FGCodeWindow OpenNewWindow(Object target, FGCodeWindow nextTo, bool reuseExisting)
	{
		FGTextEditor.nextF12GoesBack = false;
		
		if (focusedWindow && IsMaximized(focusedWindow))
			ToggleMaximized(focusedWindow);
		
		UnhideSi3Tabs();
		
		if (reuseExisting || target == null)
		{
			if (target == null)
				target = Selection.activeObject as MonoScript;
			if (target == null)
				target = Selection.activeObject as TextAsset;
			if (target == null)
				target = Selection.activeObject as Shader;
			if (target == null)
				return null;
			
			var assetPath = AssetDatabase.GetAssetPath(target);
			if (assetPath.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase) ||
				assetPath.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			
			string guid = AssetDatabase.AssetPathToGUID(assetPath);
			
			foreach (FGCodeWindow codeWindow in codeWindows)
			{
				if (codeWindow && codeWindow.targetAssetGuid == guid)
				{
					codeWindow.Focus();
					return codeWindow;
				}
			}
		}

		useTargetAsset = target;
		FGCodeWindow window = ScriptableObject.CreateInstance<FGCodeWindow>();
		useTargetAsset = null;

		if (!window.TryDockNextToSimilarTab(nextTo))
		{
			var rc = defaultPosition;
			rc.y -= 5f;
			window.position = rc;
			window.Show();
			window.position = rc;
		}

		window.Focus();
		return window;
	}
	
	private bool TryDockNextToSimilarTab(EditorWindow nextTo)
	{
		return DockNextTo(this, nextTo);
	}
	
	public static bool DockNextTo(EditorWindow dockThis, EditorWindow nextTo)
	{
		if (API.windowsField == null || API.mainViewField == null || API.panesField == null || API.addTabMethod == null)
			return false;
		
		System.Array windows = API.windowsField.GetValue(null, null) as System.Array;
		if (windows == null)
			return false;
		
		object nextToView = null;
		object defaultView = null;
		object siView = null;

		if (nextTo != null && API.parentField != null)
		{
			var parent = API.parentField.GetValue(nextTo);
			if (parent != null && parent.GetType() == API.dockAreaType)
				nextToView = parent;
		}
		
		foreach (var window in windows)
		{
			if (nextToView != null)
				break;

			var mainView = API.mainViewField.GetValue(window, null);
			var allChildren = API.allChildrenField.GetValue(mainView, null) as System.Array;
			if (allChildren == null)
				continue;

			foreach (var view in allChildren)
			{
				if (view.GetType() != API.dockAreaType)
					continue;

				var panes = API.panesField.GetValue(view) as List<EditorWindow>;
				if (panes == null)
					continue;

				if (nextTo != null && panes.Contains(nextTo))
				{
					nextToView = view;
					break;
				}
				if (defaultView == null && defaultDockNextTo != null)
				{
					if (panes.Any(pane => pane.GetType().ToString() == defaultDockNextTo))
						defaultView = view;
					if (panes.Any(pane => pane is FGCodeWindow))
						siView = view;
				}
				if (siView == null && panes.Any(pane => pane is FGCodeWindow))
				{
					siView = view;
				}
			}
		}
		
		var addToView = nextToView ?? siView ?? defaultView;
		if (addToView == null)
			return false;
		
		API.addTabMethod.Invoke(addToView, new object[] { dockThis });
		return true;
	}
	
	private List<EditorWindow> GetTabsInDockArea()
	{
		var parent = API.parentField.GetValue(this) ?? savedParent;
		if (parent == null || parent.GetType() != API.dockAreaType)
			return null;
		
		return API.panesField.GetValue(parent) as List<EditorWindow>;
	}
	
	private FGCodeWindow GetAdjacentCodeTab(bool right)
	{
		List<EditorWindow> panes = GetTabsInDockArea();
		if (panes == null)
			return null;

		int index = panes.FindIndex(wnd => wnd == this);
		if (index < 0)
			return null;

		if (right)
		{
			if (index + 1 < panes.Count)
				index = panes.FindIndex(index + 1, wnd => wnd is FGCodeWindow);
			else
				index = -1;
		}
		else
		{
			if (index > 0)
				index = panes.FindLastIndex(index - 1, wnd => wnd is FGCodeWindow);
			else
				index = -1;
		}
		if (index >= 0)
			return panes[index] as FGCodeWindow;
		
		return null;
	}
	
	private void SelectAdjacentCodeTab(bool right)
	{
		FGCodeWindow codeTab = GetAdjacentCodeTab(right);
		if (codeTab != null)
		{
			codeTab.Focus();
			codeTab.textEditor.AddRecentLocation(1, false);
		}
	}
	
	private void MoveThisTab(bool right)
	{
		var parent = API.parentField.GetValue(this);
		if (parent == null || parent.GetType() != API.dockAreaType)
			return;
		
		List<EditorWindow> panes = API.panesField.GetValue(parent) as List<EditorWindow>;
		if (panes == null)
			return;

		int index = panes.FindIndex(wnd => wnd == this);
		if (index < 0)
			return;

		if (right && index < panes.Count - 1)
		{
			panes[index] = panes[index + 1];
			panes[index + 1] = this;
			Focus();
			Repaint();
		}
		else if (!right && index > 0)
		{
			panes[index] = panes[index - 1];
			panes[index - 1] = this;
			Focus();
			Repaint();
		}
	}

	private void DestroyIfOrphaned()
	{
		var parent = API.parentField.GetValue(this) as Object;
		if (!parent)
		{
			DestroyImmediate(this);
			return;
		}
		
		if (parent.GetType() != API.dockAreaType)
			return;

		var panes = API.panesField.GetValue(parent) as List<EditorWindow>;
		if (panes == null)
			return;

		if (!panes.Contains(this))
			DestroyImmediate(this);
	}

	private void OnEnable()
	{
		if (!prefsLoaded)
			LoadPrefs();
		
		EditorApplication.delayCall += DestroyIfOrphaned;
		
		codeWindows.Add(this);

		hideFlags = HideFlags.HideAndDontSave;
		textEditor.onRepaint = /*Repaint;
		textEditor.onChange =*/ OnTextBufferChanged;

		if (targetAsset == null)
			targetAsset = useTargetAsset;
		if (targetAsset == null && !string.IsNullOrEmpty(targetAssetGuid))
		{
			string path = AssetDatabase.GUIDToAssetPath(targetAssetGuid);
			if (!string.IsNullOrEmpty(path))
				targetAsset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
		}
		if (targetAsset == null)
			targetAsset = Selection.activeObject as MonoScript;
		if (targetAsset == null)
			targetAsset = Selection.activeObject as TextAsset;
		if (targetAsset == null)
			targetAsset = Selection.activeObject as Shader;

		if (targetAsset != null)
			targetAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetAsset));
		
		if (targetAsset == null && !string.IsNullOrEmpty(targetAssetGuid))
		{
			codeWindows.Remove(this);
			try { Close(); } catch {}
			DestroyImmediate(this);
		}
		else
		{
			EditorApplication.update -= OnFirstUpdate;
			EditorApplication.update += OnFirstUpdate;
		}
	}
	
	[System.NonSerialized]
	private bool firstUpdateDone;

	public void OnFirstUpdate()
	{
		Repaint();
		
		EditorApplication.update -= OnFirstUpdate;
		firstUpdateDone = true;

		if (targetAsset != null)
		{
			if (!guidHistory.Contains(targetAssetGuid))
			{
				if (this == focusedWindow && guidHistory.Count > 0)
					guidHistory.Insert(0, targetAssetGuid);
				else
					guidHistory.Add(targetAssetGuid);
				
				SaveGuidHistory();
			}
			
			SaveDefaultPosition();
			
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
			title = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(targetAsset));
#else
			titleContent.text = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(targetAsset));
#endif
		}
		else
		{
			codeWindows.Remove(this);
			Close();
		}
		
		UpdateWindowTitle();
	}
	
	private void OnFirstRepaint()
	{
		if (!firstUpdateDone)
			OnFirstUpdate();
		
		textEditor.OnEnable(targetAsset);
		
		if (this == focusedWindow)
			EditorApplication.delayCall += textEditor.TextBuffer.LoadImmediately;
	}

	public static void CheckAssetRename(string guid)
	{
		foreach (FGCodeWindow wnd in codeWindows)
		{
			if (wnd && wnd.targetAssetGuid == guid)
			{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
				wnd.title = System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
#else
				wnd.titleContent.text = System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
#endif
				wnd.UpdateWindowTitle();
				wnd.Repaint();
			}
		}
	}

	private void PingLineWhenLoaded()
	{
		if (textEditor.CanEdit() && textEditor.codeViewRect.width > 0f)
		{
			EditorApplication.update -= PingLineWhenLoaded;
			textEditor.PingLine(pingLineWhenLoaded);
		}
	}

	private void SetCursorWhenLoaded()
	{
		if (textEditor.CanEdit() && textEditor.codeViewRect.width > 0f)
		{
			EditorApplication.update -= SetCursorWhenLoaded;
			textEditor.SetCursorPosition(setCursorLineWhenLoaded, setCursorCharacterIndexWhenLoaded);
			if (setSelectionLengthWhenLoaded > 0)
				textEditor.PingText(textEditor.caretPosition, setSelectionLengthWhenLoaded, FGTextEditor.yellowPingColor);
		}
	}
	
	private static EditorWindow tabToFocusOnUpdate = null;
	
	private void FocusNextTabOnUpdate()
	{
		EditorApplication.update -= FocusNextTabOnUpdate;
		
		if (tabToFocusOnUpdate)
		{
			tabToFocusOnUpdate.Focus();
			tabToFocusOnUpdate = null;
		}
	}
	
	private void OnDestroy()
	{
		EditorApplication.update -= InitOnLoad;
		EditorApplication.update -= OnFirstUpdate;
		EditorApplication.update -= PingLineWhenLoaded;
		
		if (tabToFocusOnUpdate == this)
		{
			EditorApplication.update -= FocusNextTabOnUpdate;
			tabToFocusOnUpdate = null;
		}
		
		FGCodeWindow focusTab = null;
		var otherTabs = GetTabsInDockArea();
		if (otherTabs != null)
		{
			var historyIndex = int.MaxValue;
			for (var i = otherTabs.Count; i --> 0 && historyIndex != 0; )
			{
				var tab = otherTabs[i] as FGCodeWindow;
				if (tab != null && tab != this)
				{
					var index = guidHistory.IndexOf(tab.targetAssetGuid);
					if (index >= 0 && index < historyIndex)
					{
						focusTab = tab;
						historyIndex = index;
					}
				}
				else if (otherTabs[i])
				{
					defaultDockNextTo = otherTabs[i].GetType().ToString();
				}
			}
			if (focusTab != null)
			{
				tabToFocusOnUpdate = focusTab;
				EditorApplication.update -= FocusNextTabOnUpdate;
				EditorApplication.update += FocusNextTabOnUpdate;
			}
		}
	}
	
	private void OnDisable()
	{
		codeWindows.Remove(this);
		textEditor.onRepaint = null;
		//textEditor.onChange = null;
		textEditor.OnDisable();
	}
	
	private void OnTextBufferChanged()
	{
		UpdateWindowTitle();
		Repaint();
	}
	
	private static List<string> guidHistory = new List<string>();
	
	public static List<string> GetGuidHistory()
	{
		var result = new List<string>();
		for (int i = 0; i < guidHistory.Count; ++i)
		{
			var guid = guidHistory[i];
			foreach (var wnd in codeWindows)
				if (wnd && wnd.targetAssetGuid == guid)
				{
					if (!result.Contains(guid))
						result.Add(guid);
					break;
				}
		}
		foreach (var wnd in codeWindows)
		{
			if (wnd && !string.IsNullOrEmpty(wnd.targetAssetGuid) && !result.Contains(wnd.targetAssetGuid))
			{
				string path = AssetDatabase.GUIDToAssetPath(wnd.targetAssetGuid);
				if (!string.IsNullOrEmpty(path))
				{
					result.Add(wnd.targetAssetGuid);
				}
			}
		}
		return result;
	}
	
	public static FGCodeWindow GetMostRecentlyActive(string ignoreGuid)
	{
		for (int i = 0; i < guidHistory.Count; ++i)
		{
			var guid = guidHistory[i];
			if (guid != ignoreGuid)
				foreach (var wnd in codeWindows)
					if (wnd && wnd.targetAssetGuid == guid)
						return wnd;
		}
		return null;
	}
	
	private static void AddMostRecentGuidHistory(string guid)
	{
		var index = guidHistory.IndexOf(guid);
		if (index > 0)
		{
			for (var i = index; i >= 1; --i)
				guidHistory[i] = guidHistory[i - 1];
			guidHistory[0] = guid;
			SaveGuidHistory();
		}
		else if (index < 0)
		{
			guidHistory.Insert(0, guid);
			SaveGuidHistory();
		}
		
		SaveDefaultPosition();
	}
	
	private void OnFocus()
	{
		if (string.IsNullOrEmpty(targetAssetGuid))
			return;
		
		savedParent = API.parentField.GetValue(this) ?? savedParent;
		
		EditorApplication.update -= AddMostRecentGuidOnUpdate;
		EditorApplication.update += AddMostRecentGuidOnUpdate;
	}
	
	private static void AddMostRecentGuidOnUpdate()
	{
		EditorApplication.update -= AddMostRecentGuidOnUpdate;
		
		var focusedCodeWindow = focusedWindow as FGCodeWindow;
		if (focusedCodeWindow != null && !string.IsNullOrEmpty(focusedCodeWindow.targetAssetGuid))
			AddMostRecentGuidHistory(focusedCodeWindow.targetAssetGuid);
	}
	
	private static void SaveGuidHistory()
	{
		if (guidHistory.Count > 50)
			guidHistory.RemoveRange(50, guidHistory.Count - 50);
		var joinedGuids = string.Join(";", guidHistory.ToArray());
		EditorPrefs.SetString("ScriptInspectorRecentGUIDs", joinedGuids);
		EditorPrefs.SetString("ScriptInspectorDefaultDockNextTo", defaultDockNextTo ?? "");
	}
	
	private static void SaveDefaultPosition()
	{
		EditorPrefs.SetFloat("ScriptInspectorDefaultPositionX", defaultPosition.x);
		EditorPrefs.SetFloat("ScriptInspectorDefaultPositionY", defaultPosition.y);
		EditorPrefs.SetFloat("ScriptInspectorDefaultPositionW", defaultPosition.width);
		EditorPrefs.SetFloat("ScriptInspectorDefaultPositionH", defaultPosition.height);
	}

	private void OnLostFocus()
	{
		textEditor.OnLostFocus();
	}

	[System.NonSerialized]
	private bool isFirstOnGUI = true;
	
	private void OnGUI()
	{
		if (isFirstOnGUI)
		{
			isFirstOnGUI = false;
			OnFirstRepaint();
		}

		if (Event.current.isKey && TabSwitcher.OnGUIGlobal())
			return;
		
		var isOSX = Application.platform == RuntimePlatform.OSXEditor;
		
		switch (Event.current.type)
		{
			case EventType.layout:
				if (IsFloating() && FGTextBuffer.activeEditor == textEditor)
					defaultPosition = position;
				break;
				
			case EventType.KeyDown:
				if ((Event.current.modifiers & ~(EventModifiers.FunctionKey | EventModifiers.Numeric | EventModifiers.CapsLock)) == EventModifiers.Control &&
					(Event.current.keyCode == KeyCode.PageUp || Event.current.keyCode == KeyCode.PageDown))
				{
					SelectAdjacentCodeTab(Event.current.keyCode == KeyCode.PageDown);
					Event.current.Use();
					GUIUtility.ExitGUI();
				}
				else if (Event.current.alt && Event.current.control)
				{
					if (Event.current.keyCode == KeyCode.RightArrow || Event.current.keyCode == KeyCode.LeftArrow)
					{
						if (Event.current.shift)
						{
							MoveThisTab(Event.current.keyCode == KeyCode.RightArrow);
						}
						else
						{
							SelectAdjacentCodeTab(Event.current.keyCode == KeyCode.RightArrow);
						}
						Event.current.Use();
						GUIUtility.ExitGUI();
					}
				}
				else if (!isOSX && !Event.current.alt && !Event.current.shift && EditorGUI.actionKey)
				{
					if (Event.current.keyCode == KeyCode.W || Event.current.keyCode == KeyCode.F4)
					{
						Event.current.Use();
						if (!IsMaximized())
						{
							codeWindows.Remove(this);
							Close();
						}
					}
				}
				else if (isOSX && !Event.current.alt && EditorGUI.actionKey)
				{
					if (Event.current.shift && Event.current.keyCode == KeyCode.W ||
						!Event.current.shift && Event.current.keyCode == KeyCode.F4)
					{
						Event.current.Use();
						if (!IsMaximized())
						{
							codeWindows.Remove(this);
							Close();
						}
					}
				}
				else if (!isOSX && !Event.current.alt && Event.current.shift && EditorGUI.actionKey)
				{
					if (Event.current.keyCode == KeyCode.W || Event.current.keyCode == KeyCode.F4)
					{
						CloseOtherTabs();
					}
				}
				else if (Event.current.alt && !Event.current.shift && !EditorGUI.actionKey && Event.current.keyCode == KeyCode.Return)
				{
					Event.current.Use();
					ToggleMaximized(this);
					GUIUtility.ExitGUI();
				}
				break;
				
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (DragAndDrop.objectReferences.Length > 0)
				{
					bool ask = false;

					HashSet<Object> accepted = new HashSet<Object>();
					foreach (Object obj in DragAndDrop.objectReferences)
					{
						var assetPath = AssetDatabase.GetAssetPath(obj);
						if (assetPath.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase) ||
							assetPath.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						
						if (assetPath.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase))
						{
							if (obj is MonoScript)
								accepted.Add(obj);
							else if (obj is TextAsset || obj is Shader)
								accepted.Add(obj);
							else if (obj is Material)
							{
								Material material = obj as Material;
								if (material.shader != null)
								{
									int shaderID = material.shader.GetInstanceID();
									if (shaderID != 0)
									{
										assetPath = AssetDatabase.GetAssetPath(shaderID);
										if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase))
											accepted.Add(material.shader);
									}
								}
							}
						}
						else if (obj is GameObject)
						{
							GameObject gameObject = obj as GameObject;
							MonoBehaviour[] monoBehaviours = gameObject.GetComponents<MonoBehaviour>();
							foreach (MonoBehaviour mb in monoBehaviours)
							{
								MonoScript monoScript = MonoScript.FromMonoBehaviour(mb);
								if (monoScript != null)
								{
									assetPath = AssetDatabase.GetAssetPath(monoScript);
									if (!string.IsNullOrEmpty(assetPath) &&
										assetPath.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase) &&
										!assetPath.EndsWith(".dll", System.StringComparison.OrdinalIgnoreCase))
									{
										accepted.Add(monoScript);
										ask = true;
									}
								}
							}
						}
					}
					
					if (accepted.Count > 0)
					{
						DragAndDrop.AcceptDrag();
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						if (Event.current.type == EventType.DragPerform)
						{
							Object[] sorted = accepted.OrderBy(x => x.name, System.StringComparer.OrdinalIgnoreCase).ToArray();

							if (ask && sorted.Length > 1)
							{
								GenericMenu popupMenu = new GenericMenu();
								foreach (Object target in sorted)
								{
									Object tempTarget = target;
									string fileName = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(target));
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
									fileName = fileName.Replace('_', '\xFF3F');
#endif
									popupMenu.AddItem(
										new GUIContent("Open " + fileName),
										false,
										() => { OpenNewWindow(tempTarget, this, true); });
								}
								popupMenu.AddSeparator("");
								popupMenu.AddItem(
									new GUIContent("Open All"),
									false,
									() => { foreach (Object target in sorted) OpenNewWindow(target, this, true); });
								
								popupMenu.ShowAsContext();
							}
							else
							{
								foreach (Object target in sorted)
									OpenNewWindow(target, this, true);
							}
						}
						Event.current.Use();
						return;
					}
				}
				break;

			case EventType.ValidateCommand:
				if (Event.current.commandName == "ScriptInspector.AddTab")
				{
					Event.current.Use();
					return;
				}
				break;

			case EventType.ExecuteCommand:
				if (Event.current.commandName == "ScriptInspector.AddTab")
				{
					Event.current.Use();
					OpenNewWindow(targetAsset, this, false);
					return;
				}
				break;
		}
		
		if (!wantsMouseMove)
			wantsMouseMove = true;
		if (rectOffset == null)
			rectOffset = new RectOffset(0, 0, 18, 0);
		textEditor.OnWindowGUI(this, rectOffset);
	}
	
	[System.NonSerialized]
	private static RectOffset rectOffset;
	
	private void UpdateWindowTitle()
	{
		bool isModified = textEditor.IsModified;
		if (!isModified && textEditor.TextBuffer == null)
		{
			if (!string.IsNullOrEmpty(targetAssetGuid))
			{
				string path = AssetDatabase.GUIDToAssetPath(targetAssetGuid);
				if (!string.IsNullOrEmpty(path))
				{
					var buffer = FGTextBufferManager.TryGetBuffer(path);
					if (buffer != null)
						isModified = buffer.IsModified;
				}
			}
		}
		
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		if (title.StartsWith("*", System.StringComparison.Ordinal))
		{
			if (!isModified)
				title = title.Substring(1);
			else
				return;
		}
		else
		{
			if (isModified)
				title = "*" + title;
			else
				return;
		}
#else
		if (titleContent.text.StartsWith("*", System.StringComparison.Ordinal))
		{
			if (!isModified)
				titleContent.text = titleContent.text.Substring(1);
			else
				return;
		}
		else
		{
			if (isModified)
				titleContent.text = "*" + titleContent.text;
			else
				return;
		}
#endif
		
		foreach (FGCodeWindow wnd in codeWindows)
			if (wnd && wnd.targetAssetGuid == targetAssetGuid)
				wnd.UpdateWindowTitle();
	}
	
	static GUIStyle tabStyle;
	static GUIStyle tabBgStyle;
	[System.NonSerialized]
	private bool hoverTabs = false;
	[System.NonSerialized]
	private float[] tabWidths;
	
	static int hoverIndex;
	static float hoverFraction;
	static float hoverEffect;
	static EditorWindow hoverWindow;
	static System.DateTime lastHoverChangeTime;
	static bool hoverMouseDown;
	
	private void ShowButton(Rect position)
	{
		if (SISettings.expandTabTitles == 0)
			return;
		
		if (Event.current.type != EventType.Repaint)
			return;
		
		if (GUIUtility.hotControl != 0
			|| DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length != 0
			|| DragAndDrop.paths != null && DragAndDrop.paths.Length != 0)
		{
			return;
		}
		
		if (tabStyle == null)
		{
			tabStyle = "dragtab";
			tabBgStyle = "dockarea";
		}
		
		position.xMin = 0f;
		position.y -= 4f;
		position.height = 17f;
		position.width += 2f;
		
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3 && !UNITY_4_5
		position.width -= 20f;
#endif
		
		var newHoverTabs = GUIUtility.hotControl == 0 && position.Contains(Event.current.mousePosition);// && mouseOverWindow == this);
		if (newHoverTabs != hoverTabs)
		{
			lastHoverChangeTime = FGTextEditor.frameTime;
			hoverTabs = newHoverTabs;
			if (hoverTabs)
			{
				hoverWindow = this;
			}
			Repaint();
			
			return;
		}
		
		if (hoverWindow != null && hoverWindow != this)
			return;
				
		if (hoverTabs)
		{
			if (hoverEffect < 1f)
			{
				hoverEffect += 5f * (float) (FGTextEditor.frameTime - lastHoverChangeTime).TotalSeconds;
				if (hoverEffect > 1f)
					hoverEffect = 1f;
			}
		}
		else
		{
			if (hoverEffect > 0f)
			{
				hoverEffect -= 1.5f * (float) (FGTextEditor.frameTime - lastHoverChangeTime).TotalSeconds;
				if (hoverEffect < 0f)
				{
					hoverEffect = 0f;
					hoverWindow = null;
				}
			}
		}
		
		if (hoverTabs || hoverEffect > 0f)
		{
			lastHoverChangeTime = FGTextEditor.frameTime;
			
			List<EditorWindow> panes = GetTabsInDockArea();
			if (panes == null)
				return;
			
			int thisIndex = panes.IndexOf(this);
			if (thisIndex < 0)
				return;
			
			var numTabs = panes.Count;
			var tabWidth = Mathf.Min(position.width / numTabs, 100f);
			
			if (tabWidths == null || tabWidths.Length != numTabs)
			{
				tabWidths = new float[numTabs];
				for (int i = 0; i < numTabs; ++i)
				{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
					var tabTitle = API.cachedTitleContentField.GetValue(panes[i], null) as GUIContent;
					tabWidths[i] = Mathf.Max(tabWidth, tabStyle.CalcSize(tabTitle).x);
#else
					tabWidths[i] = Mathf.Max(tabWidth, tabStyle.CalcSize(panes[i].titleContent).x);
#endif
				}
			}
			
			tabBgStyle.Draw(position, false, false, false, false);
			
			hoverIndex = Mathf.Clamp((int) ((Event.current.mousePosition.x - position.x) / tabWidth), 0, numTabs - 1);
			
			var hoverTabWidth = tabWidths[hoverIndex];
			var hoverOverflow = (hoverTabWidth - tabWidth) * hoverEffect;
			
			hoverFraction = Mathf.Clamp01((Event.current.mousePosition.x - position.x - hoverIndex * tabWidth) / tabWidth);
			
			for (var i = 0; i < numTabs; ++i)
			{
				var tabRect = position;
				tabRect.x = Mathf.Floor(position.x + i * tabWidth);
				tabRect.width = tabWidth;
				
				if (i == hoverIndex)
				{
					tabRect.width = tabWidth + hoverOverflow;
					tabRect.x -= hoverFraction * hoverOverflow;
				}
				else if (i == hoverIndex - 1)
				{
					tabRect.x -= hoverFraction * hoverOverflow;
					var thisOverflow = (tabWidths[i] - tabWidth) * hoverEffect;
					tabRect.xMin -= thisOverflow * (1f - hoverFraction);
				}
				else if (i == hoverIndex + 1)
				{
					tabRect.x += (1f - hoverFraction) * hoverOverflow;
					var thisOverflow = (tabWidths[i] - tabWidth) * hoverEffect;
					tabRect.width += thisOverflow * hoverFraction;
				}
				else if (i < hoverIndex)
				{
					var distance = 0.1f * (12f - (hoverFraction + hoverIndex - i));
					if (distance > 0f)
					{
						if (distance > 1f)
							distance = 1f;
						var leftOverflow = (tabWidths[hoverIndex - 1] - tabWidth);
						tabRect.x -= hoverFraction * hoverOverflow * distance * hoverEffect;
						tabRect.x -= leftOverflow * (1f - hoverFraction) * distance * hoverEffect;
					}
				}
				else if (i > hoverIndex)
				{
					var distance = 0.1f * (11f - (i - hoverIndex - hoverFraction));
					if (distance > 0f)
					{
						if (distance > 1f)
							distance = 1f;
						var rightOverflow = (tabWidths[hoverIndex + 1] - tabWidth);
						tabRect.x += (1f - hoverFraction) * hoverOverflow * distance * hoverEffect;
						tabRect.x += rightOverflow * hoverFraction * distance * hoverEffect;
					}
				}
				
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
				GUIContent title = API.cachedTitleContentField.GetValue(panes[i], null) as GUIContent;
				tabStyle.Draw(tabRect, title, false, false, i == thisIndex, focusedWindow == this);
	#else
				tabStyle.Draw(tabRect, panes[i].titleContent, false, false, i == thisIndex, focusedWindow == this);
	#endif
			}
			
			Repaint();
		}
		else
		{
			tabWidths = null;
		}
	}
	
	private static bool IsMaximized(EditorWindow window)
	{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1
		if (API.windowLayoutType == null)
			return false;
		return (bool) API.isMaximizedMethod.Invoke(null, new object[] {window});
#else
		return window.maximized;
#endif
	}
	
	private static void ToggleMaximized(EditorWindow window)
	{
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1
		if (API.windowLayoutType != null)
		{
			var isMaximized = (bool) API.isMaximizedMethod.Invoke(null, new object[] {window});
			if (isMaximized)
				API.unMaximizeMethod.Invoke(null, new object[] {window});
			else
				API.maximizeMethod.Invoke(null, new object[] {window});
		}
#else
		window.maximized = !window.maximized;
#endif
		
		var asCodeWindow = window as FGCodeWindow;
		if (asCodeWindow)
			asCodeWindow.textEditor.FocusCodeView();
	}
	
	private bool IsMaximized()
	{
	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1
		if (API.windowLayoutType == null)
			return false;
		return (bool) API.isMaximizedMethod.Invoke(null, new object[] {this});
	#else
		return maximized;
	#endif
	}
	
	public void AddItemsToMenu(GenericMenu menu)
	{
		if (!string.IsNullOrEmpty(targetAssetGuid))
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(targetAssetGuid);
			var fileName = System.IO.Path.GetFileName(assetPath);
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			fileName = fileName.Replace('_', '\xFF3F');
#endif
			menu.AddItem(new GUIContent("Ping " + fileName), false, () => {
				EditorApplication.ExecuteMenuItem("Window/Project");
				EditorGUIUtility.PingObject(targetAsset);
			});
#if UNITY_EDITOR_OSX
			menu.AddItem(new GUIContent("Reveal in Finder"), false, () => {
				Selection.activeObject = targetAsset;
				EditorApplication.ExecuteMenuItem("Assets/Reveal in Finder");
			});
#else
			menu.AddItem(new GUIContent("Show in Explorer"), false, () => {
				Selection.activeObject = targetAsset;
				EditorApplication.ExecuteMenuItem("Assets/Show in Explorer");
			});
#endif
			menu.AddSeparator("");
			var isMaximized = IsMaximized();
			
			menu.AddItem("Maximize", "&\n", "Maximize", "&enter", isMaximized, () => ToggleMaximized(this));
			if (isMaximized)
			{
				menu.AddDisabledItem(new GUIContent("Close Tab"));
				menu.AddDisabledItem(new GUIContent("Close All SI Tabs"));
				menu.AddItem("Close Other SI Tabs", "", "Close Other SI Tabs", "%#w", false, null);
			}
			else
			{
				menu.AddItem("Close Tab", "%#w", "Close Tab", "%w", false, () => {
					codeWindows.Remove(this);
					Close();
				});
				menu.AddItem(new GUIContent("Close All SI Tabs"), false, () => {
					CloseOtherTabs();
					try { codeWindows.Remove(this); Close(); } catch {}
				});
				menu.AddItem("Close Other SI Tabs", "", "Close Other SI Tabs", "%#w", false, CloseOtherTabs);
			}
			
			menu.ShowAsContext();
			GUIUtility.ExitGUI();
		}
	}
	
	public void CloseOtherTabs()
	{
		var allWindows = new FGCodeWindow[codeWindows.Count];
		codeWindows.CopyTo(allWindows);
		foreach (var window in allWindows)
			if (window && window != this)
				try { codeWindows.Remove(window); window.Close(); } catch {}
	}
	
	private bool IsFloating()
	{
		if (API.parentField == null || API.parentProperty == null)
			return false;
		var parent = API.parentField.GetValue(this);
		if (parent == null)
			return false;
		if (parent.GetType() != API.dockAreaType)
			return true;
		parent = API.parentProperty.GetValue(parent, null);
		if (parent == null)
			return false;
		var view = parent;
		while (parent != null)
		{
			if (parent.GetType() != API.splitViewType)
				break;
			view = parent;
			parent = API.parentProperty.GetValue(parent, null);
		}
		if (view == null)
			return true;
		var viewParent = API.parentProperty.GetValue(view, null);
		if (viewParent == null || viewParent.GetType() != API.mainWindowType)
			return true;
		//if ((UnityEngine.Object) win.m_Parent.window == (UnityEngine.Object) null)
		//	return (View) null;
		return false;
	}
	
	static void UnhideSi3Tabs()
	{
		var path = System.IO.Directory.GetCurrentDirectory() + "/Library/Si3Layout.temp";
		if (System.IO.File.Exists(path))
			ToggleSi3Tabs();
	}
	
#if UNITY_EDITOR_OSX
	[MenuItem("Window/Script Inspector 3/Toggle Floating Tabs _%#t", true, 810)] // Shift-Cmd-T
#else
	[MenuItem("Window/Script Inspector 3/Toggle Floating Tabs _&t", true, 810)] // Alt+T
#endif
	static bool ValidateToggleSi3Tabs()
	{
		var path = System.IO.Directory.GetCurrentDirectory() + "/Library/Si3Layout.temp";
		if (System.IO.File.Exists(path))
			return true;
		
		if (codeWindows.Count == 0)
			return false;
		
		foreach (var tab in codeWindows)
			if (tab && tab.IsFloating())
				return true;
		
		return false;
	}
	
	public static bool hidingFloatingTabs;
	
#if UNITY_EDITOR_OSX
	[MenuItem("Window/Script Inspector 3/Toggle Floating Tabs _%#t", false, 810)] // Shift-Cmd-T
#else
	[MenuItem("Window/Script Inspector 3/Toggle Floating Tabs _&t", false, 810)] // Alt+T
#endif
	static void ToggleSi3Tabs()
	{
		var path = System.IO.Directory.GetCurrentDirectory() + "/Library/Si3Layout.temp";
		var oldHistory = guidHistory.ToArray();
		var mostRecentIndex = int.MaxValue;
		
		var rc = defaultPosition;

		if (System.IO.File.Exists(path))
		{
			rc.y -= 5f;
			
			FGCodeWindow setFocusOn = null;
			FGCodeWindow lastShown = null;
			var allWindows = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(path);
			
			try
			{
				System.IO.File.Delete(path);
			}
			catch (System.IO.IOException)
			{
				foreach (var window in allWindows)
					DestroyImmediate(window);
				return;
			}
			
			var firstTab = true;
			foreach (var window in allWindows)
			{
				var codeWindow = (FGCodeWindow) window;
					
				if (firstTab || !codeWindow.TryDockNextToSimilarTab(lastShown))
				{
					codeWindow.Show(true);
					codeWindow.position = rc;
					codeWindow.Focus();
					codeWindow.Repaint();
					lastShown = codeWindow;
					firstTab = false;
				}
				var index = System.Array.IndexOf(oldHistory, codeWindow.targetAssetGuid);
				if (index >= 0 && index < mostRecentIndex)
				{
					mostRecentIndex = index;
					setFocusOn = codeWindow;
				}
			}
			
			if (setFocusOn)
				setFocusOn.Focus();
		}
		else if (codeWindows.Count > 0)
		{
//#if UNITY_EDITOR_OSX
			rc.y -= 5f;
//#endif
			
			var floatingSi3Tabs = new List<FGCodeWindow>(codeWindows);
			for (var i = floatingSi3Tabs.Count; i --> 0; )
			{
				var tab = floatingSi3Tabs[i];
				if (!tab || !tab.IsFloating())
				{
					floatingSi3Tabs.RemoveAt(i);
				}
				else
				{
					tab.Repaint();
				}
			}
			
			hidingFloatingTabs = true;
			EditorApplication.delayCall += () => hidingFloatingTabs = false;
			
			var toggleSi3Tabs = floatingSi3Tabs.ToArray();
			UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(toggleSi3Tabs, path, true);
			for (var i = toggleSi3Tabs.Length; i --> 0; )
			{
				var tab = toggleSi3Tabs[i];
				if (tab)
				{
					codeWindows.Remove(tab);
					tab.Close();
				}
			}
			
//#if UNITY_EDITOR_OSX
			rc.y += 5f;
//#endif
			defaultPosition = rc;
			SaveDefaultPosition();
		}
		
		guidHistory.Clear();
		guidHistory.AddRange(oldHistory);
		SaveGuidHistory();
		SaveDefaultPosition();
	}
}

}
