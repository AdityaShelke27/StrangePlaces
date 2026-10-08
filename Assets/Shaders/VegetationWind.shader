Shader "Custom/VegetationWind"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Wind)]
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.15
        _WindSpeed ("Wind Speed", Range(0, 10)) = 2
        _SwayAmount ("Sway Amount", Range(0, 1)) = 0.15

        [Header(Gusts)]
        _GustStrength ("Gust Strength", Range(0, 1)) = 0.25
        _GustSpeed ("Gust Speed", Range(0, 10)) = 1.5

        [Header(Shape)]
        _AnchorHeight ("Anchor Height", Range(0, 1)) = 0.1
        _BendPower ("Bend Power", Range(0.5, 5)) = 2
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            fixed4 _Color;

            float _WindStrength;
            float _WindSpeed;
            float _SwayAmount;

            float _GustStrength;
            float _GustSpeed;

            float _AnchorHeight;
            float _BendPower;

            // Set globally by StormController
            float _StormIntensity;
            float4 _WindDirection;

			float _GustIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 worldPosition =
                    mul(unity_ObjectToWorld, v.vertex).xyz;

                // Different plants don't move perfectly together.
                float phase =
                    worldPosition.x * 1.37 +
                    worldPosition.y * 0.73;

                // 0 at bottom, 1 at top.
                float heightMask = saturate(
                    (v.uv.y - _AnchorHeight) /
                    max(0.001, 1.0 - _AnchorHeight)
                );

                heightMask = pow(
                    heightMask,
                    _BendPower
                );

                // Gentle continuous sway.
                float sway =
                    sin(
                        _Time.y * _WindSpeed +
                        phase
                    );

                // Slower irregular gust.
                float gust =
                    sin(
                        _Time.y * _GustSpeed +
                        phase * 0.6
                    );

                gust *= sin(
                    _Time.y * _GustSpeed * 0.37 +
                    phase
                );

                float baseWind =
                    _WindStrength +
                    (_StormIntensity * _SwayAmount);

                float gustAmount =
                    gust *
                    _GustStrength *
                    _StormIntensity;

                float totalWind =
                    (sway * baseWind) +
                    gustAmount;

				totalWind *=
					lerp(
						1.0,
						1.8,
						_GustIntensity
					);

                float2 direction =
                    normalize(_WindDirection.xy + 0.0001);

                float2 displacement =
                    direction *
                    totalWind *
                    heightMask;

                // Convert world-like wind direction into
                // local vertex displacement.
                v.vertex.xy += displacement;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color =
                    tex2D(_MainTex, i.uv) *
                    i.color;

                return color;
            }

            ENDCG
        }
    }
}