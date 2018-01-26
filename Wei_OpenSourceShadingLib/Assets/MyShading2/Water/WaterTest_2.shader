// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GameCore/Mobile/Water/Diffuse2"
{
	Properties{
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_ReflectionTex("Reflection", 2D) = "white" {}
		_RefractionTex("Refraction", 2D) = "white" {}
		_DUDVMap("DUDV map",2D) = "white"{}
		_DistortionStrength("DUDV Strength",Range(0.001,0.03)) = 0.02
		_DistortionSpeedScaler("DS Speed Scaler",Float) = 1.0
		_NormalMap("Normals", 2D) = "bump" {}
		_Speed1("Speed 1",Vector) = (1,1,1,1)
		_BumpScale("Bump Scale", Range(-2,2)) = 1
		[Gamma] _Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.1
		[NoScaleOffset] _DetailNormalMap("Detail Normals", 2D) = "bump" {}
		_DetailBumpScale("Detail Bump Scale",  Range(-2,2)) = 1
		_Speed2("Speed 2",Vector) = (-1,-1,1,1)
	}
	SubShader{
	Tags{ "LightMode" = "ForwardBase" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

	ZWrite Off
	Lighting Off
	Cull Off
	Fog{ Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	Pass{
	CGPROGRAM

	#pragma target 3.0
	#pragma multi_compile _ SHADOWS_SCREEN
	#pragma multi_compile _ VERTEXLIGHT_ON

	#pragma vertex MyVertexProgram
	#pragma fragment MyFragmentProgram

	#include "AutoLight.cginc"
	#include "UnityPBSLighting.cginc"
	
	uniform float _RefType;
	float4 _Tint;
	sampler2D _DUDVMap;
	sampler2D _ReflectionTex, _RefractionTex;
	sampler2D _NormalMap, _DetailNormalMap;
	float4 _NormalMap_ST, _DetailNormalMap_ST;
	float _BumpScale, _DetailBumpScale;
	float3 _Speed1;
	float3 _Speed2;

	float _Metallic;
	float _Smoothness;

	float _DistortionSpeedScaler;
	float _DistortionStrength;

	//UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

		struct VertexData {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
		};

		struct Interpolators {
			float4 pos : SV_POSITION;
			float4 uv : TEXCOORD0;
			float3 normal : TEXCOORD1;
			float3 worldPos : TEXCOORD2;

			#if defined(SHADOWS_SCREEN)
						SHADOW_COORDS(3)
			#endif

			#if defined(VERTEXLIGHT_ON)
				float3 vertexLightColor : TEXCOORD4;
			#endif

			
			float4 clipSpace : WTF;
		};

		void ComputeVertexLightColor(inout Interpolators i) {
			#if defined(VERTEXLIGHT_ON)
			i.vertexLightColor = Shade4PointLights(
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb,
				unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, i.worldPos, i.normal
			);
			#endif
		}

		Interpolators MyVertexProgram(VertexData v) {
			Interpolators i;
			i.pos = UnityObjectToClipPos(v.vertex);
			i.worldPos = mul(unity_ObjectToWorld, v.vertex);
			i.normal = UnityObjectToWorldNormal(v.normal);
			i.uv.xy = TRANSFORM_TEX(v.uv, _NormalMap);
			i.uv.zw = TRANSFORM_TEX(v.uv, _DetailNormalMap);
			i.clipSpace = i.pos;

#if defined(SHADOWS_SCREEN)
			TRANSFER_SHADOW(i);
#endif

			ComputeVertexLightColor(i);
			return i;
		}

		UnityLight CreateLight(Interpolators i) {
			UnityLight light;

			#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
				light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
			#else
				light.dir = _WorldSpaceLightPos0.xyz;
			#endif

	
			UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos);
		

			light.color = _LightColor0.rgb * attenuation;
			light.ndotl = DotClamped(i.normal, light.dir);
			return light;
		}

		UnityIndirect CreateIndirectLight(Interpolators i) {
			UnityIndirect indirectLight;
			indirectLight.diffuse = 0;
			indirectLight.specular = 0;

			#if defined(VERTEXLIGHT_ON)
				indirectLight.diffuse = i.vertexLightColor;
			#endif

			#if defined(FORWARD_BASE_PASS)
				indirectLight.diffuse += max(0, ShadeSH9(float4(i.normal, 1)));
				//float3 envSample = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.normal);
				//indirectLight.specular = envSample;
			#endif
			return indirectLight;
		}

		void InitializeFragmentNormal(inout Interpolators i) {
			float2 normal1_xy = _Speed1.xy;
			normal1_xy = normalize(normal1_xy);
			float2 normal2_xy = _Speed2.xy;
			normal2_xy = normalize(normal2_xy);

			float2 xy;
			xy.x = i.uv.x + normal1_xy.x * _Speed1.z * _Time;
			xy.y = i.uv.y + normal1_xy.y * _Speed1.z * _Time;

			float2 zw;
			zw.x = i.uv.z + normal2_xy.x * _Speed2.z * _Time;
			zw.y = i.uv.w + normal2_xy.y * _Speed2.z * _Time;

			float3 mainNormal = UnpackScaleNormal(tex2D(_NormalMap, xy), _BumpScale);
			float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, zw), _DetailBumpScale);
			i.normal = BlendNormals(mainNormal, detailNormal);
			i.normal = i.normal.xzy;
		}

		float4 MyFragmentProgram(Interpolators i) : SV_TARGET{

				float3 r = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float  d = saturate(dot(r, normalize(i.normal)));//r+(1-r)*pow(d,5)	

				half2 dnc = (i.clipSpace.xy / i.clipSpace.w) / 2.0f + 0.5f;
				half2 coords = half2(dnc.x, dnc.y);
				coords.y = 1 - coords.y;

				//fixed xScroll = _DistortionSpeed * _Time;
				fixed xScroll = ((_Speed1.z + _Speed2.z) * _Time / 8.0f) * _DistortionSpeedScaler;
				half2 uvDis = tex2D(_DUDVMap, half2(i.uv.x + xScroll, i.uv.y)).rg * 0.1;
				uvDis = i.uv + float2(uvDis.x, uvDis.y + xScroll);
				half2 totalDistortion = (tex2D(_DUDVMap, uvDis).rg * 2.0 - 1.0) * _DistortionStrength;



				half2 refCoord;
				half2 refraCoord; 

				half4 flecol;
				half4 fracol;

				float fresnel = d / pow((d), -1.5f);
				half4 outcolor = half4(1,1,1,1);
				if (_RefType == 0)
				{
					refCoord = coords + totalDistortion;
					flecol = tex2D(_ReflectionTex, refCoord);

					refraCoord = coords + totalDistortion;
					fracol = tex2D(_RefractionTex, refraCoord);

					outcolor = lerp(flecol, fracol, fresnel);
				}
				else if (_RefType == 1)
				{
					refCoord = coords + totalDistortion;
					flecol = tex2D(_ReflectionTex, refCoord);
					outcolor = flecol;
				}
				else if (_RefType == 2)
				{
					refraCoord = coords + totalDistortion;
					fracol = tex2D(_RefractionTex, refraCoord);
					outcolor = fracol;
				}
				else if (_RefType == 3)
				{
					refraCoord = coords + totalDistortion;
					fracol = tex2D(_RefractionTex, refraCoord);
					outcolor = fracol;
				}

				half3 tint = lerp(_Tint.rgb, float3(1, 1, 1), fresnel);
				float3 albedo = outcolor.rgb *tint;

				InitializeFragmentNormal(i);

				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
		

				float3 specularTint;
				float oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(
					albedo, _Metallic, specularTint, oneMinusReflectivity
				);

				return UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, _Smoothness,
					i.normal, viewDir,
					CreateLight(i), CreateIndirectLight(i)
				);
			}

		ENDCG
		}
	}

	CustomEditor "WaterShaderEditor"
}