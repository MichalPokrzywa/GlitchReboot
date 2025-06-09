using UnityEngine;

public class ChessLevelScript2 : PuzzleBase
{
    public GameObject bridge1;
    
    public override void DoTerminalCode()
    {
        Debug.Log("Szachowy Terminal2");
        bool leftPlatform1 = GetVariableValue<bool>("platform_left1");
        if (bridge1 != null)
        {
            bridge1.SetActive(leftPlatform1);
        }        
    }
}

