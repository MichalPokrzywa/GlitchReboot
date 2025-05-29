using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class DoorLua : LuaApi
{
    public static void OpenDoors(GameObject doors, bool shouldClosed)
    {
        doors.SetActive(shouldClosed);
    }

    public static void Nonsense(GameObject doors, bool shouldOpen)
    {
        Debug.Log("Wywolalo:");
        Debug.Log(doors);
        Debug.Log(shouldOpen);
        doors.SetActive(shouldOpen);
    }
}
