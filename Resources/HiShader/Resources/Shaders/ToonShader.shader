Shader "Unlit/ToonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth ("OutLine Width", Range(0.01, 1)) = 0.01
        _OutlineColor ("Outline Color", Color) = (0.5, 0.5, 0.5, 1)

        _RampStart ("RampStart", Range(0.1, 1)) = 0.3
        _RampSize ("RampSize", Range(0, 1)) = 0.1

        [IntRange] _RampStep("RampStep", Range(1, 10)) = 1
        _RampSmooth ("RampSmooth", Range(0.01, 1)) = 0.1

        _DarkColor ("DarkColor", Color) = (0.4, 0.4, 0.4, 1)
        _LightColor ("LightColor", Color) = (0.8, 0.8, 0.8, 1)

        _SpecPow ("SpecPow", Range(0.01, 1)) = 0.1
        _SpecularColor ("SpecularColor", Color) = (1.0, 1.0, 1.0, 1)
        _SpecIntensity ("SpecIntensity", Range(0, 1)) = 0
        _SpecSmooth ("SpecSmooth", Range(0, 0.5)) = 0.1

        _RimThreshold ("RimThreshold", Range(0, 1)) = 0.05
        _RimSmooth ("RimSmooth", Range(0, 0.5)) = 0.1
        _RimColor ("RimColor", Color) = (1.0, 1.0, 1.0, 1)
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;

                float3 worldNormal: TEXCOORD1;
                float3 worldPos: TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RampStart;
            float _RampSize;
            float _RampStep;
            float _RampSmooth;
            float3 _DarkColor, _LightColor;
            float _SpecPow;
            float3 _SpecularColor;
            float _SpecIntensity, _SpecSmooth;
            float _RimColor;
            float _RimThreshold, _RimSmooth;

            float linearstep(float min, float max, float t)
            {
                return saturate((t - min) / (max - min));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 normal = normalize(i.worldNormal);
                float3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
                float NoL = dot(i.worldNormal, worldLightDir);

                float halfLambert = NoL * 0.5 + 0.5;

                //高光
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz) - i.worldPos.xyz;
                float3 halfDir = normalize(viewDir + worldLightDir);
                float NoH = dot(normal, halfDir);
                float blinnPhone = pow(max(0, NoH), _SpecPow * 128.0);
                float3 specularColor = smoothstep(0.7 - _SpecSmooth / 2, 0.7 + _SpecSmooth / 2, blinnPhone) + _SpecularColor * _SpecIntensity;

                //边缘光
                float NoV = dot(i.worldNormal, viewDir);
                float rim = 1 - max(0, NoV) * NoL;
                float3 rimColor = smoothstep(_RimThreshold - _RimSmooth / 2, _RimThreshold + _RimSmooth / 2, rim) * _RimColor;

                //色阶
                float ramp = linearstep(_RampStart, _RampStart + _RampSize, halfLambert);
                float step = ramp * _RampStep;
                float gridStep = floor(step);
                float smoothStep = smoothstep(gridStep, gridStep + _RampSmooth, step) + gridStep;
                ramp = smoothStep / _RampStep;
                float3 rampColor = lerp(_DarkColor, _LightColor, ramp);
                rampColor *= col;

                float3 finalColor = saturate(rampColor + specularColor + rimColor);
                return float4(finalColor, 1);
            }
            ENDCG
        }
        
        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                float4 newVertex = float4(v.vertex.xyz + normalize(v.normal) * _OutlineWidth * 0.01, 1);
                o.vertex = UnityObjectToClipPos(newVertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

    }
}
