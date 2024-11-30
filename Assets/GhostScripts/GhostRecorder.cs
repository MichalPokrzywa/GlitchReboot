using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    public Ghost ghost;
    private float timer;
    private float timeValue;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (ghost.isRecord)
        {
            ghost.RestetData();
            timeValue = 0;
            timer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        if(ghost.isRecord & timer >= 1 / ghost.recordFrequancy)
        {
            ghost.timeStamp.Add(timeValue);
            ghost.position.Add(this.transform.position);
            ghost.rotation.Add(this.transform.eulerAngles);
            timer = 0;
        }
    }
}
