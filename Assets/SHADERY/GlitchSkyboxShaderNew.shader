// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/GlitchHexSkyboxNew"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlitchAmplitude ("Glitch Amplitude", Float) = 0.2
        _GlitchNarrowness ("Glitch Narrowness", Float) = 4.0
        _GlitchBlockiness ("Blockiness", Float) = 2.0
        _GlitchMinimizer ("Minimizer", Float) = 5.0
        _Resolution ("Resolution", Vector) = (1920,1080,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Resolution;
            float _GlitchAmplitude;
            float _GlitchNarrowness;
            float _GlitchBlockiness;
            float _GlitchMinimizer;

            float rand(float2 p, float time)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233)) + time) * 43758.5453);
            }

            float noise(float2 uv, float blockiness, float time)
            {
                float2 lv = frac(uv);
                float2 id = floor(uv);

                float n1 = rand(id, time);
                float n2 = rand(id + float2(1,0), time);
                float n3 = rand(id + float2(0,1), time);
                float n4 = rand(id + float2(1,1), time);

                float2 u = smoothstep(0.0, 1.0 + blockiness, lv);
                return lerp(lerp(n1, n2, u.x), lerp(n3, n4, u.x), u.y);
            }

            float fbm(float2 uv, int count, float blockiness, float complexity, float time)
            {
                float val = 0.0;
                float amp = 0.5;

                for (int i = 0; i < count; ++i)
                {
                    val += amp * noise(uv, blockiness, time);
                    amp *= 0.5;
                    uv *= complexity;
                }

                return val;
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float time = _Time.y;
                float2 uv = i.uv;
                float2 res = _Resolution.xy;

                float2 a = float2(uv.x * (res.x / res.y), uv.y);
                float2 uv2 = float2(a.x / res.x, exp(a.y));
                uv2.y += time * 0.5; // add vertical motion for animation

                float2 id = floor(uv * 8.0);
                float shift = _GlitchAmplitude * pow(
                    fbm(uv2, (int)(rand(id, time) * 6.0 + 1.0), _GlitchBlockiness, _GlitchNarrowness, time),
                    _GlitchMinimizer
                );

                float scanline = abs(cos(uv.y * 400.0));
                scanline = smoothstep(0.0, 2.0, scanline);
                shift = smoothstep(0.00001, 0.2, shift);

                float colR = tex2D(_MainTex, uv + float2(shift, 0)).r * (1.0 - shift);
                float colG = tex2D(_MainTex, uv - float2(shift, 0)).g * (1.0 - shift) + rand(id, time) * shift;
                float colB = tex2D(_MainTex, uv - float2(shift, 0)).b * (1.0 - shift);

                float3 f = float3(colR, colG, colB) - (0.1 * scanline);

                return float4(f, 1.0);
            }
            ENDHLSL
        }
    }
}
