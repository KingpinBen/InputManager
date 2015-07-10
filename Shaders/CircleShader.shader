//	In Editor vid: http://youtu.be/eNiRR2DMXxM

Shader "Custom/CircleShader" 
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Width("Line Width", Range(.01, .1)) = .01
		_NRadius("Radius", Range(0, .5)) = .01
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform half4 _Color;
			uniform half _Width;
			uniform half _NRadius;
			uniform sampler2D _MainTex;

			struct v2f
			{
				float4 position : POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base i)
			{
				v2f output;

				output.position = mul(UNITY_MATRIX_MVP, i.vertex);
				output.uv = i.texcoord;

				return output;
			}

			half4 frag(v2f i) : COLOR
			{
				//	Recenter the UV
				i.uv.x -= .5;
				i.uv.y -= .5;

				half distFromCenter = sqrt(
					i.uv.x * i.uv.x +
					i.uv.y * i.uv.y);

				half4 result = half4(0,0,0,0);

				//	distFromCenter < .5 causes the quad to display as a circle, removing the corners
				if (distFromCenter < _NRadius && distFromCenter > _NRadius - _Width)
					return _Color;

				if (distFromCenter < _NRadius - _Width)
				{
					result = _Color;
					result.a = .2f;
				}

				return result;
			}

			ENDCG
		}

	}
}
