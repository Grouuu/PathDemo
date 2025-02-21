Shader "Custom/Dust"
{
    Properties
    {
        // input data
        //_Position("Position", Vector) = (0, 0, 0, 0)
        _WorldVelocity("WorldVelocity", Vector) = (0, 0, 0, 0)
        _Color("_Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        ZWrite Off                              // not written in depth buffer
        Blend SrcAlpha OneMinusSrcAlpha         // allow transparency

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

            //float4 _Position;
            float4 _WorldVelocity;
            float4 _Color;

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
                noisePosition *= 0.2;
                // clouds movement
                noisePosition.x -= _Time * _WorldVelocity.x;
                noisePosition.y -= _Time * _WorldVelocity.y;

                float alpha = perlinNoise(noisePosition);

                //float2 worldPosition = i.world;
                //float4 color1 = getCloudColor(float3(1, 0, 0), worldPosition, 0.2, float2(10, 0));
                //float4 color2 = getCloudColor(float3(0, 1, 0), worldPosition, 0.3, float2(5, 0));
                //float4 color3 = getCloudColor(float3(0, 0, 1), worldPosition, 0.5, float2(2, 0));

                ////float4 color = color1 * color1.a + color2 * (1.0 - color1.a) * 0.5;
                //float4 color = blendColors(color1, color2, 0.5);
                //color = blendColors(color, color3, 0.5);

                //float noiseValue = perlinNoise(worldPosition);
                //float grainThreshold = noiseValue * 0.5;
                //float distanceToGrain = length(worldPosition - floor(worldPosition));
                //float grain = step(0.05, distanceToGrain);

                //color = float4(0, 0, 0, 0);

                ////if (noiseValue > 0.5)
                //if (grain < 0.5 && noiseValue > grainThreshold)
                //{
                //    color = float4(1, 0, 0, 1);
                //}

                //float noiseValue = perlinNoise(worldPosition);
                //float grainSize = 0.05;
                //float2 gridCoords = worldPosition / grainSize;
                ////float4 grainPosition = float4(gridCoords.xy, 0, 0);
                ////float grain = step(0.5, noiseValue);
                //float distanceToGrain = length(gridCoords - floor(gridCoords));
                ////color = float4(1, 0, 0, grain);

                //color = float4(0, 0, 0, 0);
                //if (distanceToGrain < 0.05 && noiseValue > 0.5)
                //{
                //    color = float4(1, 0, 0, noiseValue);
                //}

                //float4 color = color3;
                //color = lerp(color, color2, 0.1);
                //color = lerp(color, color1, 0.8);

                //return float4(color);
                return float4(_Color.xyz, alpha);
                //return float4(0, i.uv.y, 0, 1);
                //return float4(i.uv.x, 0, 0, 1);
                //return float4(i.uv, 0, i.uv.x);
            }

            ENDCG
        }
    }
}
