using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BossFight : MonoBehaviour
{

    [SerializeField] private int scale =200;
    [SerializeField] private float duration =5;
    private Vector3 bossPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BossFightBegin();
        bossPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BossFightBegin()
    {
        StartCoroutine(ScaleOverTime(this.transform, new Vector3(scale, scale, scale), duration));
    }

    private IEnumerator ScaleOverTime(Transform obj, Vector3 targetScale, float duration)
    {
        Vector3 startScale = obj.localScale;
        float elapsed = 0f;
        this.GetComponent<GlitchSwitcher>().ApplyGlitch(true);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            obj.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        this.GetComponent<SpiderProceduralAnimation>().stepSize *= scale;
        this.GetComponent<GlitchSwitcher>().ApplyGlitch(false);
        obj.localScale = targetScale; // Ustaw dok³adnie koñcow¹ skalê na koñcu
    }
}
