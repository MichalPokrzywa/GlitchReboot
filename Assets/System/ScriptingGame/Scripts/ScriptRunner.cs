using System;
using MoonSharp.Interpreter;
using UnityEngine;

public class ScriptRunner : Singleton<ScriptRunner>
{
    private readonly Script script = new Script();
    void Awake()
    {
        Script.DefaultOptions.DebugPrint = s => Debug.Log(s);
        //script.Globals["print"] = (Action<DynValue>)CustomPrint;
    }

    //void Start()
    //{
    //    //script.DoString("print('Its working')");
    //    Debug.Log(Application.streamingAssetsPath);
    //}

    private void CustomPrint(DynValue value)
    {
        Debug.Log(value.ToPrintString());
    }

    public Script GetScript()
    {
        return script;
    }
}
