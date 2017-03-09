Shader "ShaderLib/flowMapTest" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_FlowMap("Flow Map", 2D) = "grey" {}
		_Speed("Speed", Range(0.001, 1.0)) = 0.25
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

			struct v2f {
				float4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _FlowMap;
			fixed _Speed;

			fixed4 _MainTex_ST;

			v2f vert(appdata_base IN) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, IN.vertex);
				o.uv = TRANSFORM_TEX(IN.texcoord, _MainTex); //detail see Pixelation shader
				return o;
			}

			fixed4 frag(v2f v) : COLOR{
				fixed4 c;

				//half3 flowVal = (tex2D(_FlowMap, v.uv));
				half3 flowVal = (tex2D(_FlowMap, v.uv) * 2 - 1);
				
				/*
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

				*/

				float timeOffset1 = frac(_Time.y * _Speed); 

				c = tex2D(_MainTex, v.uv - flowVal.xy * timeOffset1); //Minus uv --> move pixel inverse direction
				return c;
		}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
