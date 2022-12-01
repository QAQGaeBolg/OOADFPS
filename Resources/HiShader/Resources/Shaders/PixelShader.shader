Shader "Unlit/PixelShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float3 worldNormal: TEXCOORD1;
                float3 worldPos: TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                return o;
            }

            //v2f vert(appdata v)
            //{
            //    v2f o;
            //    o.vertex = UnityObjectToClipPos(v.vertex);
            //    // 模型空间转到世界空间
            //    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            //    // 法线向量归一化
            //    o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
            //    return o;
            //}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 worldLightDir = UnityWorldSpaceLightDir(i.worldPos);
                float NoL = dot(i.worldNormal, worldLightDir);
                float halfLambert = NoL * 0.5 + 0.5;

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col * halfLambert;
            }
            ENDCG
        }
    }
}
