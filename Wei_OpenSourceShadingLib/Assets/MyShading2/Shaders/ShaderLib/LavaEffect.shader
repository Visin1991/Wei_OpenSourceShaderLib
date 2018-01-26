// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ShaderLib/LavaEffect" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_FlowMap("Flow Map", 2D) = "grey" {}
		_Extrude("Extrude", Range(-1, 1)) = 0.2
		_Speed("Speed", Range(0.001, 1.0)) = 0.25
	}
	SubShader{
		Pass{
			Tags{ "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _FlowMap;
			fixed _Extrude;
			fixed _Speed;

			fixed4 _MainTex_ST;

			v2f vert(appdata_base IN) {
				v2f o;
				o.pos = UnityObjectToClipPos(IN.vertex);
				o.uv = TRANSFORM_TEX(IN.texcoord, _MainTex); //detail see Pixelation shader
				return o;
			}

			fixed4 frag(v2f v) : COLOR{
				fixed4 c;
				//get and uncompress the flow vector for this pixel
				
				half3 flowVal = (tex2D(_FlowMap, v.uv) * 2 - 1) * _Extrude; //see Note 1.1

				float timeOffset1 = frac(_Time.y * _Speed + 0.5); //Note1.2  //dif1 range =>[0.5,1.0f)
				float timeOffset2 = frac(_Time.y * _Speed); //dif2 range =>[0.0f,1.0f)

				half4 col1 = tex2D(_MainTex, v.uv - flowVal.xy * timeOffset1); //Minus uv --> move pixel inverse direction
				half4 col2 = tex2D(_MainTex, v.uv - flowVal.xy * timeOffset2);

				half lerpVal = abs((timeOffset1 - 0.5) * 2); //lerpVal range [0,1.0f)
				c = lerp(col1, col2, lerpVal); //use to reset the flow value

				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
/*
	The color of the flow map give the direction of the movement.
	For more information about flow map, read this good article : http://graphicsrunner.blogspot.fr/2010/08/water-using-flow-maps.html

Note 1.1
	float4 tex2D(sampler2D samp, float2 s) : http://http.developer.nvidia.com/Cg/tex2D.html
	so tex2D function retrun the value rgba. each component have the range [0,1]
	[0,1] * 2 - 1 => [-1,1]

	//red color represent the Horizontal flow dir Offset, green color represent the Vertical flow dir Offset
		animateUvOffset = flowVal.xy * timeOffset1
		originUV - animateUvOffset => animatedUV

		
		{
			Minus uv --> move pixel inverse direction

			when the current pixel is fully rad. flowVal will be float3(1,-1,-1)
			uv - float(1,-1) * timeOffset1 - will cause pixel move right down

			when the current pixel is fully rad. flowVal will be float3(-1,1,-1)
			uv - float(-1,1) * timeOffset1 - will cause pixel move left up

			So When
				R > 0.5f Pixel move right
				R < 0.5f Pixel move left

				G > 0.5f Pixel move UP
				G < 0.5f pixek move Down
		}

Note1.2
	float frac(float v)
	{
		return v - floor(v);
	}
*/