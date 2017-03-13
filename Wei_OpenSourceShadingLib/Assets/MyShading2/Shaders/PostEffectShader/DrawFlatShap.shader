//This shader goes on the objects themselves. It just draws the object as white, and has the "Outline" layer.
Shader "ShaderLib/PostEffect/DrawFlatShap" {
	SubShader{
		ZWrite Off
		ZTest Always
		Lighting Off
		Pass
		{
			CGPROGRAM
			#pragma vertex VShader
			#pragma fragment FShader

			struct vtf
			{
				float4 pos:SV_POSITION; //顶点位置-->映射到镜头后的位置
			};
			
			//仅仅是把 顶点位置 转化到 镜头所对应的位置
			vtf VShader(vtf i)
			{
				vtf o;
				o.pos = mul(UNITY_MATRIX_MVP, i.pos);
				return o;
			}

			half4 FShader() :COLOR0
			{
				return half4(1,1,1,1);
			}
			ENDCG
		}
	}
}
