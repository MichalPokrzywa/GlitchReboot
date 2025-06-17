using System.Collections;
using UnityEngine;

public class ParkourCoverLevel2 : PuzzleBase
{
    [Header("PuzzleItems")]
    public GameObject Box;

    public override void DoTerminalCode()
    {
        float scale = GetVariableValue<int>("Reducing_Scale") == 0 ? 1f : 1f/GetVariableValue<int>("Reducing_Scale");
        Debug.Log(GetVariableValue<int>("Reducing_Scale"));
        StartCoroutine(ScaleOverTime(Box.transform, new Vector3(scale, scale, scale), 1.5f));
    }

    private IEnumerator ScaleOverTime(Transform obj, Vector3 targetScale, float duration)
    {
        Vector3 startScale = obj.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        obj.localScale = targetScale; // Ustaw dok³adnie koñcow¹ skalê na koñcu
    }

}