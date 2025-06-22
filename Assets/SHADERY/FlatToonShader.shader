Shader "Custom/FlatToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _RampTex ("Toon Ramp", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float2 uv         : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_RampTex);
            SAMPLER(sampler_RampTex);

            float4 _MainTex_ST;
            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Sample texture
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Get main directional light
                Light mainLight = GetMainLight();
                float3 normal = normalize(IN.normalWS);
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = dot(normal, lightDir);
                NdotL = saturate(NdotL);

                // Add ambient light
                float3 ambient = SampleSH(normal); // Environment lighting via spherical harmonics
                float totalLight = NdotL * mainLight.color.r + ambient.r;

                // Ramp it using the toon ramp texture
                float3 rampColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(totalLight, 0)).rgb;

                // Calculate additional per-pixel lights (point/spot)
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint i = 0; i < pixelLightCount; i++)
                {
                    Light light = GetAdditionalLight(i, IN.positionWS);
                    float3 lightDir = normalize(light.direction);
                    float NdotL_add = dot(normal, lightDir);
                    NdotL_add = saturate(NdotL_add);
                    totalLight += NdotL_add * light.color.r;
                }

                totalLight = saturate(totalLight);
                rampColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, float2(totalLight, 0)).rgb;

                float3 finalColor = texColor.rgb * rampColor * _Color.rgb;

                return float4(finalColor, texColor.a);
            }
            ENDHLSL
        }
    }
}
