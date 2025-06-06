using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
using System.Linq;
using Unity.VisualScripting;

public class ChessLevelScript1 : PuzzleBase
{
    private bool once  = true;
    [Header("PlatfromObject")]
    public GameObject platform; 
    [Header("DestinationObject")]
    public GameObject destination;

    private Vector3 destPos;
    
    public override void DoTerminalCode()
    {
        Debug.Log("Szachowy Terminal1");
        if (!once) return;
        
        int platforms = GetVariableValue<int>("platform_num");
        
        destPos = destination.transform.position;

        float delta = (destPos.x - platform.transform.position.x) / platforms;

        for (int i = 1; i < platforms; i++)
        {
            GameObject nextPlatform = Instantiate(platform);

            //nextPlatform.transform.position = platform.transform.position;
            nextPlatform.SetActive(true);

            //nextPlatform.GetComponentInChildren<MovingPlatform>().zAxisReverie = platforms / 10;

            nextPlatform.GetComponentInChildren<MovingPlatform>().waypoints.First().transform.position = 
                nextPlatform.GetComponentInChildren<MovingPlatform>().waypoints.First().transform.position + new Vector3 (i* delta, 0.0f, 0.0f);


            nextPlatform.GetComponentInChildren<MovingPlatform>().startMoving = true;
            
        }
        once = false;
        Destroy(platform.gameObject);
    }
}

