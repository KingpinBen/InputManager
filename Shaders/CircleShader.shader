//	In Editor vid: http://youtu.be/eNiRR2DMXxM

Shader "Custom/CircleShader" 
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Width("Line Width", Range(.01, .1)) = .01
		_Radius("Radius", Range(0, .5)) = .01
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
			uniform half _Radius;

			struct v2f
			{
				float4 position : POSITION;
				half2 uv : TEXCOORD0;
				
				//	x = sqr-Radius
				//	y = sqr-RadiusMinusWidth
				half2 sqrs : TEXCOORD1;
			};

			v2f vert(appdata_base i)
			{
				v2f o;

				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = i.texcoord - half2(.5, .5);

				o.sqrs.x = pow(_Radius, 2);
				o.sqrs.y = pow(_Radius - _Width, 2);

				return output;
			}

			half4 frag(v2f i) : COLOR
			{
				half distance = (i.uv.x * i.uv.x) + (i.uv.y * i.uv.y);

#if SHADER_API_D3D9
				//	Clipping on D3D9 was compiling to invoke a texkill. 
				if (i.sqrs.x < distance)
					return _Color * (distance < i.sqrs.y ? .5 : 1);
				return half4(0, 0, 0, 0);
#else
				clip(distance > i.sqrs.x ? -1 : 1);

				half4 result = _Color;
				if (distance < i.sqrs.y)
					result.a = .5;

				return result;
#endif
			}

			ENDCG
		}

	}
}
