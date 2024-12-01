using System;
using UnityEngine;
using DG.Tweening; // DoTween namespace

public class moveObject : MonoBehaviour
{
    // Punkt początkowy
    public Vector3 startPoint;

    // Punkt końcowy
    public Vector3 endPoint;

    // Czas trwania ruchu w sekundach
    public float duration = 2.0f;

    private RectTransform rectTransform;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        StartMoving();
    //    }
    //}

    void Start()
    {
        
        // Pobierz komponent RectTransform
        rectTransform = GetComponent<RectTransform>();

        // Ustaw obiekt w punkcie początkowym i ukryj go
        rectTransform.anchoredPosition = startPoint;
    }

    // Rozpocznij ruch
    public void StartMoving()
    {
        // Upewnij się, że obiekt jest aktywny
        gameObject.SetActive(true);

        // Przesuń obiekt z punktu startowego do końcowego
        rectTransform.anchoredPosition = startPoint;
        rectTransform.DOAnchorPos(endPoint, duration).OnComplete(() =>
        {
            // Po zakończeniu ruchu ukryj obiekt
            Debug.Log("KURTYNA!");
            //gameObject.SetActive(false);
        });
    }
}
