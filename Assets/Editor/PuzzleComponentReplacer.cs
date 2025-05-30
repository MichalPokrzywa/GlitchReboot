using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class PuzzleComponentReplacer : MonoBehaviour
{
    [MenuItem("CONTEXT/PuzzleBase/Replace With New or Existing Derived Type...")]
    static void ReplaceWithNewOrCreate(MenuCommand command)
    {
        ReplacePuzzleComponentWindow.Init(command.context as Component);
    }

}
public class ReplacePuzzleComponentWindow : EditorWindow
{
    private string newTypeName = "";
    private Component oldComponent;
    private const string scriptFolderPath = "Assets/System/ScriptingGame/CSVersion";

    public static void Init(Component target)
    {
        var window = GetWindow<ReplacePuzzleComponentWindow>("Replace Puzzle Component");
        window.oldComponent = target;
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Enter name of the new PuzzleBase-derived class:", EditorStyles.boldLabel);
        newTypeName = EditorGUILayout.TextField("New Class Name", newTypeName);

        if (GUILayout.Button("Replace or Create"))
        {
            if (string.IsNullOrWhiteSpace(newTypeName))
            {
                EditorUtility.DisplayDialog("Error", "Class name cannot be empty.", "OK");
                return;
            }

            var foundType = FindDerivedType(newTypeName);
            if (foundType != null)
            {
                ReplaceComponent(foundType);
            }
            else
            {
                CreateNewScript(newTypeName);
                Close(); // Close the window now; replacement will occur after compilation.
            }
        }
    }

    Type FindDerivedType(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == name && typeof(PuzzleBase).IsAssignableFrom(t) && !t.IsAbstract);
    }

    void ReplaceComponent(Type newType)
    {
        GameObject go = oldComponent.gameObject;
        SerializedObject oldSerialized = new SerializedObject(oldComponent);
        SerializedProperty iterator = oldSerialized.GetIterator();

        Undo.RecordObject(go, "Replace Puzzle Component");
        Component newComponent = Undo.AddComponent(go, newType);

        SerializedObject newSerialized = new SerializedObject(newComponent);
        while (iterator.NextVisible(true))
        {
            if (iterator.name == "m_Script") continue;
            newSerialized.CopyFromSerializedProperty(iterator);
        }

        newSerialized.ApplyModifiedProperties();
        Undo.DestroyObjectImmediate(oldComponent);

        Debug.Log($"Replaced {oldComponent.GetType().Name} with {newType.Name}.");
    }

    void CreateNewScript(string className)
    {
        Directory.CreateDirectory(scriptFolderPath);
        string fullPath = Path.Combine(scriptFolderPath, $"{className}.cs");

        if (File.Exists(fullPath))
        {
            Debug.LogWarning($"Script {className}.cs already exists at {fullPath}");
            return;
        }

        string template = $@"using UnityEngine;

public class {className} : PuzzleBase
{{
    [Header(""PuzzleItems"")]
    public GameObject waypoint;

    public override void DoTerminalCode()
    {{
        Debug.Log(""This is Empty"");
    }}
}}";

        File.WriteAllText(fullPath, template);
        Debug.Log($"New PuzzleBase-derived script created at: {fullPath}");

        EditorPrefs.SetInt("ReplacePuzzle_ComponentGO", oldComponent.gameObject.GetInstanceID());
        EditorPrefs.SetString("ReplacePuzzle_NewTypeName", className);
        EditorPrefs.SetBool("ReplacePuzzle_PendingReplace", true);

        AssetDatabase.Refresh();
    }
}

[InitializeOnLoad]
public static class PuzzleComponentAutoReplacer
{
    static PuzzleComponentAutoReplacer()
    {
        if (EditorPrefs.GetBool("ReplacePuzzle_PendingReplace", false))
        {
            EditorApplication.update += TryReplace;
        }
    }

    static void TryReplace()
    {
        if (!EditorPrefs.GetBool("ReplacePuzzle_PendingReplace")) return;

        int goId = EditorPrefs.GetInt("ReplacePuzzle_ComponentGO", -1);
        string typeName = EditorPrefs.GetString("ReplacePuzzle_NewTypeName", "");

        GameObject go = EditorUtility.InstanceIDToObject(goId) as GameObject;
        if (go == null || string.IsNullOrEmpty(typeName)) return;

        var oldComponent = go.GetComponent<PuzzleBase>();
        if (oldComponent == null) return;

        var newType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == typeName && typeof(PuzzleBase).IsAssignableFrom(t));

        if (newType == null) return; // not yet compiled

        Undo.RecordObject(go, "Auto Replace Puzzle Component");
        var newComponent = Undo.AddComponent(go, newType);

        SerializedObject oldSerialized = new SerializedObject(oldComponent);
        SerializedObject newSerialized = new SerializedObject(newComponent);
        SerializedProperty iterator = oldSerialized.GetIterator();

        while (iterator.NextVisible(true))
        {
            if (iterator.name == "m_Script") continue;
            newSerialized.CopyFromSerializedProperty(iterator);
        }

        newSerialized.ApplyModifiedProperties();
        Undo.DestroyObjectImmediate(oldComponent);

        EditorPrefs.DeleteKey("ReplacePuzzle_PendingReplace");
        EditorPrefs.DeleteKey("ReplacePuzzle_NewTypeName");
        EditorPrefs.DeleteKey("ReplacePuzzle_ComponentGO");

        EditorApplication.update -= TryReplace;
        AssignButtonCallbacks(newComponent as PuzzleBase);
        Debug.Log($"✅ Automatically replaced with new class: {typeName}");
    }

    static void AssignButtonCallbacks(PuzzleBase puzzle)
    {
        if (puzzle == null) return;

        Transform terminal = puzzle.transform; // Adjust if terminal is nested differently

        var swapButton = terminal.Find("Swap Button")?.GetComponent<ButtonInteraction>();
        var copyButton = terminal.Find("Copy Button")?.GetComponent<ButtonInteraction>();

        if (swapButton != null)
        {
            swapButton.onButtonPressed.RemoveAllListeners();
            swapButton.onButtonPressed.AddListener(puzzle.SwapLanguage);
        }

        if (copyButton != null)
        {
            copyButton.onButtonPressed.RemoveAllListeners();
            copyButton.onButtonPressed.AddListener(puzzle.SendToTablet);
        }

        Debug.Log("🟢 Button events assigned: SwapLanguage and SendToTablet.");
    }
}