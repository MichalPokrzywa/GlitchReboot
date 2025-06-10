using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    private SpiderBehaviour spider;
    private SpiderBehaviour.TargetData data;
    private bool isEntered = false;
    public void Setup(SpiderBehaviour spider, SpiderBehaviour.TargetData data)
    {
        this.spider = spider;
        this.data = data;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isEntered && !spider.isTalking)
        {
            isEntered = true;
            spider.OnPlayerEnterTarget(data);
        }
    }
}
