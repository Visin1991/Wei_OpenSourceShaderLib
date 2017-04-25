Shader "GameCore/Mobile/Water/Diffuse"
{
	Properties{
		_ReflectionTex("Reflection", 2D) = "white" {}
		_RefractionTex("Refraction", 2D) = "white" {}
		_RefColor("Color",Color) = (1,1,1,1)
	}
	SubShader{
	Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
	ZWrite On Lighting Off Cull Off Fog{ Mode Off } Blend One Zero
	LOD 100
	Pass{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

	uniform float4x4 _ProjMatrix;
	uniform float _RefType;
	sampler2D _ReflectionTex;
	sampler2D _RefractionTex;
	float4 _RefColor;

	struct outvertex {
		float4 pos : SV_POSITION;
		float4 uv0 : TEXCOORD0;
		float4 refparam : COLOR0;//r:fresnel,g:none,b:none,a:none
	};

	outvertex vert(appdata_tan v) {
		outvertex o;
		o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
		float4 posProj = mul(_ProjMatrix, v.vertex);
		o.uv0 = posProj;
		float3 r = normalize(ObjSpaceViewDir(v.vertex));
		float d = saturate(dot(r,normalize(v.normal)));//r+(1-r)*pow(d,5)				
		o.refparam = float4(d,0,0,0);

		return o;
	}

	float4 frag(outvertex i) : COLOR
	{
		half4 flecol = tex2D(_ReflectionTex,i.uv0 / i.uv0.w);
		half4 fracol = tex2D(_RefractionTex,i.uv0 / i.uv0.w);

		half4 outcolor = half4(1,1,1,1);
		if (_RefType == 0)
		{
			outcolor = lerp(flecol,fracol,i.refparam.r);
		}
		else if (_RefType == 1)
		{
			outcolor = flecol;
		}
		else if (_RefType == 2)
		{
			outcolor = fracol;
		}
		return outcolor*_RefColor;
	}
		ENDCG
	}
	}
}