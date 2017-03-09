// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ShaderLib/PlaneToWave" {
	Properties{
		_Color("Color", Color) = (0, 0, 0, 1)
		_Amplitude("Amplitude", Range(0,4)) = 1.0
		_Movement("Movement", Range(-100,100)) = 0
	}

	SubShader{
		Tags{ "RenderType" = "transparent" }

		Pass
		{
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4 _Color;
			float _Amplitude;
			float _Movement;

			struct vertexInput
			{
				float4 vertex : POSITION;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				float4 posWorld = mul(unity_ObjectToWorld, input.vertex);//convert local position to world position

				float displacement = cos(posWorld.x + _Movement * _Time);
				posWorld.y = posWorld.y + _Amplitude * displacement;

				output.pos = mul(UNITY_MATRIX_VP, posWorld); //Convert world positon to screen position

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				return _Color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}