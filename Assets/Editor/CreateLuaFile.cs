using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateLuaFile : MonoBehaviour
{
    private static readonly string defaultLuaCode = "print(\"Hello Lua Code\")";

    [MenuItem("Assets/Create/Lua Script", false, 80)]
    public static void CreateLuaScript()
    {
        // Default folder path
        string folderPath = "Assets";

        // Check if the selected object is a folder
        if (Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject))
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(selectedPath))
            {
                folderPath = selectedPath;
            }
            else
            {
                // If a file is selected, use its folder
                folderPath = Path.GetDirectoryName(selectedPath);
            }
        }

        // Create a unique file path
        string fullPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/NewLuaScript.lua");

        // Write the file content
        File.WriteAllText(fullPath, defaultLuaCode);

        // Refresh and select the created file
        AssetDatabase.Refresh();
        var createdAsset = AssetDatabase.LoadAssetAtPath<Object>(fullPath);
        Selection.activeObject = createdAsset;
        EditorGUIUtility.PingObject(createdAsset);
    }
}