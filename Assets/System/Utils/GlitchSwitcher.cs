using System.Collections.Generic;
using UnityEngine;

public class GlitchSwitcher : MonoBehaviour
{
    public Material glitchMaterial; // The material used for the glitch effect
    public bool glitchOnStart;
    public float strenght = -0.15f;
    public bool checkerBoxes = true;
    public bool glitchColor = true;
    private class RendererInfo
    {
        public Renderer renderer;
        public Material[] originalMaterials;
    }

    private List<RendererInfo> renderers = new List<RendererInfo>();

    void Awake()
    {
        CacheOriginalMaterials();
        ApplyGlitch(glitchOnStart);
    }

    private void CacheOriginalMaterials()
    {
        renderers.Clear();

        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            RendererInfo info = new RendererInfo
            {
                renderer = rend,
                originalMaterials = rend.materials
            };
            renderers.Add(info);
        }
    }

    public void ApplyGlitch(bool enable)
    {
        foreach (var info in renderers)
        {
            if (enable)
            {
                Material[] glitchMats = new Material[info.originalMaterials.Length];

                for (int i = 0; i < glitchMats.Length; i++)
                {
                    Material originalMat = info.originalMaterials[i];

                    // Create a fresh copy of glitchMaterial
                    Material glitchCopy = new Material(glitchMaterial);

                    // Copy the main texture from the original material
                    if (originalMat.HasProperty("_MainTex"))
                        glitchCopy.SetTexture("_Texture", originalMat.GetTexture("_MainTex"));

                    // Optionally copy color to preserve tint
                    if (originalMat.HasProperty("_Color"))
                        glitchCopy.SetColor("_Color", originalMat.GetColor("_Color"));

                    if(glitchCopy.HasProperty("_GlitchInvertedStrength"))
                        glitchCopy.SetFloat("_GlitchInvertedStrength", strenght);

                    if (glitchCopy.HasProperty("_CheckerBoardGlitchBool"))
                        glitchCopy.SetInt("_CheckerBoardGlitchBool", checkerBoxes ? 1 : 0);

                    if (glitchCopy.HasProperty("_GlitchColor"))
                        glitchCopy.SetInt("_GlitchColor", glitchColor ? 1 : 0);

                    glitchMats[i] = glitchCopy;
                }

                info.renderer.materials = glitchMats;
            }
            else
            {
                info.renderer.materials = info.originalMaterials;
            }
        }

        glitchOnStart = enable;
    }
}
