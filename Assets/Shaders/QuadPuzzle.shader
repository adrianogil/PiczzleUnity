Shader "Puzzle/Quad"
{
	Properties {
		_SelectedPosition("Selected Position", Vector) = (-1,-1,0,0)
		_EmptyPosition("Empty Position", Vector) = (-1,-1,0,0)
		_GridSize("Grid Size", Vector) = (1,1,0,0)
		_SelectionMove("Move", Range(0,1)) = 0
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

			float2 _SelectedPosition;
			float2 _EmptyPosition;

			float4 _BorderColor;
			float2 _GridSize;

			float _SelectionMove;

			uniform int _GridXLength = 9;
            uniform float _GridX[9];

            uniform int _GridYLength = 9;
            uniform float _GridY[9];

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

				// return tex2D(_MainTex, o.uv);

				float4 color = float4(0,0,0,0);

				float2 relative_uv = 0;

				for (int y = 0; y < 3; y++)
				{
					for (int x = 0; x < 3; x++)
					{
						if (_GridX[y*3 + x] >= 0 && _GridY[y*3 + x] >= 0)
						{
							if (_SelectedPosition.x != x || _SelectedPosition.y != y)
							{
								relative_uv = uv - float2(x,y) * (1/_GridSize);
								relative_uv += float2(_GridX[y*3 + x],_GridY[y*3 + x]) * (1/_GridSize);

								relative_uv.y = 1 - relative_uv.y;

								color += tex2D(_MainTex, relative_uv) * bposition(float2(x,y), uv, _GridSize);
							}
						}
					}
				}

				float2 distance = _EmptyPosition - _SelectedPosition;
				float2 currentUVPosition = _SelectedPosition + _SelectionMove * distance;
				currentUVPosition *= (1/_GridSize);

				if (uv.x >= currentUVPosition.x && uv.x <= currentUVPosition.x + (1/_GridSize.x) &&
					uv.y >= currentUVPosition.y && uv.y <= currentUVPosition.y + (1/_GridSize.y))
				{
					relative_uv = float2(_GridX[_SelectedPosition.y*_GridSize.x + _SelectedPosition.x],
										 _GridY[_SelectedPosition.y*_GridSize.x + _SelectedPosition.x]);
					relative_uv *= (1/_GridSize);

					relative_uv += (uv - currentUVPosition);

					relative_uv.y = 1 - relative_uv.y;

					color = tex2D(_MainTex, relative_uv);
				}

				return color;
			}
			ENDCG
		}
	}


}