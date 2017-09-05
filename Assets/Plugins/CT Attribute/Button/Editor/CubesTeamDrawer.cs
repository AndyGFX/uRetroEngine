using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CubesTeam
{
    public class CubesTeamDrawer : PropertyDrawer
    {
        private bool doOnlyOnce;

        //private Dictionary<string, GameObject> targetObjects = new Dictionary<string, GameObject>();

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Rect:
                    return base.GetPropertyHeight(prop, label) * 3;

                case SerializedPropertyType.Vector2:
                    return base.GetPropertyHeight(prop, label) * 2;

                case SerializedPropertyType.Vector3:
                    return base.GetPropertyHeight(prop, label) * 2;

                default:
                    return base.GetPropertyHeight(prop, label);
            }
        }

        public static void CallMethod(MonoBehaviour go, string methodName)
        {
            MethodInfo buttonFunction = GetMethodRecursively(go.GetType(), methodName);
            if (buttonFunction == null)
            {
                Debug.LogError(string.Format("Function {0} not found in class {1} or any of its base classes.", methodName, go.GetType().ToString()));
            }
            else
            {
                buttonFunction.Invoke(go, null);
            }
        }

        public static void CallMethod(ScriptableObject so, string methodName)
        {
            MethodInfo buttonFunction = GetMethodRecursively(so.GetType(), methodName);
            if (buttonFunction == null)
            {
                Debug.LogError(string.Format("Function {0} not found in class {1} or any of its base classes.", methodName, so.GetType().ToString()));
            }
            else
            {
                buttonFunction.Invoke(so, null);
            }
        }

        public static MethodInfo GetMethodRecursively(System.Type type, string methodName)
        {
            MethodInfo buttonFunction = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null, new System.Type[0], null);
            if (buttonFunction == null)
            {
                if (type.BaseType != null)
                    return GetMethodRecursively(type.BaseType, methodName);
                else
                    return null;
            }
            return buttonFunction;
        }

        public static FieldInfo GetReflectedFieldInfoRecursively(SerializedProperty prop, out System.Object targetObject, string fieldName = "")
        {
            string fullpath = prop.propertyPath;

            string propName = prop.name;

            if (!string.IsNullOrEmpty(fieldName))
            {
                fullpath = fullpath.Replace(prop.name, fieldName);
                propName = fieldName;
            }

            List<string> pathList = fullpath.Split('.').ToList();

            if (propName == "data")
            {
                // This is a list, we must find the real name by getting the string before "Array"

                propName = pathList[pathList.LastIndexOf("Array") - 1];
            }

            pathList.Remove("Array");

            targetObject = prop.serializedObject.targetObject;

            if ((prop.serializedObject.targetObject as MonoBehaviour) != null)
            {
                return GetReflectedFieldInfoRecursively(prop.serializedObject.targetObject, (prop.serializedObject.targetObject as MonoBehaviour).GetType().GetField((prop.propertyPath.Split('.').Length == 1 && !string.IsNullOrEmpty(propName) ? propName : prop.propertyPath.Split('.')[0]), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), propName, pathList, out targetObject);
            }

            if ((prop.serializedObject.targetObject as ScriptableObject) != null)
            {
                return GetReflectedFieldInfoRecursively(prop.serializedObject.targetObject, (prop.serializedObject.targetObject as ScriptableObject).GetType().GetField((prop.propertyPath.Split('.').Length == 1 && !string.IsNullOrEmpty(propName) ? propName : prop.propertyPath.Split('.')[0]), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), propName, pathList, out targetObject);
            }

            return null;
        }

        private static FieldInfo GetReflectedFieldInfoRecursively(System.Object targetObject, FieldInfo field, string propName, List<string> pathNames, out System.Object outObject)
        {
            if (pathNames.Count > 1 && pathNames[0] != propName)
            {
                pathNames.RemoveAt(0);

                if (pathNames[0].Contains("data["))
                {
                    // The next field is hidden inside a list
                    int pathIndex = int.Parse(pathNames[0].Replace("data[", "").Replace("]", ""));

                    pathNames.RemoveAt(0);

                    try
                    {
                        // If index is greater than length, the list's size is being increased this frame, return null to prevent an error and try again next frame.
                        if ((field.GetValue(targetObject) as IList).Count <= pathIndex)
                        {
                            outObject = null;
                            return null;
                        }
                    }
                    catch
                    {
                        // If it breaks here, the value is null. Return for this frame to prevent an error and try again next frame.
                        outObject = null;
                        return null;
                    }

                    return GetReflectedFieldInfoRecursively((field.GetValue(targetObject) as IList)[pathIndex], field.FieldType.GetGenericArguments()[0].GetField(pathNames[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), propName, pathNames, out outObject);
                }
                return GetReflectedFieldInfoRecursively(field.GetValue(targetObject), field.FieldType.GetField(pathNames[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public), propName, pathNames, out outObject);
            }
            else
            {
                outObject = targetObject;
                return field;
            }
        }

        public static T GetReflectedFieldRecursively<T>(SerializedProperty prop, string fieldName = "")
        {
            System.Object targetObject = null;
            FieldInfo info = GetReflectedFieldInfoRecursively(prop, out targetObject, fieldName);

            T temp = default(T);
            try
            {
                temp = (T)(info.GetValue(targetObject));
            }
            catch
            {
                try
                {
                    temp = (T)(info.GetValue(targetObject) as IList)[int.Parse(prop.propertyPath.Substring(prop.propertyPath.LastIndexOf("data[") + 5).Split(']')[0])];
                }
                catch
                {
                    // Debug.LogWarning(string.Format("The script has no property named {0}.", prop.name));
                }
            }

            return temp;
        }

        protected void WrongVariableTypeWarning(string attributeName, string variableType)
        {
            if (!doOnlyOnce)
            {
                Debug.LogError(string.Format("The {0}Attribute is designed to be applied to {1} only!", attributeName, variableType));
                doOnlyOnce = true;
            }
        }

        protected System.Object GetFieldOrPropertyValue(Component component, string fieldName)
        {
            if (component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) != null)
                return component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(component);
            else if (component.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) != null)
                return component.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(component, null);
            else
                Debug.LogError(string.Format("{0} does not contain a field or property named {1}!", component, fieldName));

            return "";
        }

        public void DrawColoredBox(Rect position, Color c)
        {
            Color temp = GUI.color;

            GUI.color = c;
            EditorGUI.HelpBox(position, "", MessageType.None);

            GUI.color = temp;
        }

        public void DrawPropertyByType(int index, Rect position, SerializedProperty prop, GUIContent label)
        {
            string fieldName = prop.propertyPath.Split('.')[0] + "[" + index.ToString() + "]";
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    prop.boolValue = EditorGUI.Toggle(position, fieldName, prop.boolValue);
                    break;

                case SerializedPropertyType.Color:
                    prop.colorValue = EditorGUI.ColorField(position, fieldName, prop.colorValue);
                    break;

                case SerializedPropertyType.Float:
                    prop.floatValue = EditorGUI.FloatField(position, fieldName, prop.floatValue);
                    break;

                case SerializedPropertyType.Integer:
                    prop.intValue = EditorGUI.IntField(position, fieldName, prop.intValue);
                    break;

                case SerializedPropertyType.ObjectReference:
                    System.Type type = typeof(UnityEngine.Object);
                    if (prop.objectReferenceValue != null)
                        type = prop.objectReferenceValue.GetType();
                    prop.objectReferenceValue = EditorGUI.ObjectField(position, fieldName, prop.objectReferenceValue, type, true);
                    break;

                case SerializedPropertyType.Rect:
                    prop.rectValue = EditorGUI.RectField(position, fieldName, prop.rectValue);
                    break;

                case SerializedPropertyType.String:
                    prop.stringValue = EditorGUI.TextField(position, fieldName, prop.stringValue);
                    break;

                case SerializedPropertyType.Vector2:
                    prop.vector2Value = EditorGUI.Vector2Field(position, fieldName, prop.vector2Value);
                    position.height -= 16;
                    break;

                case SerializedPropertyType.Vector3:
                    prop.vector3Value = EditorGUI.Vector3Field(position, fieldName, prop.vector3Value);
                    break;

                default:
                    break;
            }
        }

        public void InsertToListByType(IList list, int index, SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    list.Insert(index, false);
                    break;

                case SerializedPropertyType.Color:
                    list.Insert(index, Color.white);
                    break;

                case SerializedPropertyType.Float:
                    list.Insert(index, 0.0f);
                    break;

                case SerializedPropertyType.Integer:
                    list.Insert(index, 0);
                    break;

                case SerializedPropertyType.Rect:
                    list.Insert(index, new Rect(0, 0, 128, 25));
                    break;

                case SerializedPropertyType.String:
                    list.Insert(index, "Undefined");
                    break;

                case SerializedPropertyType.Vector2:
                    list.Insert(index, Vector2.zero);
                    break;

                case SerializedPropertyType.Vector3:
                    list.Insert(index, Vector3.zero);
                    break;

                default:
                    break;
            }
        }
    }
}