Shader "FOB/ImageExtusion"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_HeatTex("Displacement Texture", 2D) = "white" {}
		_ScratchEffectTex("Get Hurt Effect",2D) = "white" {}
		_Magnitude("Magnitude", Range(0,0.1)) = 0.02
		_WaveSpeed("WaveSpeed", Range(0.1,10)) = 1
	}
SubShader
{
	// No culling or depth
	Cull Off ZWrite Off ZTest Always

	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		//Vertex Input
		struct appdata
		{
			float4 vertex : POSITION; //local position of the mesh
			float2 uv : TEXCOORD0;
		};

		//Fragment Input
		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		uniform bool convertUVY = false;

		//Vertex shader
		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex); //convert the local position to MVP position
			o.uv = v.uv;
			if (convertUVY) o.uv.y = 1 - o.uv.y;
			return o;
		}

		//outside-variable
		sampler2D _MainTex;
		sampler2D _HeatTex;
		sampler2D _ScratchEffectTex;

		float _Magnitude;
		half _WaveSpeed;
		
		
		uniform bool isHit;
		uniform bool useHeat;
		uniform float scratchTensity;
		static const float2 uv_Center = float2(0.5f, 0.5f);

		//Fragment Shader
		float4 frag(v2f i) : SV_Target
		{
			float4 col;
			//-------------------Heat Effect-------------------
			if (useHeat) {
				//get animated uv position based on _Time
				float2 distuv = float2(i.uv.x + _Time.x * _WaveSpeed, i.uv.y + _Time.x * _WaveSpeed);
				//get red color and green color based on the animated uv
				float2 disp = tex2D(_HeatTex, distuv).rg;

				//scale the range of disp form (0,1) to (0,2). and then move down the value to (-1,1) * _Magnitude;
				disp = ((disp * 2) - 1) * _Magnitude;

				//because disp is now based on a animatied rg color of a heatMap.
				//So disp has range of : (-1,1) * _Magnitude. 
				//See Notes 1.0
				col = tex2D(_MainTex, i.uv + disp);
			}else{
				col = tex2D(_MainTex, i.uv);
			}
			//-------------------------------------------------

			//-------------------Hurt Effect-------------------
			//get Hurt effect
			if (isHit) {
				float dstX = abs(uv_Center.x - i.uv.x);
				float dstY = abs(uv_Center.y - i.uv.y);
				float tint = dstX > dstY ? dstX : dstY;
				if (tint > 0.4f) {
					float average = (col.x + col.y + col.z) / 3;
					//col = float4(average / 2, average / 2, average / 2, 1.0f);
					col += float4((tint - 0.4f)*5, 0, 0, 1);
				}
			}

			if (scratchTensity > 0.0f)
			{
				float4 scratchCol = tex2D(_ScratchEffectTex, i.uv);
				if (scratchCol.a != 0)
				{
					col = lerp(col,scratchCol, scratchTensity);
				}
			}
			
			//-----------------------------------------------
			return col;
		}
		ENDCG
	}
}
}

/*
Notes 1.0:
	  With hightMap heatMap what ever maps crosspond to time. We can do regular effect
*/