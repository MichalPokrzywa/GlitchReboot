using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VariableDice))]
public class VariableDiceEditor : Editor
{
    SerializedProperty typeProp;
    SerializedProperty baseNumberValueProp;
    SerializedProperty baseBooleanValueProp;
    SerializedProperty textListProp;

    void OnEnable()
    {
        // Link SerializedProperties to the corresponding fields
        typeProp = serializedObject.FindProperty("type");
        baseNumberValueProp = serializedObject.FindProperty("baseNumberValue");
        baseBooleanValueProp = serializedObject.FindProperty("baseBooleanValue");
        textListProp = serializedObject.FindProperty("textList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Ensure serialized object is up to date

        // Display the VariableType dropdown
        EditorGUILayout.PropertyField(typeProp);

        // Show the appropriate field based on the selected type
        VariableType type = (VariableType)typeProp.enumValueIndex;
        if (type == VariableType.Number)
        {
            EditorGUILayout.PropertyField(baseNumberValueProp, new GUIContent("Base Number Value"));
        }
        else if (type == VariableType.Boolean)
        {
            EditorGUILayout.PropertyField(baseBooleanValueProp, new GUIContent("Base Boolean Value"));
        }

        // Display the textList field
        EditorGUILayout.PropertyField(textListProp, new GUIContent("Text List"), true);

        serializedObject.ApplyModifiedProperties(); // Apply changes to the serialized object
    }
}