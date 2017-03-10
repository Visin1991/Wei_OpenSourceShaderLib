Shader "ShaderLib/Stencil/ObjectToPit" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }

		Stencil{
			Ref 1
			Comp NotEqual //Only Pass to call when the ref is not 1
			Pass keep //Keep the current contents of the buffer.
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
		Only Pass to call when the Ref is not 1.  and Keep 
		this means when there are other layer behind  the current depth. and Ref is not 1. it Keeps it own color

		if the there are a layer behind the current depth and it Ref is 1. We will just dont not pass to call.

		Also The Stencil operation could  iterate through multiple depth layers.......
	
*/