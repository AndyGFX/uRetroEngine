using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DeviceConsole))]
public class DeviceConsoleEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("uiContainer"), new GUIContent("UI Container"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("logContainer"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("commandInputField"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("autoFocusInputField"));

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("headerColour"));

		SerializedProperty headTextProp = serializedObject.FindProperty("headerText");

		EditorGUILayout.LabelField("Header Text:");
		headTextProp.stringValue = EditorGUILayout.TextArea(headTextProp.stringValue);

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("logPrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("warningLogPrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("errorLogPrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("assertLogPrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("exceptionLogPrefab"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("exceptionStackTracePrefab"));

		EditorGUILayout.Space();

		serializedObject.ApplyModifiedProperties();
	}
}
