// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/GlitchHexSkyboxNew"
{
Properties
    {
        _Tint("Tint Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define PI 3.141592
            static const float2 helper = float2(1.0, 1.7320508);

            float4 _Tint;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = normalize(mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }

            float4 getHex(float2 uv)
            {
                float4 hexID = floor(float4(uv, uv - float2(0.5, 1.0)) / helper.xyxy) + 0.5;
                float4 hexUV = float4(uv - hexID.xy * helper, uv - (hexID.zw + 0.5) * helper);
                return dot(hexUV.xy, hexUV.xy) < dot(hexUV.zw, hexUV.zw)
                    ? float4(hexUV.xy, hexID.xy)
                    : float4(hexUV.zw, hexID.zw + 0.5);
            }

            float getHexDF(float2 uv)
            {
                uv = abs(uv);
                return max(dot(uv, helper * 0.5), uv.x);
            }

            float getHexSegment(float2 uv)
            {
                float segIndex = atan2(uv.x, uv.y) / (PI * 2.0) + 0.5;
                return floor(segIndex * 6.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir);
                float2 uv = float2(
                    atan2(dir.x, dir.z) / (2.0 * PI) + 0.5,
                    dir.y * 0.5 + 0.5
                );
                float2 localUV = uv * 2.0 - 1.0; // match original -1 to 1 range


                float time = _Time.y;

                float zoom = sin(time * 0.5) * 2.0 + 8.0;
                float4 hexGrid = getHex(localUV * zoom + float2(0.55, 0.2) * time);
                float hexDF = getHexDF(hexGrid.xy);
                float hexSegment = getHexSegment(hexGrid.xy);

                float feather = 0.01;
                float3 col = 0;

                float direction = dot(hexGrid.zw, float2(1.0, 2.0)) * 0.1;
                float modVal = fmod(hexSegment, 5.0);

                float progressX = sin(time + PI * modVal + direction) * 0.5 + 0.4;
                float progressY = sin(time + PI * modVal + direction + 0.15) * 0.5 + 0.4;
                float progressZ = sin(time + PI * modVal + direction + 0.3) * 0.5 + 0.4;

                col.r = 1.0 - smoothstep(progressX, progressX + feather, hexDF);
                col.g = 1.0 - smoothstep(progressY, progressY + feather, hexDF);
                col.b = 1.0 - smoothstep(progressZ, progressZ + feather, hexDF);

                return float4(col * _Tint.rgb, 1.0);
            }
            ENDCG
        }
    }
    FallBack Off
}
