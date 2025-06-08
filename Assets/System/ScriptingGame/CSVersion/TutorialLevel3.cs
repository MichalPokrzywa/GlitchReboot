using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevel3 : PuzzleBase
{
    [Header("PuzzleItems")]
    public Destructable wall;

    public ParticleSystem boom;
    public List<Light> lights;
    public Material turnOffMaterial;
    private bool isBoomDone = false;
    private float waitTimer = 3f;
    private Coroutine boomCoroutine = null;
    
    public override void DoTerminalCode()
    {
        bool start = GetVariableValue<bool>("Boom");


        if (isBoomDone)
            return;

        if (start)
        {
            if (boomCoroutine == null)
            {
                boomCoroutine = StartCoroutine(BoomCountdown());
            }
        }
        else
        {
            // Reset timer if start is false
            if (boomCoroutine != null)
            {
                StopCoroutine(boomCoroutine);
                boomCoroutine = null;
            }
        }
        Debug.Log($"{boomCoroutine} {start}");

    }

    private IEnumerator BoomCountdown()
    {
        yield return new WaitForSeconds(waitTimer);
        isBoomDone = true;

        // Do your boom logic here
        boom.Play(true);
        yield return new WaitForSeconds(0.1f);
        foreach (Light l in lights)
        {
            l.enabled = false;
            l.gameObject.GetComponentInParent<Renderer>().material = turnOffMaterial;
        }
        wall.Explode(null);
        boomCoroutine = null;
    }
}