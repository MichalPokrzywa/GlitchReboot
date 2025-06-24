using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    private SpiderBehaviour spider;
    private SpiderBehaviour.TargetData data;

    public void Setup(SpiderBehaviour spider, SpiderBehaviour.TargetData data)
    {
        this.spider = spider;
        this.data = data;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spider.OnPlayerEnterTarget(data);
        }
    }
}
