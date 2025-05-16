// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/GlitchHexSkybox"
{
Properties
    {
        _MainTex   ("Texture",    2D)        = "white" {}
        _LineColor ("Line Color", Color)     = (0,0,0,1)
        _LineWidth ("Line Width", Range(0.0001, 1)) = 0.02
        _Scale     ("Grid Scale", Range(1, 1000))    = 5.0
        _GroundColor  ("Ground Color",     Color) = (0.2,0.2,0.2,1)
        _FadeRange    ("Horizon Fade Range", Range(0.01,0.5)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4   _MainTex_ST;
            float4   _LineColor;
            float    _LineWidth;
            float    _Scale;
            float4   _GroundColor;
            float    _FadeRange;

            struct appdata
            {
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 pos    : SV_POSITION;
                float3 viewDir: TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // full‐screen triangle/quad (in normalized clip space)
                o.pos = UnityObjectToClipPos(v.vertex);
                // world‐space view direction
                o.viewDir = UnityObjectToWorldDir(v.vertex.xyz);
                return o;
            }

            // Draws a smooth equilateral triangle grid
            float SampleTriangleEdge(float2 uv, float scale, float lineW)
            {
                uv *= scale;

                // rotate Y into the triangle lattice
                float invSqrt3   = 0.57735026919;   // 1/√3
                float twoOverSqrt3 = 1.15470053838; // 2/√3
                float2 r;
                r.x = uv.x - uv.y * invSqrt3;
                r.y = uv.y * twoOverSqrt3;

                float2 f = frac(r);
                // distance to the nearest triangle edge
                float d = min(min(f.x, f.y), 1.0 - (f.x + f.y));
                return smoothstep(lineW, lineW + 0.005, d);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.viewDir);

                // 1) compute horizon fade
                float fade = saturate((dir.y / _FadeRange) * 0.5 + 0.5);

                // 2) if well below horizon, solid ground
                if (dir.y < -_FadeRange)
                    return _GroundColor;

                // 3) spherical UVs
                float u = atan2(dir.x, dir.z) / UNITY_PI * 0.5 + 0.5;
                float v = asin(clamp(dir.y, -1, 1)) / UNITY_PI + 0.5;
                float2 uv = float2(u, v);

                // 4) triangle edge mask
                float edgeMask = SampleTriangleEdge(uv, _Scale, _LineWidth);
                //    edgeMask=1 inside triangles, 0 on lines

                // 5) build base sky color: 
                //    triangles → _LineColor, lines → texture
                float4 lineTex = tex2D(_MainTex, TRANSFORM_TEX(uv, _MainTex));
                float4 skyBase = lerp(lineTex, _LineColor, edgeMask);

                // 6) blend skyBase with ground across the fade band
                return lerp(_GroundColor, skyBase, fade);
            }
            ENDCG
        }
    }
    FallBack Off
}
