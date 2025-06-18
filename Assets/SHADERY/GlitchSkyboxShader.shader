// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/GlitchHexSkybox"
{
    Properties
    {
        // Sky/Panorama
        _MainTex         ("Skybox Texture",       2D)    = "white" {}
        _AltTex          ("Alternate Texture",    2D)    = "white" {}

        [Header(Texture Swap Settings)]
        [Space]
        _SwapInterval    ("Swap Interval (sec)",  Float) = 10.0
        _MainDuration    ("Main Texture Duration (sec)", Float) = 6.0

        [Space]
        [Header(Main Glitch Settings)]
        [Space]
        _GlitchAmplitude ("Glitch Amplitude",   Float) = 0.2
        _GlitchNarrow    ("Glitch Narrowness",  Float) = 4.0
        _GlitchBlocky    ("Glitch Blockiness",  Float) = 2.0
        _GlitchMinimizer ("Glitch Minimizer",   Float) = 5.0
        _GlitchNumCell   ("Glitch Cells",      int) = 8
        _GlitchColor     ("Gltich Efect Color", Color) = (1,1,1,1)

        [Space]
        [Header(Glitch Alt Settings)]
        [Space]
        _AltGlitchAmplitude ("Alt Glitch Amplitude",   Float) = 0.1
        _AltGlitchNarrow    ("Alt Glitch Narrowness",  Float) = 2.0
        _AltGlitchBlocky    ("Alt Glitch Blockiness",  Float) = 1.0
        _AltGlitchMinimizer ("Alt Glitch Minimizer",   Float) = 3.0
        _AltGlitchNumCell   ("Alt Glitch Cells",       int)   = 5
        _AltGlitchColor     ("Alt Glitch Color",       Color) = (1,0.5,0.5,1)

        [Space]
        [Header(Triangle Grid Settings)]
        [Space]
        //[Toggle] _Invert("Invert color?", Float) = 0
        // Grid overlay
        _LineColor       ("Grid Line Color",    Color) = (0,0,0,1)
        _LineWidth       ("Grid Line Width",    Range(0.0001,1)) = 0.02
        _GridScale       ("Grid Scale",         Range(1,1000))   = 5.0
        _LineBlur        ("Grid Line Softness", Range(0,1))      = 0.002

        // Horizon/fade
        [Space]
        [Header(Fade Settings)]
        [Space]
        _GroundColor     ("Ground Color",       Color) = (0.2,0.2,0.2,1)
        _FadeRange       ("Horizon Fade",       Range(0.01,0.5)) = 0.1

        [Space]
        [Header(Scan lines Settings)]
        [Space]
        [Toggle] _EnableTime("Enable Time?", Float) = 1
        _TimeMultiplayer ("Time Multiplayer", Range(0.0,1.0)) = 0.4
        _ScanLineWidth ("Scan Line Width", Range(50.0,1000.0)) = 200

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

            //–– Properties
            sampler2D _MainTex;      float4 _MainTex_ST;
            sampler2D _AltTex;       float4 _AltTex_ST;
            float _SwapInterval;
            float _MainDuration;
            float4   _LineColor, _GlitchColor;
            float    _LineWidth, _GridScale, _LineBlur;
            float4   _GroundColor;   float _FadeRange;
            float    _GlitchAmplitude, _GlitchNarrow, _GlitchBlocky, _GlitchMinimizer;
            float _ScanLineWidth,_TimeMultiplayer, _EnableTime;
            int _GlitchNumCell;

            float _AltGlitchAmplitude, _AltGlitchNarrow, _AltGlitchBlocky, _AltGlitchMinimizer;
            int   _AltGlitchNumCell;
            float4 _AltGlitchColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };
            struct v2f
            {
                float4 pos     : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos     = UnityObjectToClipPos(v.vertex);
                o.viewDir = UnityObjectToWorldDir(v.vertex.xyz);
                return o;
            }

            // Simple 2D random & noise for glitch
            float rand(float2 p, float t) 
            { 
                return frac(sin(dot(p, float2(12.9898,78.233)) + t)*43758.5453); 
            }

            float noise(float2 uv, float blocky, float t)
            {
                float2 id = floor(uv);
                float2 f  = frac(uv);
                float n1 = rand(id, t), n2 = rand(id+1, t);
                float n3 = rand(id+float2(0,1), t), n4 = rand(id+1+float2(0,1), t);
                float2 u = smoothstep(0,1+blocky, f);
                return lerp(lerp(n1,n2,u.x), lerp(n3,n4,u.x), u.y);
            }

            float fbm(float2 uv, int oct, float blocky, float narrow, float t)
            {
                float v=0, amp=0.5;
                for(int i=0;i<oct;i++) { v+=amp*noise(uv,blocky,t); amp*=0.5; uv*=narrow; }
                return v;
            }

            // moves uv into a triangle lattice and returns a 3-component “barycentric” uv
            float3 SimplexGrid(float2 uv)
            {
                // skew factors
                const float F = 0.3660254038; // (sqrt(3)-1)/2
                float s = (uv.x + uv.y) * F;
                float2 skew = uv + s;

                float3 xyz;
                if (skew.x > skew.y)
                {
                    // upper triangle
                    xyz.xy = 1.0 - float2(skew.x, skew.y - skew.x);
                    xyz.z  = skew.y;
                }
                else
                {
                    // lower triangle
                    xyz.yz = 1.0 - float2(skew.x - skew.y, skew.y);
                    xyz.x  = skew.x;
                }
                return xyz;
            }

            // returns 1.0 exactly on the triangle edges (within a small blur band)
            float SampleTriangleEdgeBary(float3 cellUv, float lineW, float blur)
            {
                // we’ll take the max over the three edges:
                float m = 0.0;
                // for each barycentric component:
                [unroll]
                for (int i = 0; i < 3; ++i)
                {
                    float c = cellUv[i];          // coords from 0 to 1 across a triangle
                    m = max(m, smoothstep(lineW - blur, lineW + blur, c));
                    m = max(m, smoothstep(lineW - blur, lineW + blur, 1.0 - c));
                }
                return saturate(m);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t      = _Time.y;
                float cycle  = t - _SwapInterval * floor(t / _SwapInterval);
                bool useMain = (cycle < _MainDuration);

                // 1) Ray & horizon fade
                float3 dir  = normalize(i.viewDir);
                float  fade = saturate((dir.y/_FadeRange)*0.5 + 0.5);
                if(dir.y < -_FadeRange) return _GroundColor;

                // 2) Spherical UV for entire sky
                float u = frac(atan2(dir.x,dir.z)/(2*UNITY_PI) + 0.5);
                float v = asin(clamp(dir.y,-1,1))/UNITY_PI + 0.5;
                float2 sphUV = float2(u,v);

                //Sample Texture tilting and offset
                float2 mainUV = TRANSFORM_TEX(sphUV, _MainTex);
                float2 altUV  = TRANSFORM_TEX(sphUV, _AltTex);
                float2 texUV  = useMain ? mainUV : altUV;

                // 2) pick which glitch params to use
                float gAmp   = useMain ? _GlitchAmplitude    : _AltGlitchAmplitude;
                float gNarrow= useMain ? _GlitchNarrow       : _AltGlitchNarrow;
                float gBlocky= useMain ? _GlitchBlocky       : _AltGlitchBlocky;
                float gMin   = useMain ? _GlitchMinimizer    : _AltGlitchMinimizer;
                int   gCells = useMain ? _GlitchNumCell      : _AltGlitchNumCell;
                float4 gCol  = useMain ? _GlitchColor        : _AltGlitchColor;

                // 3) Glitch-distort that UV before sampling
                float time = _Time.y;
                float2 uv2 = float2((sphUV.x * 1.5) / 2000, exp(sphUV.y));
                if(_EnableTime == 1)
                    uv2.y += time * _TimeMultiplayer;

                float2 cell = floor(sphUV * gCells);
                float fbmVal = fbm(uv2, int(rand(cell,t)*6+1), gBlocky, gNarrow, t);
                float shift = gAmp * pow(fbmVal, gMin);
                shift = smoothstep(0.00001, 0.2, shift);


                // 4) Creating Scanlines
                // scanline darkening
                float scan = abs(cos(sphUV.y * _ScanLineWidth));
                scan = smoothstep(0,2,scan);

                // 5) Sample your panorama
                // final glitch UV offset
                float2 gUV = texUV + float2(shift, 0);
                float4 skyCol = useMain 
                    ? tex2D(_MainTex, gUV) 
                    : tex2D(_AltTex, gUV);

                float colR = (useMain 
                    ? tex2D(_MainTex, gUV + float2(shift,0)).r 
                    : tex2D(_AltTex, gUV + float2(shift,0)).r)
                  * (1.0 - shift)
                  + rand(cell, t) * shift * _GlitchColor.r;
                float colG = (useMain 
                    ? tex2D(_MainTex, gUV - float2(shift,0)).g 
                    : tex2D(_AltTex, gUV - float2(shift,0)).g)
                  * (1.0 - shift)
                  + rand(cell, t) * shift * _GlitchColor.g;
                float colB = (useMain 
                    ? tex2D(_MainTex, gUV - float2(shift,0)).b 
                    : tex2D(_AltTex, gUV - float2(shift,0)).b)
                  * (1.0 - shift)
                  + rand(cell, t) * shift * _GlitchColor.b;
                skyCol.rgb = float3(colR, colG, colB) - (0.01 * scan);

                // 6) Build triangle grid in a *second* UVspace
                float2 gridUV  = (sphUV + float2(shift,0)) * _GridScale;
                float3 bary    = SimplexGrid(gridUV);
                float3 cUv     = frac(bary);

                // 7) Edge mask
                float edge = SampleTriangleEdgeBary(cUv,_LineWidth,_LineBlur);

                // 8) Overlay grid lines
                float4 col = lerp(skyCol, _LineColor, edge);

                // 9) Finally horizon‐fade to ground color
                return lerp(_GroundColor, col, fade);
            }
            ENDCG
        }
    }
    FallBack Off
}
