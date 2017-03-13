Shader "ShaderLib/PostEffect/Post_OutLine" {
	Properties
	{
		//Graphics.Blit() sets the "_MainTex" property to the texture passed in
		_MainTex("Main Texture",2D) = "black"{}
		_SceneTex("Scene Texture",2D) = "black"{}
	}

	SubShader {
		//Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM

			sampler2D _MainTex;

			//<SamplerName>_TexelSize is a float2 that says how much screen space a texel occupies.
			float2 _MainTex_TexelSize;
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvs : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;

				//we are only drawing a quad to the screen
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				//we need to fix the UVs to match our screen space coordinates. There is a Unity define for this that should normally be used.
				o.uvs = o.pos.xy / 2 + 0.5;

				return o;
			}

			half frag(v2f i) : COLOR
			{
				//arbitrary number of iterations for now
				int NumberOfIterations = 20;

				//split texel size into smaller words
				float TX_x = _MainTex_TexelSize.x;

				//and a final intensity that increments based on surrounding intensities.
				float ColorIntensityInRadius;

				//for every iteration we need to do horizontally
				for (int k = 0; k<NumberOfIterations; k += 1)
				{
					//increase our output color by the pixels in the area
					ColorIntensityInRadius += tex2D(_MainTex,i.uvs.xy + float2((k - NumberOfIterations / 2)*TX_x, 0 )).r / NumberOfIterations;
				}

				//output some intensity of teal
				return ColorIntensityInRadius;
			}

			ENDCG
		}//End Pass

		GrabPass{}

		Pass
		{
			CGPROGRAM

			sampler2D _MainTex;
			sampler2D _SceneTex;

			//we need to declare a sampler2D by the name of "_GrabTexture" that Unity can write to during GrabPass{}
			sampler2D _GrabTexture;

			//<SamplerName>_TexelSize is a float2 that says how much screen space a texel occupies.
			float2 _GrabTexture_TexelSize;

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvs : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;

				//Despite the fact that we are only drawing a quad to the screen, Unity requires us to multiply vertices by our MVP matrix, presumably to keep things working when inexperienced people try copying code from other shaders.
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				//Also, we need to fix the UVs to match our screen space coordinates. There is a Unity define for this that should normally be used.
				o.uvs = o.pos.xy / 2 + 0.5;

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				int NumberOfIterations = 20;
				//split texel size into smaller words
				float TX_y = _GrabTexture_TexelSize.y;

				//and a final intensity that increments based on surrounding intensities.
				half ColorIntensityInRadius = 0;

				//if something already exists underneath the fragment (in the original texture), discard the fragment.
				if (tex2D(_MainTex,i.uvs.xy).r>0)
				{
					return tex2D(_SceneTex,float2(i.uvs.x,1 - i.uvs.y));
				}

				//for every iteration we need to do vertically
				for (int j = 0; j<NumberOfIterations; j += 1)
				{
					//increase our output color by the pixels in the area
					ColorIntensityInRadius += tex2D(_GrabTexture,float2(i.uvs.x,1 - i.uvs.y) + float2(0,(j - NumberOfIterations / 2)*TX_y)).r / NumberOfIterations;
				}

				//this is alpha blending, but we can't use HW blending unless we make a third pass, so this is probably cheaper.
				half4 outcolor = ColorIntensityInRadius*half4(0,1,1,1) * 2 + (1 - ColorIntensityInRadius)*tex2D(_SceneTex,float2(i.uvs.x,1 - i.uvs.y));
				return outcolor;
			}
			ENDCG
		}//End Pass
	}
}
