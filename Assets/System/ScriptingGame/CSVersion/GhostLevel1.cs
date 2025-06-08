using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class GhostLevel1 : PuzzleBase
{
    [Header("PuzzleItems")]
    [SerializeField] private Transform stairsCover;
    [SerializeField] private Transform stairs;
    [SerializeField] private GameObject floor;
    private Vector3 stairsBasicPosition;
    private Vector3 stairsCoverBasicPosition;

    public void Awake()
    {
        stairsBasicPosition = stairs.transform.position;
        stairsCoverBasicPosition = stairsCover.transform.position;
    }

    public override void DoTerminalCode()
    {
        if (GetVariableValue<bool>("StairsPlatform"))
        {
            StartCoroutine(SmoothMoveStairs(stairsCover, new Vector3(stairsCoverBasicPosition.x, stairsCoverBasicPosition.y, stairsCoverBasicPosition.z + 12f), 1.5f));
            StartCoroutine(FloorGlitch(1.6f));
            StartCoroutine(SmoothMoveStairs(stairs, new Vector3(stairsBasicPosition.x, stairsBasicPosition.y + 12f, stairsBasicPosition.z), 1.5f));
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
}