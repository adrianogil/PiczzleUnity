Shader "Puzzle/Quad"
{
	Properties {
		_GridSize("Grid Size", Vector) = (1,1,0,0)
		_BorderSize("Border Size", Float) = 0.05
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

			float when_eq(float x, float y) {
				return 1.0 - abs(sign(x - y));
			}

			float when_gt(float x, float y) {
			  return max(sign(x - y), 0.0);
			}

			float when_lt(float x, float y) {
			  return max(sign(y - x), 0.0);
			}

			float when_ge(float x, float y) {
			  return 1.0 - when_lt(x, y);
			}

			float when_le(float x, float y) {
			  return 1.0 - when_gt(x, y);
			}

			float bposition(float2 bpos, float2 uv, float2 gridSize)
			{
				return when_ge(uv.x, (bpos.x)*(1/gridSize.x)) * when_lt(uv.x, (bpos.x+1)*(1/gridSize)) *
					   when_ge(uv.y, (bpos.y)*(1/gridSize.y)) * when_lt(uv.y, (bpos.y+1)*(1/gridSize.y));
			}

			float4 frag(vert_output o) : COLOR
			{
				float2 uv = o.uv;
				uv.y = 1 - uv.y;

				if ((uv.x > 0.05 && frac(uv.x * _GridSize.x) < 0.05) || (uv.y > 0.05 && frac(uv.y * _GridSize.y) < 0.05))
				{
					return _BorderColor;
				}

				return tex2D(_MainTex, o.uv);

				// return tex2D(_MainTex, o.uv) * bposition(float2(2,2), uv, _GridSize);
			}

			ENDCG
		}
	}


}