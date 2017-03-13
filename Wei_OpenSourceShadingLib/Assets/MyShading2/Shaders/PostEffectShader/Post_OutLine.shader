Shader "ShaderLib/PostEffect/Post_OutLine" {
	Properties
	{
		//Graphics.Blit() sets the "_MainTex" property to the texture passed in
		_MainTex("Main Texture",2D) = "black"{}
	}
	SubShader {
		//Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM

			sampler2D _MainTex;

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

			uniform int NumberOfIterations = 9;

			half4 frag(v2f i) : COLOR
			{
				
				//Split texel size into smaller world
				float TX_x = _MainTex_TexelSize.x;
				float TX_y = _MainTex_TexelSize.y;

				float ColorIntensityInRadius;

				//if something already exists underneath the fragment, discard the fragment.
				if (tex2D(_MainTex, i.uvs.xy).r>0)
				{
					discard;
				}

				//for every iteration we need to do horizontally
				for (int k = 0; k<NumberOfIterations; k += 1)
				{
					//for every iteration we need to do vertically
					for (int j = 0; j<NumberOfIterations; j += 1)
					{
						//increase our output color by the pixels in the area
						ColorIntensityInRadius += tex2D(_MainTex, i.uvs.xy + float2((k - NumberOfIterations / 2)*TX_x,(j - NumberOfIterations / 2)*TX_y)).r;
					}
				}

				//output some intensity of teal
				return ColorIntensityInRadius*half4(0, 1, 1, 1);
			}

			ENDCG
		}
	}
}
