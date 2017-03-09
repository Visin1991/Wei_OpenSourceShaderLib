Shader "ShaderLib/BumpIntensity" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Bump("Normal map", 2D) = "bump" {}
		_Intensity("Intensity", Range(-20,20)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Bump;
		float _Intensity;

		struct Input {
			float2 uv_MainTex;
			float2 uv_Bump;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed3 n = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
			n.x *= _Intensity; //when we only scale the x y value.  we acturally scale the different.
			n.y *= _Intensity; //as long as we dont scale xyz together.
			o.Normal = normalize(n);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
