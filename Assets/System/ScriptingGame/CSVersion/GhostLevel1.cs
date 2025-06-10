using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class GhostLevel1 : PuzzleBase
{
    [Header("PuzzleItems")]
    [SerializeField] private Transform stairsCover;
    [SerializeField] private Transform stairs;
    [SerializeField] private Transform bridge;
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject platform;
    private Transform platformTransform;
    private Vector3 stairsBasicPosition;
    private Vector3 stairsCoverBasicPosition;
    private Vector3 bridgeBasicPosition;
    private Vector3 platformBasicPosition;
    private bool stairsState = false;
    private bool bridgeState = false;
    private bool platformState = false;
    private float duration = 6f;

    public void Awake()
    {
        stairsBasicPosition = stairs.position;
        stairsCoverBasicPosition = stairsCover.position;
        bridgeBasicPosition = bridge.position;
        platformBasicPosition = platform.transform.Find("Waypoints").Find("Waypoint").position;
        platformTransform = platform.transform.Find("Waypoints").Find("Waypoint").transform;
    }

    public override void DoTerminalCode()
    {
        if (GetVariableValue<bool>("StairsPlatform"))
        {
            stairsState = true;
            StartCoroutine(SmoothMoveStairs(stairsCover, new Vector3(stairsCoverBasicPosition.x, stairsCoverBasicPosition.y, stairsCoverBasicPosition.z + 12f), duration));
            StartCoroutine(FloorGlitch(duration+0.2f));
            StartCoroutine(SmoothMoveStairs(stairs, new Vector3(stairsBasicPosition.x, stairsBasicPosition.y + 12f, stairsBasicPosition.z), duration));
        }else if (GetVariableValue<bool>("BridgePlatform"))
        {
            bridgeState = true;
            StartCoroutine(SmoothMoveStairs(bridge, new Vector3(bridgeBasicPosition.x, bridgeBasicPosition.y, bridgeBasicPosition.z + 18f), duration));
            StartCoroutine(BridgeGlitch(duration + 0.2f));
        }else if (!GetVariableValue<bool>("StairsPlatform") && stairsState)
        {
            stairsState = false;
            StartCoroutine(SmoothMoveStairs(stairsCover, new Vector3(stairsCoverBasicPosition.x, stairsCoverBasicPosition.y, stairsCoverBasicPosition.z ), duration));
            StartCoroutine(FloorGlitch(duration + 0.2f));
            StartCoroutine(SmoothMoveStairs(stairs, new Vector3(stairsBasicPosition.x, stairsBasicPosition.y , stairsBasicPosition.z), duration));
        }else if (!GetVariableValue<bool>("BridgePlatform") && bridgeState)
        {
            bridgeState = false;
            StartCoroutine(SmoothMoveStairs(bridge, new Vector3(bridgeBasicPosition.x, bridgeBasicPosition.y, bridgeBasicPosition.z ), duration));
            StartCoroutine(BridgeGlitch(duration + 0.2f));
        }else if (GetVariableValue<bool>("MovingPlatform"))
        {
            platformState = true;
            platformTransform.position += new Vector3(0f, 0f, 16f);
            Debug.Log(platform.transform.position);
            Debug.Log(platformTransform.position);
        }else if(!GetVariableValue<bool>("MovingPlatform") && platformState)
        {
            platformState = false;
            platformTransform.position = platformBasicPosition;
        }
    }


    private IEnumerator SmoothMoveStairs(Transform transform, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Vector3.Lerp interpoluje liniowo miêdzy start a target.
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;  // Czekamy do nastêpnej klatki
        }

        // Upewniamy siê, ¿e obiekt trafia dok³adnie do docelowej pozycji.
        transform.position = targetPosition;
    }

    private IEnumerator FloorGlitch( float duration)
    {
        floor.GetComponent<GlitchSwitcher>().ApplyGlitch(true);
        stairs.GetComponent<GlitchSwitcher>().ApplyGlitch(true);
        stairsCover.GetComponent<GlitchSwitcher>().ApplyGlitch(true);
        yield return new WaitForSeconds(duration);
        floor.GetComponent<GlitchSwitcher>().ApplyGlitch(false);
        stairs.GetComponent<GlitchSwitcher>().ApplyGlitch(false);
        stairsCover.GetComponent<GlitchSwitcher>().ApplyGlitch(false);
    }

    private IEnumerator BridgeGlitch(float duration)
    {
        bridge.GetComponent<GlitchSwitcher>().ApplyGlitch(true);
        yield return new WaitForSeconds(duration);
        bridge.GetComponent<GlitchSwitcher>().ApplyGlitch(false);
    }
}