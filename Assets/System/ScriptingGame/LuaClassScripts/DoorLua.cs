using MoonSharp.Interpreter;
using UnityEngine;

[CreateAssetMenu(menuName = "Lua/API/Doors")]
[MoonSharpUserData]
public class DoorLua : LuaApi
{
    public void OpenDoors(GameObject doors, bool shouldOpen)
    {
        doors.SetActive(shouldOpen);
    }
}
