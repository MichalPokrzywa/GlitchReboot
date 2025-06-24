using System.Collections;
using UnityEngine;

public class SpiderCutscene : MonoBehaviour
{
    public GameObject cage;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cage.SetActive(true);
        }
    }

    IEnumerable CutsceneTime()
    {
        yield return new WaitForSeconds(10f);
    }
}
