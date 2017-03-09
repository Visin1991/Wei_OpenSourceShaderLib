Shader "ShaderLib/VaryingColor" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_RampTex("Ramp texture", 2D) = "white" {}
		_Speed("Speed", Range(1, 10)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _RampTex;
			float _Speed;

			fixed4 frag(v2f_img i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return tex2D(_RampTex, fixed2(col.r + _Time.x * _Speed, 0.5));
			}
			ENDCG
		}
	}
}
