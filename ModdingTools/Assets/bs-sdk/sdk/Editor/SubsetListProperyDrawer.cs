using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;




public class SubsetListProperyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
            return;
        SubsetList targetEnum = GetBaseProperty<SubsetList>(property);
        EditorGUI.BeginProperty(position, label, property);
        try
        {
            property.FindPropertyRelative("selected").intValue = EditorGUI.MaskField(position, property.name, targetEnum.selected, targetEnum.names);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        EditorGUI.EndProperty();
    }

    static T GetBaseProperty<T>(SerializedProperty prop)
    {
        // Separate the steps it takes to get to this property
        string[] separatedPaths = prop.propertyPath.Split('.');

        // Go down to the root of this serialized property
        System.Object reflectionTarget = prop.serializedObject.targetObject as object;
        // Walk down the path to get the target object
        foreach (var path in separatedPaths)
        {
            FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
            reflectionTarget = fieldInfo.GetValue(reflectionTarget);
        }
        return (T)reflectionTarget;
    }
}