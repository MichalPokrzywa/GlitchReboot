using MoonSharp.Interpreter;
using UnityEngine;

[CreateAssetMenu(menuName = "Lua/API/LuaApiTest")]
[MoonSharpUserData]
public class LuaApiTest : LuaApi
{
    public static void TestFunction(int number)
    {
        Debug.Log($"Number: {number}!!!!");
    }
}