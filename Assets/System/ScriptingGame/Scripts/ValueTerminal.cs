using MoonSharp.Interpreter;
using UnityEngine;

public class ValueTerminal : BaseTerminal
{
    public DynValue Result { get; private set; }

    protected override void OnVariableChanged()
    {
        script.DoString(luaScript);
        DynValue func = script.Globals.Get(functionName);
        var result = script.Call(func);
        Debug.Log("Lua returned: " + result);
    }
}
