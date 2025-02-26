Shader "Custom/Dust"
{
    Properties
    {
        // input data
        _WorldVelocity("WorldVelocity", Vector) = (0, 0, 0, 0)
        _Color("_Color", Color) = (0, 0, 0, 1)
        _NoiseSize("_NoiseSize", float) = 0.2
        _VerticalScale("_VerticalScale", float) = 1
        _MinAlpha("_MinAlpha", float) = 0
        _MaxAlpha("_MaxAlpha", float) = 1

        // runtime
        _Intensity("_Intensity", float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        ZWrite Off                              // not written in depth buffer
        Blend Zero OneMinusSrcAlpha             // allow transparency

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Scripts/Shaders/ShaderLib.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;       // vertex position
                float2 uv : TEXCOORD0;          // texture position
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;    // clip space position
                float2 uv : TEXCOORD0;          // mesh position [0, 1]
                float4 world : TEXCOORD1;       // world position
            };

            float4 _WorldVelocity;
            float4 _Color;
            float _NoiseSize;
            float _VerticalScale;
            float _MinAlpha;
            float _MaxAlpha;
            float _Intensity;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
                o.uv = v.uv;
                o.world = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 getCloudColor (float3 color, float2 position, float scale, float2 velocity) {

                position *= scale;
                position.x -= _Time * velocity.x;
                position.y -= _Time * velocity.y;
                
                return float4(color.xyz, perlinNoise(position));
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                // CLOUDS ALPHA

                float2 noisePosition = i.world;
                // size of clouds
                noisePosition *= _NoiseSize;
                // scale of clouds
                noisePosition.y *= _VerticalScale;
                // clouds movement
                noisePosition.x -= _Time * _WorldVelocity.x;
                noisePosition.y -= _Time * _WorldVelocity.y;

                float alpha = perlinNoise(noisePosition);

                // sharpness
                alpha = smoothstep(_MinAlpha, _MaxAlpha, alpha);
                // intensity
                alpha *= _Intensity;

                // clamp 01
                alpha = saturate(alpha);

                //return float4(alpha, 0, 0, 1);
                return float4(_Color.xyz, alpha);
            }

            ENDCG
        }
    }
}
