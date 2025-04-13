using MoonSharp.Interpreter;
using UnityEngine;

public class VoidTerminal : BaseTerminal
{
    protected override void OnVariableChanged()
    {
        script.DoString(luaScript);
        DynValue func = script.Globals.Get(functionName);
        var result = script.Call(func);
        if (!Equals(result, DynValue.Void))
        {
            Debug.Log("Lua returned: " + result);
        }
        else
            Debug.Log("Lua did code: ");
    }
}
