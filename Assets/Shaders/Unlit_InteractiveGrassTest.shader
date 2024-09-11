Shader "CrashHead/Unlit_InteractiveGrassTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor("Tint color", Color) = (1,1,1,1)
        _HitPoint("Hit Point", Vector) = (0, 0, 0, 0)
        _HitPower("Hit Power", float) = 1.5
        _HitRadius("Hit Radius", float) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent+0"}
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _TintColor;
            float4 _MainTex_ST;
            float4 _HitPoint;
            float _HitPower;
            float _HitRadius;

            v2f vert (appdata v)
            {
                // grass offset
                float radius = _HitRadius;
                float4 hitPointLifted = _HitPoint;
                hitPointLifted.y = v.vertex.y;
                float dist = clamp(radius - distance(hitPointLifted, v.vertex), 0, radius) / radius;
                float height = max(v.vertex.y - 0.2f, 0);
                float4 dir = normalize(v.vertex - hitPointLifted);
                dir.y -= dist;
                float4 offset = _HitPower * dist * height * dir;

                // grass animation
                float gap = 0.5f;
                float animOffset = 0.1f * height * sin(2 * height  + 2 * sin(gap * v.vertex.x) * sin(gap * v.vertex.z) - 50 * _Time); 
                offset.x += animOffset;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex + offset);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
