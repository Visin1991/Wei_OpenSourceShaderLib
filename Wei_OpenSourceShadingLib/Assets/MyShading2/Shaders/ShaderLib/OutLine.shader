Shader "Custom/OutLine" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
		_Outline("_Outline", Range(0,1)) = 0
		_OutlineColor("Color", Color) = (1, 1, 1, 1)
	}
	SubShader{
		Pass{
			Tags{ "RenderType" = "Opaque" }
			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			float _Outline;
			float4 _OutlineColor;

			//This will get the same effect
			float4 vert(appdata_base v) : SV_POSITION
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);						//Get world position of the vertex
				float3 normal = mul((float3x3) UNITY_MATRIX_MV, v.normal);		//Get the world normal of the vertex
				normal.x *= UNITY_MATRIX_P[0][0];
				normal.y *= UNITY_MATRIX_P[1][1];

				//Only the edge--->of the world normal will not effect by projection Matrix

				//Add outLine color after we convert to MVP space
				o.pos.xy += normal.xy * _Outline;
				return o.pos;
			}

			/*
			float4 vert(appdata_base v) : SV_POSITION
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);						//Get world position of the vertex
				float3 normal = mul((float3x3) UNITY_MATRIX_MVP, v.normal);		//Get the world normal of the vertex
				o.pos.xy += normal.xy * _Outline;
				//o.pos.xyz += normal * _Outline;

				return o.pos;
			}*/

			half4 frag(v2f i) : COLOR
			{
				return _OutlineColor;
			}
			ENDCG
		}

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
		}

	ENDCG
	}
	FallBack "Diffuse"
}
