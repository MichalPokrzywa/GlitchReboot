using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomEditor(typeof(ResetObject))]
public class ResetObjectEditor : Editor
{
    private SerializedProperty itemsProp;
    private bool[] foldouts;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private ResetObject resetTarget;

    private bool preventDuplicates = true;

    void OnEnable()
    {
        itemsProp = serializedObject.FindProperty("itemsToReset");
        foldouts = new bool[itemsProp.arraySize];

        resetTarget = (ResetObject)target;

        lastPosition = resetTarget.transform.position;
        lastRotation = resetTarget.transform.rotation;

        EditorApplication.update += MonitorTransformChange;
    }
    void OnDisable()
    {
        EditorApplication.update -= MonitorTransformChange;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Resettable Items", EditorStyles.boldLabel);
        preventDuplicates = EditorGUILayout.Toggle("Prevent Duplicate Reset Types", preventDuplicates);

        EditorGUILayout.Space();

        for (int i = 0; i < itemsProp.arraySize; i++)
        {
            if (foldouts.Length != itemsProp.arraySize)
                Array.Resize(ref foldouts, itemsProp.arraySize);

            SerializedProperty item = itemsProp.GetArrayElementAtIndex(i);
            var actionTypeProp = item.FindPropertyRelative("actionType");

            string title = $"{actionTypeProp.enumDisplayNames[actionTypeProp.enumValueIndex]} Action";
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], title, true);

            if (foldouts[i])
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(actionTypeProp);

                var type = (ResetObject.ResetActionType)actionTypeProp.enumValueIndex;

                switch (type)
                {
                    case ResetObject.ResetActionType.TransformReset:
                        DrawTransformInfo(item);
                        break;

                    case ResetObject.ResetActionType.MaterialReset:
                        EditorGUILayout.PropertyField(item.FindPropertyRelative("originalMaterial"), new GUIContent("Original Material"));
                        break;

                    case ResetObject.ResetActionType.ScriptMethodCall:
                        EditorGUILayout.PropertyField(item.FindPropertyRelative("targetComponent"));
                        EditorGUILayout.PropertyField(item.FindPropertyRelative("methodName"));
                        EditorGUILayout.PropertyField(item.FindPropertyRelative("onResetEvent"));
                        break;
                }

                if (GUILayout.Button("Remove"))
                {
                    itemsProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (preventDuplicates)
        {
            bool allTypesUsed = Enum.GetValues(typeof(ResetObject.ResetActionType)).Cast<ResetObject.ResetActionType>().All(type =>
                Enumerable.Range(0, itemsProp.arraySize).Any(i =>
                    (ResetObject.ResetActionType)itemsProp.GetArrayElementAtIndex(i).FindPropertyRelative("actionType").enumValueIndex == type));

            if (allTypesUsed)
            {
                EditorGUILayout.HelpBox("All reset types are already used.", MessageType.Info);
                GUI.enabled = false;
            }
        }

        if (GUILayout.Button("Add New Reset Item"))
        {
            AddNewResetItem();
        }

        GUI.enabled = true;
        serializedObject.ApplyModifiedProperties();
    }

    private void MonitorTransformChange()
    {
        if (!resetTarget) return;

        // Only run in Edit Mode
        if (Application.isPlaying) return;

        Transform t = resetTarget.transform;

        if (t.position != lastPosition || t.rotation != lastRotation)
        {
            for (int i = 0; i < itemsProp.arraySize; i++)
            {
                var item = itemsProp.GetArrayElementAtIndex(i);
                var actionType = (ResetObject.ResetActionType)item.FindPropertyRelative("actionType").enumValueIndex;

                if (actionType == ResetObject.ResetActionType.TransformReset)
                {
                    item.FindPropertyRelative("initialPosition").vector3Value = t.position;
                    item.FindPropertyRelative("initialRotation").quaternionValue = t.rotation;
                    item.FindPropertyRelative("activeOnReset").boolValue = resetTarget.gameObject.activeSelf;

                    serializedObject.ApplyModifiedProperties();
                    break;
                }
            }

            lastPosition = t.position;
            lastRotation = t.rotation;
        }
    }

    void DrawTransformInfo(SerializedProperty item)
    {
        var pos = item.FindPropertyRelative("initialPosition").vector3Value;
        var rot = item.FindPropertyRelative("initialRotation").quaternionValue;
        var active = item.FindPropertyRelative("activeOnReset").boolValue;

        EditorGUILayout.HelpBox($"Initial Position: {pos}", MessageType.None);
        EditorGUILayout.HelpBox($"Initial Rotation: {rot.eulerAngles}", MessageType.None);
        EditorGUILayout.HelpBox($"Active on Reset: {active}", MessageType.None);
    }

    void AddNewResetItem()
    {
        var existingTypes = Enumerable.Range(0, itemsProp.arraySize)
            .Select(i => (ResetObject.ResetActionType)itemsProp.GetArrayElementAtIndex(i).FindPropertyRelative("actionType").enumValueIndex)
            .ToList();

        foreach (ResetObject.ResetActionType type in Enum.GetValues(typeof(ResetObject.ResetActionType)))
        {
            if (!preventDuplicates || !existingTypes.Contains(type))
            {
                itemsProp.InsertArrayElementAtIndex(itemsProp.arraySize);
                var newItem = itemsProp.GetArrayElementAtIndex(itemsProp.arraySize - 1);
                newItem.FindPropertyRelative("actionType").enumValueIndex = (int)type;
                break;
            }
        }
    }
}
