Shader "ShaderLib/FireBall" {
	Properties {
		_RampTex("Ramp texture", 2D) = "white" {}
		_NoiseTex("Noise texture",2D) = "grey" {}
		_RampVal("Ramp offset", Range(-0.5, 0.5)) = 0
		_Amplitude("Amplitude factor", Range(0, 0.03)) = 0.01
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _NoiseTex;
		sampler2D _RampTex;
		fixed _RampVal;
		fixed _Amplitude;

		struct Input {
			float2 uv_NoiseTex;
		};

		void vert(inout appdata_full v) {
			half noiseVal = tex2Dlod(_NoiseTex, float4(v.texcoord.xy, 0, 0)).r;
			v.vertex.xyz += v.normal * sin(_Time.w + noiseVal * 100)* _Amplitude;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			half noiseVal = tex2D(_NoiseTex, IN.uv_NoiseTex).r + (sin(_Time.y)) / 15;
			half4 color = tex2D(_RampTex, float2(saturate(_RampVal + noiseVal), 0.5)); //because all color on the y axis are the same, so we use the constant value 0.5f
			o.Albedo = color.rgb;
			o.Emission = color.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

/*
	vert function
		Vertex on the mesh extrude forward to the normal direction. 
		use the noise to make the extrude variation. use the time to make a animation
	
	surf function
		use time to animate the noise value

		float saturate(float x);
			Returns x saturated to the range [0,1] as follows:
				1) Returns 0 if x is less than 0; else
				2) Returns 1 if x is greater than 1; else
				3) Returns x otherwise.

*/
