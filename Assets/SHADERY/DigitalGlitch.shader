Shader "Custom/DigitalGlitch"
{
    Properties
    {
        _MainTex ("Base Map", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _TrashTex ("Trash", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,1)) = 0.5
        _TimeX     ("Time", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "URPGlitchPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            TEXTURE2D(_TrashTex);
            SAMPLER(sampler_TrashTex);

            float _Intensity;
            float _TimeX;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                // Sample the noise texture
                float4 glitch = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv);

                // Exactly Kino’s threshold
                float thresh = 1.001 - _Intensity * 1.001;

                // Three glitch weights
                float w_d = step(thresh, pow(glitch.z, 2.5)); // uv displacement
                float w_f = step(thresh, pow(glitch.w, 2.5)); // frame mix
                float w_c = step(thresh, pow(glitch.z, 3.5)); // color shuffle

                // Displace UVs when w_d == 1
                float2 uv = frac(IN.uv + glitch.xy * w_d);

                // Read from your main and trash textures
                float4 baseCol  = SAMPLE_TEXTURE2D(_MainTex,  sampler_MainTex, uv);
                float4 trashCol = SAMPLE_TEXTURE2D(_TrashTex, sampler_TrashTex, uv);

                // Mix in the trash frame
                float3 color = lerp(baseCol.rgb, trashCol.rgb, w_f);

                // Negative-color shuffle
                float3 neg = saturate(color.grb + (1.0 - dot(color, 1.0)) * 0.5);
                color = lerp(color, neg, w_c);

                return float4(color, baseCol.a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
