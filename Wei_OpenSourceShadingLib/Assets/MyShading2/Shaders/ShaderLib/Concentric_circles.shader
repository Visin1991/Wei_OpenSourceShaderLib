﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ShaderLib/Concentric_circles" {
	Properties{
		_OrigineX("PosX Origine", Range(0,1)) = 0.5
		_OrigineY("PosY Origine", Range(0,1)) = 0.5
		_Speed("Speed", Range(-100,100)) = 60.0
		_Frequency("Circle quantity", Range(10,1000)) = 60.0
	}

	SubShader{
	
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma target 3.0

			float _OrigineX;
			float _OrigineY;
			float _Speed;
			float _Frequency;

			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput {
				float4 position : SV_POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			fragmentInput vert(vertexInput i) {
				fragmentInput o;
				o.position = UnityObjectToClipPos(i.vertex);
				o.texcoord0 = i.texcoord0;
				return o;
			}

			fixed4 frag(fragmentInput i) : SV_Target{
				fixed4 color;
				float distanceToCenterSqr;
				float time = _Time.x * _Speed;

				float xdist = _OrigineX - i.texcoord0.x;
				float ydist = _OrigineY - i.texcoord0.y;

				distanceToCenterSqr = (xdist * xdist + ydist * ydist);

				color = sin(distanceToCenterSqr * _Frequency + time);
				return color;
			}
			ENDCG
		}//end Pass
	}//end SubShader
}