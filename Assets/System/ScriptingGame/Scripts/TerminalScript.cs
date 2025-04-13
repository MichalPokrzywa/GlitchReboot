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

    public bool result = false; 
    public TerminalCanvas canvas;
    public List<VariablePlatform> allLevelPlatforms = new();
    void Start()
    {
        if (LoadScriptContents())
        {
            SetupBaseVariables();
            PrepareTerminal();
            //RunScript();
        }
    }

    private void PrepareTerminal()
    {
       canvas.SetCodeText(luaScript);
       canvas.SetVariableText(script.Globals, allLevelPlatforms);
       canvas.SetImageColor(result);
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
        //script.Globals.Clear();
        if (allLevelPlatforms.Count > 0)
        {
            foreach (VariablePlatform platform in allLevelPlatforms)
            {
                script.Globals[platform.variableName] = DynValue.Nil;
                platform.variableAdded.AddListener(UpdateVariable);
                platform.variableRemoved.AddListener(RemoveVariable);
            }
            script.Globals["CheckResult"] = (Action<bool>)CheckResult;
        }
    }

    private void UpdateVariable(string variableName,object value)
    {
        switch (value)
        {
            case bool newBool:
                Debug.Log($"{variableName} is a boolean: {value}");
                script.Globals[variableName] = DynValue.NewBoolean(newBool);
                break;
            case int newInt:
                Debug.Log($"{variableName} is a numeric type: {newInt}");
                script.Globals[variableName] = DynValue.NewNumber(newInt);
                break;
        }
        canvas.SetVariableText(script.Globals, allLevelPlatforms);
        Debug.Log($"{variableName} updated to: {value}, Type: {script.Globals[variableName]}");
        RunScript();
    }

    private void RemoveVariable(string variableName)
    {
        // Remove the variable entirely or set it to nil
        script.Globals[variableName] = DynValue.Nil;
        canvas.SetVariableText(script.Globals, allLevelPlatforms);
        Debug.Log($"{variableName} removed.");
        RunScript();
    }

    private void CheckResult(bool check)
    {
        Debug.Log(check);
        if (check)
        {
            Debug.Log("Jest git");
            //audioSource.Play();
            result = true;
            canvas.SetImageColor(result);
        }
        else
        {
            Debug.Log("Nie jest git");
            result = false;
            canvas.SetImageColor(result);
        }
    }


    public void RunScript()
    {
        foreach (VariablePlatform platform in allLevelPlatforms)
        {
            if (script.Globals[platform.variableName] == DynValue.Nil)
            {
                CheckResult(false);
                return;
            }
                
        }
        script.DoString(luaScript);
    }

}
