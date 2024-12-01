using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    public List<Ghost> ghost;
    public List<GameObject> gameObjects;
    private int[] state;
    private bool isRecording = false;
    private bool isHeld = false;
    private float timer;
    private float timeValue;
    private float[] buttonHoldTimers = new float[4]; // Timery dla ka¿dego przycisku
    private int ghostIndex = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        state = new int[4] { 0, 0, 0, 0 };
        for (int i = 0; i < 4; i++)
        {
            ghost[i].RestetData();
            ghost[i].setIsRecord(false);
            ghost[i].setIsReplay(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        for (int i = 0; i < 4; i++)
        {
            if(ghost[i].getIsRecord() & timer >= 1 / ghost[i].recordFrequancy)
            {
                ghost[i].timeStamp.Add(timeValue);
                ghost[i].position.Add(this.transform.position);
                ghost[i].rotation.Add(this.transform.eulerAngles);
                timer = 0;
            }
        }


        // Obs³uga naciœniêcia przycisków
        if (Input.GetKeyUp(KeyCode.Alpha1)) OnButton1Pressed();
        if (Input.GetKeyUp(KeyCode.Alpha2)) OnButton2Pressed();
        if (Input.GetKeyUp(KeyCode.Alpha3)) OnButton3Pressed();
        if (Input.GetKeyUp(KeyCode.Alpha4)) OnButton4Pressed();

        // Obs³uga przytrzymania przycisków
        HandleButtonHold(KeyCode.Alpha1, 0, OnButton1Held);
        HandleButtonHold(KeyCode.Alpha2, 1, OnButton1Held);
        HandleButtonHold(KeyCode.Alpha3, 2, OnButton1Held);
        HandleButtonHold(KeyCode.Alpha4, 3, OnButton1Held);

    }
    private void HandleButtonHold(KeyCode key, int index, System.Action onHoldAction)
    {
        if (Input.GetKey(key))
        {
            buttonHoldTimers[index] += Time.deltaTime; // Licz czas przytrzymania
            if (buttonHoldTimers[index] >= 2f) // SprawdŸ, czy przytrzymano 2 sekundy
            {
                isHeld = true;
                onHoldAction?.Invoke();
                buttonHoldTimers[index] = 0f; // Zresetuj timer, aby unikn¹æ wielokrotnego wywo³ania
            }
        }
        else
        {
            buttonHoldTimers[index] = 0f; // Zresetuj timer, gdy przycisk nie jest trzymany
        }
    }

    // Metody wywo³ywane przy naciœniêciu
    private void OnButton1Pressed() 
    {
        if (isHeld)
        {
            isHeld = false;
            return;
        }
        ghostIndex = 0;
        changeState();
    }
    private void OnButton2Pressed() 
    {
        if (isHeld) 
        {
            isHeld = false;
            return;
        }
        ghostIndex = 1;
        changeState();
    }
    private void OnButton3Pressed() 
    {
        if (isHeld)
        {
            isHeld = false;
            return;
        }
        ghostIndex = 2;
        changeState();
    }
    private void OnButton4Pressed()
    {
        if (isHeld) 
        {
            isHeld = false;
            return;
        }
        ghostIndex = 3;
        changeState();
    }

    public void changeState()
    {
        switch (state[ghostIndex])
        {
            case 0:
                if (!isRecording)
                {
                    state[ghostIndex]++;
                    ghost[ghostIndex].setIsRecord(true);
                    isRecording = true;
                    Debug.Log(state[ghostIndex]);
                }
                break;
            case 1:
                state[ghostIndex]++;
                ghost[ghostIndex].setIsRecord(false);
                isRecording = false;
                Debug.Log(state[ghostIndex]);
                break;
            case 2:
                state[ghostIndex]++;
                gameObjects[ghostIndex].SetActive(true);
                ghost[ghostIndex].setIsReplay(true);
                StartCoroutine(WaitForSecondsCoroutine(ghost[ghostIndex].timeStamp[ghost[ghostIndex].timeStamp.Count - 1] - ghost[ghostIndex].timeStamp[0], ghostIndex));
                break;
        }
    }

    // Metody wywo³ywane przy przytrzymaniu
    private void OnButton1Held() 
    {
        state[ghostIndex] = 0;
        ghost[ghostIndex].RestetData();
        ghost[ghostIndex].setIsRecord(false);
        ghost[ghostIndex].setIsReplay(false);
    }

    IEnumerator WaitForSecondsCoroutine(float waitTime, int index_)
    {
        yield return new WaitForSeconds(waitTime);
        gameObjects[index_].transform.position = Vector3.zero;
        gameObjects[index_].transform.eulerAngles = Vector3.zero;
        gameObjects[index_].GetComponent<GhostPlayer>().Reset();
        ghost[index_].setIsReplay(false);
        gameObjects[index_].SetActive(false);

    }
}
