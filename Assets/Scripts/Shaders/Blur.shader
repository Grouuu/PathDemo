// Based on ChatGTP
Shader "Custom/SimpleBLur"
{
	Properties
	{
		_MainTex("Base Texture", 2D) = "white" { }
		_BlurSize("Blur Size", Range(0.0, 10.0)) = 1.0
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			# include "UnityCG.cginc"

			struct MeshData
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Interpolators
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			float _BlurSize;

			Interpolators vert (MeshData v)
			{
				Interpolators o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag (Interpolators i) : SV_Target
			{
				float2 offset = _BlurSize / _MainTex_TexelSize.xy;

				// Sample surrounding pixels for a basic box blur
				float4 col = tex2D(_MainTex, i.uv) * 0.16;
				col += tex2D(_MainTex, i.uv + float2(offset.x, 0)) * 0.16;
				col += tex2D(_MainTex, i.uv - float2(offset.x, 0)) * 0.16;
				col += tex2D(_MainTex, i.uv + float2(0, offset.y)) * 0.16;
				col += tex2D(_MainTex, i.uv - float2(0, offset.y)) * 0.16;

				return col;
			}

			ENDCG
        }
    }

    FallBack "Diffuse"
}