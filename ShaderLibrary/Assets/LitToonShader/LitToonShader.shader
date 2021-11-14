//Notes:
//If we want multiple shading bands, then we can use a white-black patch texture

Shader "Piyush/LitToonShader"
{
    Properties
    {
        [HDR]_Color("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_AmbientColor("Ambient Color", Color) = (0.5, 0.5, 0.5, 1)
        [HDR]_SpecularColor("Specular Color", Color) = (0.8, 0.8, 0.8, 1)
        _Glossiness("Glossiness", float) = 32
        [HDR]_RimColor("Rim Color", Color) = (1, 1, 1, 1)
        _RimAmount("Rim Amount", Range(0, 1)) = 0.5
        _RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags {
            "RenderType"="Opaque"
            "LightMode"="ForwardBase"
            "Passflag"="OnlyDirectional"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
                SHADOW_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _AmbientColor;
            float4 _SpecularColor;
            float _Glossiness;
            float4 _RimColor;
            float _RimAmount;
            float _RimThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float shadow = SHADOW_ATTENUATION(i);

                float3 normal = normalize(i.worldNormal);
                float nDotL = dot(normal, _WorldSpaceLightPos0.xyz);
                float lightIntensity = smoothstep(0, 0.1, nDotL * shadow);
                float4 lightColor = lightIntensity * _LightColor0;

                float3 viewDir = normalize(i.viewDir);
                float3 halfVec = normalize(_WorldSpaceLightPos0 + viewDir);
                float nDotH = dot(normal, halfVec);
                float specularIntensity = pow(nDotH * lightIntensity, _Glossiness * _Glossiness);
                float4 specular = smoothstep(0.005, 0.01, specularIntensity) * _SpecularColor;

                float rimDot = 1 - dot(normal, viewDir);
                float rimIntensity = rimDot * pow(nDotL, _RimThreshold);
                rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
                float4 rimColor = _RimColor * rimIntensity;


                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return (col * _Color) * (_AmbientColor + lightColor + specular + rimColor);
            }
            ENDCG
        }

        //Using the existing pass for casting the shadow
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
