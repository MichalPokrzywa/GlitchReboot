using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlitchEffectFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        [Range(0f, 1f)] public float intensity = 0.5f;
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public Settings settings = new Settings();
    DigitalGlitchPass renderPass;
    public static GlitchEffectFeature Instance { get; private set; }
    public override void Create()
    {
        if (settings.material == null)
        {
            Debug.LogWarning("DigitalGlitch material not assigned.");
            return;
        }
        renderPass = new DigitalGlitchPass(settings.material, settings.intensity)
        {
            renderPassEvent = settings.renderPassEvent
        };
        Instance = this;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material != null)
            renderer.EnqueuePass(renderPass);
    }

    public void UpdateIntensity(float value)
    {
        if (renderPass == null) return;
        renderPass._intensity = value;

    }

    class DigitalGlitchPass : ScriptableRenderPass
    {
        const int NOISE_W = 64, NOISE_H = 32;
        const float TRASH_INTERVAL1 = 13f, TRASH_INTERVAL2 = 73f;

        Material _mat;
        public float _intensity;
        int frameCounter = 0;
        Texture2D noiseTex;
        RenderTexture trash1, trash2;
        static readonly int _NoiseId = Shader.PropertyToID("_NoiseTex");
        static readonly int _TrashId = Shader.PropertyToID("_TrashTex");
        static readonly int _IntensityId = Shader.PropertyToID("_Intensity");
        static readonly int _TempID = Shader.PropertyToID("_DigitalGlitchTempRT");

        public DigitalGlitchPass(Material mat, float intensity)
        {
            _mat = mat;
            _intensity = intensity;
            SetupResources();
        }

        void SetupResources()
        {
            noiseTex = new Texture2D(NOISE_W, NOISE_H, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                hideFlags = HideFlags.DontSave
            };
            trash1 = new RenderTexture(Screen.width, Screen.height, 0) { hideFlags = HideFlags.DontSave };
            trash2 = new RenderTexture(Screen.width, Screen.height, 0) { hideFlags = HideFlags.DontSave };
            UpdateNoiseTexture();
        }

        void UpdateNoiseTexture()
        {
            Color col = RandomColor();
            for (int y = 0; y < NOISE_H; y++)
                for (int x = 0; x < NOISE_W; x++)
                {
                    if (Random.value > 0.89f) col = RandomColor();
                    noiseTex.SetPixel(x, y, col);
                }
            noiseTex.Apply();
        }

        static Color RandomColor()
            => new Color(Random.value, Random.value, Random.value, Random.value);

        public override void Execute(ScriptableRenderContext context, ref RenderingData data)
        {
            if (_mat == null) return;
            frameCounter++;

            var cmd = CommandBufferPool.Get("DigitalGlitchEffect");
            var cameraTarget = data.cameraData.renderer.cameraColorTargetHandle;

            // 1) occasionally regenerate the noise texture based on intensity
            if (Random.value > Mathf.Lerp(0.9f, 0.5f, _intensity))
                UpdateNoiseTexture();

            // 2) swap trash frames every 13 / 73 frames
            if (frameCounter % TRASH_INTERVAL1 == 0)
                cmd.Blit(cameraTarget.nameID, trash1);
            if (frameCounter % TRASH_INTERVAL2 == 0)
                cmd.Blit(cameraTarget.nameID, trash2);

            // 3) copy into a temp RT
            cmd.GetTemporaryRT(_TempID, data.cameraData.cameraTargetDescriptor, FilterMode.Bilinear);
            cmd.Blit(cameraTarget.nameID, _TempID);

            // 4) set shader properties
            _mat.SetFloat(_IntensityId, _intensity);
            _mat.SetTexture(_NoiseId, noiseTex);
            // randomly pick one of the two trash renders
            _mat.SetTexture(_TrashId, (Random.value > 0.5f) ? trash1 : trash2);

            // 5) final blit with glitch
            cmd.Blit(_TempID, cameraTarget.nameID, _mat, 0);

            // cleanup
            cmd.ReleaseTemporaryRT(_TempID);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
