Shader "Unlit/Auro"
{
    Properties
    {
        _SkyColor ("SkyColor", Color) = (0.4, 0.4, 0.4, 1)
        _SkyCurvature ("SkyCurvature", Range(0.1, 10)) = 0.4
        _AurorasTiling ("SurorasTiling", Range(0.1, 10)) = 0.4
        [IntRange] _RayMarchStep ("RayMarchStep", Range(1, 126)) = 64
        _RayMarchDistance ("RayMarchDistance", Range(0.01, 2.5)) = 1
        _AurorasTex ("AurorasTex", 2D) = "white" {}
        _AurorasColor ("AurorasColor", Color) = (0.4, 0.4, 0.4, 1)
        _AurorasAttenuation ("AurorasAttenuation", Range(0, 0.99)) = 0.4
        _AurorasIntensity ("AurorasIntensity", Range(0.1, 20)) = 3
        _AurorasNoiseTex ("AurorasNoiseTec", 2D) = "white" {}
        _AurorasSpeed ("AurorasSpeed", Range(0.01, 1)) = 0.1
        _StarNoiseTex ("StarNoiseTex", 2D) = "white" {}
        _StarShiningSpeed ("StarShiningSpeed", Range(0, 1)) = 0.1
        _StarCount ("StarCount", Range(0, 1)) = 0.3
        _SkyLineSize ("SkyLineSize", Range(0, 1)) = 0.06
        _SkyLineBasePow ("SkyLineBasePow", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            float3 _SkyColor;
            float _SkyCurvature;
            float _AurorasTiling;
            float _RayMarchStep;
            float _RayMarchDistance;
            sampler2D _AurorasTex;
            float4 _AurorasTex_ST;
            float3 _AurorasColor;
            float _AurorasAttenuation;
            float _AurorasIntensity;
            sampler2D _AurorasNoiseTex;
            float4 _AurorasNoiseTex_ST;
            float _AurorasSpeed;
            sampler2D _StarNoiseTex;
            float4 _StarNoiseTex_ST;
            float _StarShiningSpeed;
            float _StarCount;
            float _SkyLineSize;
            float _SkyLineBasePow;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos: TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(v.vertex, unity_ObjectToWorld);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const float starTime = _Time.y * _StarShiningSpeed;
                // 计算叠加区间的两层星星的uv
                const float2 beginMove = floor(starTime) * 0.3;
                const float2 endMove = ceil(starTime) * 0.3;
                const float2 beginUV = i.uv + beginMove;
                const float2 endUV = i.uv + endMove;
                // 采样两层星星的值
                float beginNoise = tex2D(_StarNoiseTex, TRANSFORM_TEX(beginUV, _StarNoiseTex)).r;
                float endNoise = tex2D(_StarNoiseTex, TRANSFORM_TEX(endUV, _StarNoiseTex)).r;
                // 减少星星
                beginNoise = saturate(beginNoise - (1 - _StarCount)) / _StarCount;
                endNoise = saturate(endNoise - (1 - _StarCount)) / _StarCount;
                const float fracStarTime = frac(starTime);
                // 混合两层星星值
                float starColor = saturate(beginNoise - fracStarTime) + saturate(endNoise - (1 - fracStarTime));
                // return tex2D(_StarNoiseTex, TRANSFORM_TEX(i.uv, _StarNoiseTex)).r;
                float3 color = 0;
                // 计算RayMarch信息
                // 每个像素发射射线
                float3 rayOrigin = 0;
                float3 totalDir = i.worldPos - rayOrigin;
                float3 rayDir = normalize(totalDir);
                // 拓展球面计算march起始点
                // 天空曲率
                float skyCurvatureFactor = rcp(rayDir.y + _SkyCurvature);
                // 模拟地球大气，无数条射线向外发射，就会形成一个球面。
                float3 basicRayPlane = rayDir * skyCurvatureFactor * _AurorasTiling;
                // march起始点
                float3 rayMarchBegin = rayOrigin + basicRayPlane;
                // 一步的大小
                float stepSize = rcp(_RayMarchStep);
                float3 avgColor = 0;
                for (float i = 0; i < _RayMarchStep; i += 1)
                {
                    float curStep = stepSize * i;
                    // 初始采样比重较大，用二次函数强化
                    curStep = curStep * curStep;
                    // 当前步进距离
                    float curDistance = curStep * _RayMarchDistance;
                    // 步进后的位置
                    float3 curPos = rayMarchBegin + rayDir * curDistance * skyCurvatureFactor;
                    float2 uv = float2(-curPos.x, curPos.z);
                    // 计算扰动uv
                    float2 warp_vec = tex2D(_AurorasNoiseTex, TRANSFORM_TEX((uv * 2 + _Time.y * _AurorasSpeed), _AurorasNoiseTex));
                    // 采样当前的噪声强度
                    float curAuroras = tex2D(_AurorasTex, TRANSFORM_TEX((uv + warp_vec * 0.1), _AurorasTex)).r;
                    // 强度衰减
                    curAuroras = curAuroras * saturate(1 - pow(curDistance, 1 - _AurorasAttenuation));
                    // 极光色彩累积计算
                    float3 curColor = sin((_AurorasColor * 2 - 1) + i * 0.043) * 0.5 + 0.5;
                    // 取两步色彩平均值，使眼色接近本色
                    avgColor = (avgColor + curColor) / 2;
                    // 混合颜色
                    color += avgColor * curAuroras * stepSize;
                }
                // 深度
                color *= _AurorasIntensity;
                // 混合天际线
                color *= saturate(rayDir.y / _SkyLineSize + _SkyLineBasePow);
                color += _SkyColor;
                // 星星
                color = color + starColor * 0.9;
                return float4(color, 1);
            }
            ENDCG
        }
    }
}
