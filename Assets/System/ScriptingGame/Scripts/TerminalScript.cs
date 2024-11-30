using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;

public class TerminalScript : MonoBehaviour
{
    [SerializeField] 
    private string luaScriptFileName = "level_0";
    
    private string luaScript;
    private Script script = new();

    public List<VariablePlatform> allLevelPlatforms = new();
    void Start()
    {
        if (LoadScriptContents())
        {
            SetupBaseVariables();
            //RunScript();
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
        foreach (VariablePlatform platform in allLevelPlatforms)
        {
            script.Globals[platform.variableName] = null;
            platform.variableAdded.AddListener(UpdateVariable);
            platform.variableRemoved.AddListener(RemoveVariable);
        }
        script.Globals["CheckResult"] = (Action<bool>)CheckResult;
    }

    private void UpdateVariable(string variableName,object value)
    {
        script.Globals[variableName] = value;
        Debug.Log(value);
        RunScript();
    }

    private void RemoveVariable(string variableName)
    {
        script.Globals[variableName] = null;
        RunScript();
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
