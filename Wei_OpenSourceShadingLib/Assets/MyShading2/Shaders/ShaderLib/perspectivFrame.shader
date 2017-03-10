Shader "ShaderLib/Stencil/perspectivFrame" {
	Properties{
		_MainColor("Main color", Color) = (.34, .85, .92, 1) // color
		_AlphaValue("the Alpha Value",Range(0,1.0)) = 0.5	
	}

	SubShader{
		Tags{ "RenderType" = "Transparent" }

		Stencil{
			Ref 1
			Comp always			//Make the stencil test always pass.
			Pass replace		//Write the reference value into the buffer.
		}

		CGPROGRAM
		#pragma surface surf Lambert alpha

		struct Input {
			fixed3 Albedo;
		};
		
		half3 _MainColor;
		half _AlphaValue;

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _MainColor;
			o.Alpha = _AlphaValue;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

/*
		in this Stencil shader. We always pass the stencil test, no matter what the reference value is.
	and always replace what ever behind it's depth pixels.
*/