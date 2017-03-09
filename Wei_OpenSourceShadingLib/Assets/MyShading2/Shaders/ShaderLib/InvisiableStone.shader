Shader "ShaderLib/InvisiableStone" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }

		Stencil{
			Ref 1
			Comp equal
			Pass Keep
		}

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

/*
	The scene contains two objects : a cube and a quad. We apply the mask shader on the quad and the
	other on the cube.

	The mask shader is rendered first (Transparent queue) and set the depth buffer value to 1. 
	Then the crate shader is rendered (Opaque queue). The crate is drawn only if the buffer value is equal to 1.
*/