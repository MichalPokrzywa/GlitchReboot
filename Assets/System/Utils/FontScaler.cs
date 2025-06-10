using TMPro;
using UnityEngine;

public class FontScaler : MonoBehaviour
{
    public TextMeshProUGUI tmpText; // TMP Text
    public int baseFontSize = 36;
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    void Start()
    {
        float scaleFactor = Mathf.Min((float)Screen.width / referenceResolution.x,
                                      (float)Screen.height / referenceResolution.y);
        tmpText.fontSize = baseFontSize * scaleFactor;
    }
}
