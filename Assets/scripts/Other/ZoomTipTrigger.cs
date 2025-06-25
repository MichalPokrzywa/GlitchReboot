using UnityEngine;

public class ZoomTipTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PanelManager.Instance.ShowTipsOnce(TipsPanel.eTipType.Zoom);
        }
    }
}
