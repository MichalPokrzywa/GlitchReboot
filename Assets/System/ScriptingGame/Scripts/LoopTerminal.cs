using MoonSharp.Interpreter;
using UnityEngine;

public class LoopTerminal : BaseTerminal
{
    private DynValue loopFunction;
    protected override void OnVariableChanged()
    {
        
        foreach (VariablePlatform platform in allLevelPlatforms)
        {
            if (script.Globals[platform.variableName] == null)
            {
                ResetAssignedElements();
                loopFunction = null;
                ShowCodeIsWorking(false);
                return;
            }

        }
        script.DoString(luaScript);
        try
        {
            DynValue func = script.Globals.Get(functionName);
            if (func.Type == DataType.Function)
            {
                loopFunction = func;
                ShowCodeIsWorking(true);
                Debug.Log("Lua returned: " + func);
            }
            else
            {
                Debug.LogWarning($"Lua function '{functionName}' not found or not callable. Type: {func.Type}");
            }
        }
        catch (ScriptRuntimeException ex)
        {
            ShowCodeIsWorking(false);
            Debug.LogError("Lua runtime error: " + ex.DecoratedMessage);
        };
    }

    void Update()
    {
        if (loopFunction != null)
        {
            script.Call(loopFunction, DynValue.NewNumber(Time.deltaTime));
        }
    }

}
