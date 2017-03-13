Shader "ShaderLib/Vertex/Jelly" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TintColor("Tint Color",Color) = (0.7,0.1,0.1,1)
		_Exturde("Exturde Intensity",Range(0.01,1)) = 1.0
	}
		SubShader{
		Pass{
			Tags{ "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _TintColor;
			half _Exturde;
			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;
				//v.vertex.x += sign(v.vertex.x) * sin(_Time.w) * _Exturde;
				//v.vertex.y += sign(v.vertex.y) * cos(_Time.w) * _Exturde;
				//v.vertex.z += sign(v.vertex.z) * cos(_Time.w) * _Exturde;
				
				//Extrude the Vertex before we convert the local pos to MVP space
				v.vertex.xyz += sin(_Time.w) * v.normal * _Exturde;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			half4 frag(v2f i) : COLOR{
				half4 c = tex2D(_MainTex, i.uv) + _TintColor;
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
