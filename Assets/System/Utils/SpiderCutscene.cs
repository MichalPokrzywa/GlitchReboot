using System;
using System.Collections;
using DG.Tweening;
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
        Camera.main.DOShakePosition(0.4f, 0.3f, randomnessMode: ShakeRandomnessMode.Harmonic);
        yield return new WaitForSeconds(0.3f);
        controller.StopMovement();
        controller.transform.LookAt(spider.transform);
        cage.SetActive(true);
        GlitchEffectFeature.Instance.UpdateIntensity(0.1f);
        yield return new WaitForSeconds(17.5f);
        GlitchEffectFeature.Instance.UpdateIntensity(0.2f);
        spider.GetComponent<GlitchSwitcher>().ApplyGlitch(true);
        yield return new WaitForSeconds(9.5f);
        controller.transform.DOLocalRotate(new Vector3(0, 0, 90), 1.5f).SetEase(Ease.InQuart);
        yield return new WaitForSeconds(7.5f);
        if (material.HasProperty("_GlitchInvertedStrength"))
            material.SetFloat("_GlitchInvertedStrength", 0.1f);
        GlitchEffectFeature.Instance.UpdateIntensity(0.4f);
        yield return new WaitForSeconds(2.5f);
        DependencyManager.sceneLoader.LoadScene(Scene.MainMenu);
        yield return new WaitForSeconds(1f);
        GlitchEffectFeature.Instance.UpdateIntensity(0f);


    }
}
