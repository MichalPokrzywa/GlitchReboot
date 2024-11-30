using System;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;

public class TerminalScript : MonoBehaviour
{
    [SerializeField] 
    private string luaScriptFileName = "level_0";
    
    private string luaScript;
    private Script script = new();

    void Start()
    {
        if (LoadScriptContents())
        {
            SetupBaseVariables();
            RunScript();
        }
    }

    private bool LoadScriptContents()
    {
        string path = Path.Combine(Application.streamingAssetsPath, luaScriptFileName + ".lua");
        if (File.Exists(path))
        {
            luaScript = File.ReadAllText(path);
            Debug.Log("Loaded Lua Script: "+ luaScriptFileName);
            return true;
        }

        Debug.LogError("Failed to Load Lua Script!");
        return false;
    }

    private void SetupBaseVariables()
    {
        script = ScriptRunner.Instance.GetScript();
        script.Globals["x_value"] = 1;
        script.Globals["y_value"] = 1;
        script.Globals["CheckResult"] = (Action<bool>)CheckResult;
    }

    private void CheckResult(bool check)
    {
        if (check)
        {
            Debug.Log("Jest git");
        }
        else
        {
            Debug.Log("Nie jest git");
        }
    }


    public void RunScript()
    {
        script.DoString(luaScript);
    }

}
