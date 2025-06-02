using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Hologram : MonoBehaviour
{
    [SerializeField] float minGlitchStrength = 0.1f;
    [SerializeField] float maxGlitchStrength = 0.25f;
    [SerializeField] float minGlitchLength = 0.1f;
    [SerializeField] float maxGlitchLength = 0.5f;
    [SerializeField] float minTimeBetweenGlitches = 4f;
    [SerializeField] float maxTimeBetweenGlitches = 8f;

    Renderer renderer;
    Coroutine hologramCoroutine;

    void OnEnable()
    {
        if (renderer == null)
            renderer = GetComponent<Renderer>();

        hologramCoroutine = StartCoroutine(Glitch());
    }

    void OnDisable()
    {
        StopGlitch();
    }

    void OnDestroy()
    {
        StopGlitch();
    }

    void StopGlitch()
    {
        if (hologramCoroutine != null)
        {
            StopCoroutine(hologramCoroutine);
            hologramCoroutine = null;
        }
    }

    IEnumerator Glitch()
    {
        if (renderer == null)
        {
            StopGlitch();
            yield break;
        }

        renderer.material.SetFloat("_GlitchStrength", 0f);

        while (true)
        {
            float timeBetweenGlitches = Random.Range(minTimeBetweenGlitches, maxTimeBetweenGlitches);
            yield return new WaitForSeconds(timeBetweenGlitches);
            float glitchStrength = Random.Range(minGlitchStrength, maxGlitchStrength);
            renderer.material.SetFloat("_GlitchStrength", glitchStrength);
            float glitchLength = Random.Range(minGlitchLength, maxGlitchLength);
            yield return new WaitForSeconds(glitchLength);
            renderer.material.SetFloat("_GlitchStrength", 0f);
        }

    }
}
