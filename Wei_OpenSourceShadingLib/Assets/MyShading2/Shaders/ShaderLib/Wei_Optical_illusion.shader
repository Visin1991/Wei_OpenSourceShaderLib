// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ShaderLib/Optical_Illusion" {
	Properties{
		_OrigineX("PosX Origine", Range(0,1)) = 0.5
		_OrigineY("PosY Origine", Range(0,1)) = 0.5
		_Speed("Speed", Range(-100,100)) = 60.0
		_CircleNbr("Circle quantity", Range(2,1000)) = 60.0
	}

SubShader{

	Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		#pragma target 3.0

		float _OrigineX;
		float _OrigineY;
		float _Speed;
		float _CircleNbr;

		struct vertexInput {
			float4 vertex : POSITION;
			float4 texcoord0 : TEXCOORD0;
		};

		struct fragmentInput {
			float4 position : SV_POSITION;
			float4 texcoord0 : TEXCOORD0;
		};

		fragmentInput vert(vertexInput i) {
			fragmentInput o;
			o.position = UnityObjectToClipPos(i.vertex);
			o.texcoord0 = i.texcoord0;
			return o;
		}
		
		fixed4 frag(fragmentInput i) : SV_Target{
			fixed4 color;
			float time = _Time.x * _Speed;
			float xdist = _OrigineX - i.texcoord0.x;
			float ydist = _OrigineY - i.texcoord0.y;

			color = sin(atan2(ydist, xdist) * _CircleNbr + time); //when color = float. it means black and white with Alpha value
			//atan2(y,x) :  the angle from X axis to position (y,x).
			//atan2(x,y) = 180 - atan2(y-x)
			//Kan tu.....
			return color;
		}
		ENDCG
	}
}
}
/*
there are more http://wenku.baidu.com/view/93c31bf7aeaad1f346933f2c.html

	atan(k) : 定义域（-∞ ，+∞）
			: 值域（-π/2 ，π/2）
			: 单调性->在（-∞ ，+∞）上为增函数

	atan2
	(0, 1) with angle π/2 (in radians);
	(−1, 0) with angle π;
	0, −1) with angle 3π/2;
	to (1, 0) with angle 0 = (2πn mod 2π).

	C语言中的atan和atan2
	-----------------------------------
	在C语言的math.h或C++中的cmath中有两个求反正切的函数atan(double x)与atan2(double y,double x)  他们返回的值是弧度 要转化为角度再自己处理下。

	前者接受的是一个正切值（直线的斜率）得到夹角，但是由于正切的规律性本可以有两个角度的但它却只返回一个，因为atan的值域是从-90~90 也就是它只处理一四象限，所以一般不用它。

	第二个atan2(double y,double x) 其中y代表已知点的Y坐标 同理x ,返回值是此点与远点连线与x轴正方向的夹角，这样它就可以处理四个象限的任意情况了，它的值域相应的也就是-180~180了

	例如：

	例1：斜率是1的直线的夹角

	cout<<atan(1.0)*180/PI;//45°

	cout<<atan2(1.0,1.0)*180/PI;//45° 第一象限

	cout<<atan2(-1.0,-1.0)*180/PI;//-135°第三象限

	后两个斜率都是1 但是atan只能求出一个45°

	例2：斜率是-1的直线的角度

	cout<<atan(-1.0)*180/PI;//-45°

	cout<<atan2(-1.0,1.0)*180/PI;//-45° y为负 在第四象限

	cout<<atan2(1.0,-1.0)*180/PI;//135° x为负 在第二象限

	常用的不是求过原点的直线的夹角 往往是求一个线段的夹角 这对于atan2就更是如鱼得水了

	例如求A(1.0,1.0) B(3.0,3.0)这个线段AB与x轴正方向的夹角

	用atan2表示为 atan2(y2-y1,x2-x1) 即 atan2(3.0-1.0,3.0-1.0)

	它的原理就相当于把A点平移到原点B点相应变成B'（x2-x1,y2-y1）点 这样就又回到先前了

	例三：

	A(0.0,5.0) B(5.0,10.0)

	线段AB的夹角为

	cout<<atan2(5.0,5.0)*180/PI;//45°
	---------------------------------
*/