Shader "Puzzle/Quad"
{
	Properties {
		_GridSize("Grid Size", Vector) = (1,1,0,0)
		_BorderColor("Border Color", Color) = (0,0,0,1)
		_MainTex ("Image", 2D) = "white"
	}

	Subshader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler _MainTex;

			float4 _BorderColor;
			float2 _GridSize;

			struct vert_input
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vert_output
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			vert_output vert(vert_input i)
			{
				vert_output o;

				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv = i.uv;

				return o;
			}

			float4 frag(vert_output o) : COLOR
			{
				// if (fmod(o.uv*100, _GridSize) < 10)
				// {
				// 	return _BorderColor;
				// }

				return tex2D(_MainTex, o.uv);
			}

			ENDCG
		}	
	}

	
}