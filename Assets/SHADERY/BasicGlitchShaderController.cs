using UnityEngine;

public class BasicGlitchShaderController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Material material;
    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
    }

    public void setCheckerBoardGlitchBool(bool value)
    {
        material.SetInt("_CheckerBoardGlitchBool", value ? 1 : 0);
    }
    
    public void setGlitchActivationBool(bool value)
    {
        material.SetInt("_GlitchActivationBool", value ? 1 : 0);
    }

    public void setGlitchInvertedStrength(float value)
    {
        // Inverted znaczy że im mniej dacie tym mocniejszy będzie efekt, 0 spowoduje brak efektu, 1 efekt słabszy
        // -1 da efekt mocniejszy, wartość bazowa (tj. -0.15) powinna być w sam raz 
        material.SetFloat("_GlitchInvertedStrength", value);
    }
}
