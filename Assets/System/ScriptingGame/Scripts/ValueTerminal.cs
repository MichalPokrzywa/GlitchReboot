using MoonSharp.Interpreter;
using UnityEngine;

public class ValueTerminal : BaseTerminal
{
    public DynValue Result { get; private set; }

    protected override void OnVariableChanged()
    {
        foreach (VariablePlatform platform in allLevelPlatforms)
        {
            if (script.Globals[platform.variableName] == null)
            {
                ResetAssignedElements();
                return;
            }

        }
        script.DoString(luaScript);
        try
        {
            DynValue func = script.Globals.Get(functionName);
            if (func.Type == DataType.Function)
            {
                var result = script.Call(func);
                if (!Equals(result, DynValue.Void))
                {
                    Debug.Log("Lua returned: " + result);
                }
                else
                {
                    Debug.Log("Lua did code: ");
                }
            }
            else
            {
                Debug.LogWarning($"Lua function '{functionName}' not found or not callable. Type: {func.Type}");
            }
        }
        catch (ScriptRuntimeException ex)
        {
            Debug.LogError("Lua runtime error: " + ex.DecoratedMessage);
        };
    }
}
