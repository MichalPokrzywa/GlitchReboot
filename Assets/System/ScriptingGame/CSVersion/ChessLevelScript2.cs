using DG.Tweening;
using UnityEngine;
using System.Collections;

public class ChessLevelScript2 : PuzzleBase
{
    public GameObject bridge1;
    public GameObject bridge2;
    public GameObject bridge3;
    public GameObject bridgeL;
    public GameObject bridgeR;

    public GameObject navMeshSurface;

    public GameObject pieceToMove;
    [Header("PieceToRemove")]
    public GameObject pieceToRemove;
    private bool once = true;

    private float bridgeUpY = 37.95f;
    private float bridgeDownY = 32.90855f; // Adjust as needed
    private float moveDuration = 0.5f;

    public override void DoTerminalCode()
    {
        if (bridge1 == null || bridge2 == null || bridge3 == null || bridgeL == null || bridgeR == null)
        {
            Debug.Log("Szachowy Terminal2 - some bridge is null");
            return;
        }
        Debug.Log("Szachowy Terminal2");
        bool a = GetVariableValue<bool>("a");
        bool b = GetVariableValue<bool>("b");
        bool c = GetVariableValue<bool>("c");
        bool d = GetVariableValue<bool>("d");
        bool e = GetVariableValue<bool>("e");

        Debug.Log("a - " + a);
        Debug.Log("b - " + b);
        Debug.Log("c - " + c);
        Debug.Log("d - " + d);
        Debug.Log("e - " + e);

        /*        bridge1.SetActive(a);   
                bridge2.SetActive(a && d);  
                bridge3.SetActive(!e && c && a);
                bridgeR.SetActive(!a && (b || e));
                bridgeL.SetActive(a && e);*/

        AnimateBridge(bridge1, a);
        AnimateBridge(bridge2, a && d);
        AnimateBridge(bridge3, !e && c && a);
        AnimateBridge(bridgeR, !a && (b || e));
        AnimateBridge(bridgeL, a && e);

        navMeshSurface.GetComponent<FilterNavMeshSurface>().ReBuildNavMesh();
        if (once && a && d)
        {
            once = false;
            Vector3 newPosition = pieceToMove.transform.position + new Vector3(5, 0, -5);
            pieceToMove.transform.DOMove(newPosition, 1, false);
            StartCoroutine(DelayedDeactivate(pieceToRemove, 0.3f));
        }
    }
    private void AnimateBridge(GameObject bridge, bool show)
    {
        if (bridge == null) return;

        float targetY = show ? bridgeDownY : bridgeUpY;

        bridge.transform.DOLocalMoveY(targetY, moveDuration);
    }

    private IEnumerator DelayedDeactivate(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}

