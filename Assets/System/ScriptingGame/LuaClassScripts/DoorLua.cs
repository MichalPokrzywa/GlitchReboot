using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class DoorLua : LuaApi
{
    public static void OpenDoors(GameObject doors, bool shouldOpen)
    {
        doors.SetActive(shouldOpen);
    }
}
