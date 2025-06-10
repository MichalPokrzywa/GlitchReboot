using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ChessLevelScript1 : PuzzleBase
{
    [Header("PlatfromObject")]
    public GameObject platform; 
    [Header("DestinationObject")]
    public GameObject destination;

    private Vector3 destPos;
    private int optimalVal = 13;
    private int minimalVal = 10;
    private float speed = 1;
    private int savedPlatforms = 0;
    private List<GameObject> platformList = new List<GameObject>();
    
    public override void DoTerminalCode()
    {
        Debug.Log("Szachowy Terminal1");
        int platforms = GetVariableValue<int>("platform_num");
        
        if (platforms == savedPlatforms) return;   
        
        if (savedPlatforms > 0) 
        { 
            for (int i = 0; i < platformList.Count; i++) 
            {
                Destroy(platformList[i]);
            }
            platformList.Clear();
        }

        savedPlatforms = platforms;
        
        destPos = destination.transform.position;

        float delta = (destPos.x - platform.transform.position.x) / platforms;

        float punish = 10.0f;
        if (platforms > minimalVal)
        {
            speed++;
            punish = 10.0f;
            if (platforms > optimalVal) 
            {
                punish -= (platforms - optimalVal) - 0.7f;
                speed += 0.2f;
            }
            if (punish < 2)
                punish = 1.0f;
        }

        for (int i = 1; i < platforms; i++)
        {
            GameObject nextPlatform = Instantiate(platform);

            nextPlatform.transform.position = platform.transform.position;

            platformList.Add(nextPlatform);

            //nextPlatform.transform.position = platform.transform.position;
            nextPlatform.SetActive(true);

            nextPlatform.GetComponentInChildren<MovingPlatform>().maxZOscilation = ((platforms / 2) - Math.Abs(i - platforms / 2)) / punish;
            nextPlatform.GetComponentInChildren<MovingPlatform>().zOscillationSpeed = speed;

            speed = -speed;
            nextPlatform.GetComponentInChildren<MovingPlatform>().waypoints.First().transform.position = 
                nextPlatform.GetComponentInChildren<MovingPlatform>().waypoints.First().transform.position + new Vector3 (i* delta, 0.0f, 0.0f);


            nextPlatform.GetComponentInChildren<MovingPlatform>().startMoving = true;
            
        }
        //Destroy(platform.gameObject);
    }
}

