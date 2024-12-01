using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    public Ghost ghost;
    public float timeValue;
    public int index1;
    public int index2;
    public bool start = false;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(ghost.getIsReplay());
        if (ghost.getIsReplay())
        {
            Debug.Log("replay is replaying");
            if (!start)
            {
                Debug.Log("start - true");
                start = true;
                timeValue = ghost.timeStamp[0];
            }
            timeValue += Time.unscaledDeltaTime;
            GetIndex();
            SetTransform();
        }
        if (!ghost.getIsReplay())
        {
            Debug.Log("start - false");
            start = false;
        }
    }
    public void Reset()
    {
        start = false;
        index1 = 0;
        index2 = 0;
    }
    private void GetIndex()
    {
        // Iteruj przez znaczniki czasu
        for (int i = 0; i < ghost.timeStamp.Count - 1; i++)
        {
            Debug.Log(index1 + " uwu " + index2);

            // SprawdŸ dok³adne dopasowanie (opcjonalne)
            if (Mathf.Abs(ghost.timeStamp[i] - timeValue) < 0.001f)
            {
                index1 = i;
                index2 = i;
                return;
            }

            // SprawdŸ, czy timeValue mieœci siê w przedziale [timeStamp[i], timeStamp[i + 1]]
            if (ghost.timeStamp[i] < timeValue && timeValue < ghost.timeStamp[i + 1])
            {
                index1 = i;
                index2 = i + 1;
                return;
            }
        }

        // Jeœli pêtla nie znajdzie przedzia³u, ustaw na ostatni element
        index1 = ghost.timeStamp.Count - 1;
        index2 = ghost.timeStamp.Count - 1;
    }

    private void SetTransform()
    {
        if (index1 == index2)
        {
            this.transform.position = ghost.position[index1];
            this.transform.eulerAngles = ghost.rotation[index1];
        }
        else
        {
            float interpolationFactor = (timeValue - ghost.timeStamp[index1]) /
                                        (ghost.timeStamp[index2] - ghost.timeStamp[index1]);

            this.transform.position = Vector3.Lerp(ghost.position[index1], ghost.position[index2], interpolationFactor);
            this.transform.eulerAngles = Vector3.Lerp(ghost.rotation[index1], ghost.rotation[index2], interpolationFactor);
        }
    }
}
