using System;
using System.Collections;
using UnityEngine;

public class SpiderCutscene : MonoBehaviour
{
    public GameObject cage;
    public SpiderBehaviour spider;
    public Material material;
    public FirstPersonController controller;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CutsceneTime());
        }
    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(0.4f);
        controller.StopMovement();
        controller.transform.LookAt(spider.transform);
        cage.SetActive(true);
        GlitchEffectFeature.Instance.UpdateIntensity(0.2f);
        yield return new WaitForSeconds(10f);
    }
}
