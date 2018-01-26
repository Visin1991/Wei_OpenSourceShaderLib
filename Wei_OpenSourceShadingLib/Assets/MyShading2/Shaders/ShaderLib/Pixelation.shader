// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ShaderLib/Pixelation" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_PixelNumberX("Pixel number along X", Range(10,500)) = 500
		_PixelNumberY("Pixel number along X", Range(10,500)) = 500
	}

	SubShader{
		Pass{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			float _PixelNumberX;
			float _PixelNumberY;


			struct v2f {
				half4 pos : POSITION;
				half2 uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); //get the vertexMVP
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half ratioX = 1 / _PixelNumberX;
				half ratioY = 1 / _PixelNumberY;
				//half2 uv = half2((int)(i.uv.x / ratioX) * ratioX, (int)(i.uv.y / ratioY) * ratioY);
				half2 uv = half2((int)(i.uv.x / ratioX) * ratioX, (int)(i.uv.y / ratioX) * ratioX);
				return tex2D(_MainTex, uv);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
/*
	Unity\Editor\Data\CGIncludes\UnityCG.inc
	// Transforms 2D UV by scale/bias property

	#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)

	It scales and offsets texture coordinates.  XY values controls the texture tiling and ZW the offset.

	原理： 通过cast float to Int 去减少颜色的差异。
	比如说： _PixelNumberX = 10; 那么 ratioX = 1/10;
			那么从 0 开始 到0.1 中间 {0.01，0.02，0.03........0.09} 这些数字 乘以 10 然后cast to Int 都等于0
			那么i.uv 从0开始到0.09的颜色就都会一样。
*/