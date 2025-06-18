using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VariableDice))]
public class VariableDiceEditor : Editor
{
    SerializedProperty typeProp;
    SerializedProperty baseNumberValueProp;
    SerializedProperty baseBooleanValueProp;
    SerializedProperty baseStringValueProp;
    SerializedProperty baseGameObjectValueProp;
    SerializedProperty textListProp;
    SerializedProperty entityIdProp;
    SerializedProperty entityTooltipInteraction;

    void OnEnable()
    {
        // Link SerializedProperties to the corresponding fields
        typeProp = serializedObject.FindProperty("type");
        baseNumberValueProp = serializedObject.FindProperty("baseNumberValue");
        baseBooleanValueProp = serializedObject.FindProperty("baseBooleanValue");
        baseStringValueProp = serializedObject.FindProperty("baseStringValue");
        baseGameObjectValueProp = serializedObject.FindProperty("baseGameObjectValue");
        textListProp = serializedObject.FindProperty("textList");
        entityIdProp = serializedObject.FindProperty("entityId");
        entityTooltipInteraction = serializedObject.FindProperty("entityTooltipInteraction");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Ensure serialized object is up to date

        // Display the Entity ID field
        EditorGUILayout.PropertyField(entityIdProp, new GUIContent("Entity ID"));

        // Display the VariableType dropdown
        EditorGUILayout.PropertyField(typeProp);

        // Show the appropriate field based on the selected type
        VariableType type = (VariableType)typeProp.enumValueIndex;
        switch (type)
        {
            case VariableType.Number:
                EditorGUILayout.PropertyField(baseNumberValueProp, new GUIContent("Base Number Value"));
                break;
            case VariableType.Boolean:
                EditorGUILayout.PropertyField(baseBooleanValueProp, new GUIContent("Base Boolean Value"));
                break;
            case VariableType.String:
                EditorGUILayout.PropertyField(baseStringValueProp, new GUIContent("Base String Value"));
                break;
            case VariableType.GameObject:
                EditorGUILayout.PropertyField(baseGameObjectValueProp, new GUIContent("Base GameObject Value"));
                break;
        }

        // Display the textList field
        EditorGUILayout.PropertyField(textListProp, new GUIContent("Text List"), true);

        // Display the Entity Interaction field
        EditorGUILayout.PropertyField(entityTooltipInteraction, new GUIContent("Entity Tooltip Interaction"));

        serializedObject.ApplyModifiedProperties(); // Apply changes to the serialized object
    }
}