// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GameCore/Mobile/Water/Diffuse"
{
	Properties{
		_ReflectionTex("Reflection", 2D) = "white" {}
		_RefractionTex("Refraction", 2D) = "white" {}
		_DUDVMap("DUDV map",2D) = "white"{}
		_Tililng("DUDV Tiling",Range(1,10)) = 6
		_Strength("DUDV Strength",Range(0.001,0.5)) = 0.02
		_RefColor("Color",Color) = (1,1,1,1)
		_WaveSpeed("Wave Speed",Range(0.02,20)) = 0.02
		_NormalMap("Water Normal",2D) = "white"{}
		_ShineDamper("Shine Damper",Range(1,100))= 20

	}
	SubShader{
	Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

	ZWrite Off
    Lighting Off
	Cull Off
	Fog{ Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha

	LOD 100
	Pass{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag

	#include "UnityCG.cginc"
	#include "UnityLightingCommon.cginc"
	

	//uniform float4x4 _ProjMatrix;
	uniform float _RefType;
	sampler2D _ReflectionTex;
	sampler2D _RefractionTex;
	sampler2D _DUDVMap;
	sampler2D _NormalMap;
	float4 _RefColor;
	int _Tililng;
	half _Strength;
	half _WaveSpeed;
	half _ShineDamper;

	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

	struct outvertex {
		float4 pos : SV_POSITION;
		float2 uv1 : TEXCOORD0;
		float3 worldPos : TEXCOORD1;
		float4 refparam : COLOR0;//r:fresnel,g:none,b:none,a:none
		float4 clipSpace : WTF;
	};

	outvertex vert(appdata_tan v) {
		outvertex o;
		o.worldPos = mul(UNITY_MATRIX_M,v.vertex);
		o.pos =  UnityObjectToClipPos(v.vertex);
		o.clipSpace = o.pos;

		float3 r = normalize(ObjSpaceViewDir(v.vertex));
		float d = saturate(dot(r,normalize(v.normal)));//r+(1-r)*pow(d,5)				
		o.refparam = float4(d,0,0,0);
		o.uv1 = v.texcoord * _Tililng;
		return o;
	}

	float4 frag(outvertex i) : COLOR
	{
		half2 dnc = (i.clipSpace.xy / i.clipSpace.w) / 2.0f + 0.5f;
		half2 coords = half2(dnc.x, dnc.y);
		coords.y = 1 - coords.y;
		
		fixed xScroll = _WaveSpeed * _Time;

		//half2 uv1 = half2(i.uv1.x + xScroll, i.uv1.y);
		//half2 uv2 = half2(-i.uv1.x + xScroll, i.uv1.y + xScroll);
		//half2 distortion1 = (tex2D(_DUDVMap, uv1).rg * 2.0f - 1.0f) * _Strength;
		//half2 distortion2 = (tex2D(_DUDVMap, uv2).rg * 2.0f - 1.0f) * _Strength;
		//half2 totalDistortion = distortion1 + distortion2;

		half2 uvDis = tex2D(_DUDVMap, half2(i.uv1.x + xScroll, i.uv1.y)).rg * 0.1;
		uvDis = i.uv1 + float2(uvDis.x, uvDis.y + xScroll);
		half2 totalDistortion = (tex2D(_DUDVMap, uvDis).rg * 2.0 - 1.0) * _Strength;

	
		half2 refCoord = coords + totalDistortion;
		half2 refraCoord = coords + totalDistortion;

		half4 flecol = tex2D(_ReflectionTex,refCoord);
		half4 fracol = tex2D(_RefractionTex,refraCoord);

		half4 outcolor = half4(1,1,1,1);
		if (_RefType == 0)
		{
			float f =  i.refparam.r / pow((i.refparam.r),-1.5f);
			outcolor = lerp(flecol, fracol, f);
		}
		else if (_RefType == 1)
		{
			outcolor = flecol;
		}
		else if (_RefType == 2)
		{
			outcolor = fracol;
		}

		outcolor *= _RefColor;

		//Fake Normal
		half4 normalMapColor = tex2D(_NormalMap, uvDis);
		half3 normal = half3(normalMapColor.r * 2.0 - 1, normalMapColor.b, normalMapColor.g * 2.0 - 1.0);
		normal = normalize(normal);
		

		float3 lightDir = _WorldSpaceLightPos0.xyz;
		float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
		float3 lightColor = _LightColor0.rgb;
		float3 reflectionLighting = reflect(-lightDir, normal);

		float specular = max(dot(reflectionLighting, viewDir), 0.0);
		specular = pow(specular, _ShineDamper);
		float3 specularHighlights = lightColor * specular;

		//float depth = i.clipSpace.z / i.clipSpace.w;
		//float depth_Buffer = (_ProjectionParams.z * (depth - _ProjectionParams.y)) / (depth * (_ProjectionParams.z - _ProjectionParams.y));
		
		/*float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, coords);
		d = Linear01Depth(d);*/

		//return float4(0, d, 0,1);
		return lerp(outcolor, float4(0.0, 0.3, 0.5, 1.0), 0.5) + float4(specularHighlights, 1);

	}
		ENDCG
	}
	}
}