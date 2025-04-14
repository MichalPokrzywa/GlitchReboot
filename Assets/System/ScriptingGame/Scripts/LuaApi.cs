using MoonSharp.Interpreter;
using UnityEngine;

public abstract class LuaApi : ScriptableObject
{
    public string globalName = "API";

    // Register this object in Lua
    public virtual void RegisterTo(Script luaScript)
    {
        UserData.RegisterType(GetType());
        luaScript.Globals[globalName] = UserData.Create(this);
    }
}
