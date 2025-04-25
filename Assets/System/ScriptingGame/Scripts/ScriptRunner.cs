using System;
using System.Linq;
using MoonSharp.Interpreter;
using UnityEngine;

public class ScriptRunner : Singleton<ScriptRunner>
{
    private readonly Script script = new Script();
    void Awake()
    {
        Script.DefaultOptions.DebugPrint = s => Debug.Log(s);
        script.Globals["print"] = (Action<DynValue>)CustomPrint;
        script.Globals["Time"] = UserData.CreateStatic(typeof(Time));
        UserData.RegisterType<GameObject>();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Quaternion>();
        UserData.RegisterType<Time>();
        LoadLuaApi();
    }

    void Start()
    {
        //script.DoString("print('Its working')");
        Debug.Log(Application.streamingAssetsPath);
    }
    private void LoadLuaApi()
    {
        var luaApiType = typeof(LuaApi);
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => luaApiType.IsAssignableFrom(t) && t != luaApiType && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            Debug.Log(type.FullName);
            UserData.RegisterType(type);
            // script.Globals[type.Name] = UseData.Create(type);
            script.Globals[type.Name] = type;
        }
    }
    private void CustomPrint(DynValue value)
    {
        Debug.Log(value.ToPrintString());
    }

    public Script GetScript()
    {
        return script;
    }
}
