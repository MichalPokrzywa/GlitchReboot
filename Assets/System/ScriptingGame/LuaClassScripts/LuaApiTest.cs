using MoonSharp.Interpreter;
using UnityEngine;


[MoonSharpUserData]
public class LuaApiTest : LuaApi
{
    public static void TestFunction(int number)
    {
        Debug.Log($"Number: {number}!!!!");
    }
}