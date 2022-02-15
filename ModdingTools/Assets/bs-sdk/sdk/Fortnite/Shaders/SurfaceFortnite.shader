Shader "BitshiftProgrammer/SurfaceFortnite" // Goto www.bitshiftprogrammer.com for more
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Main texture", 2D) = "white"{}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Placement("Placement value", Range(-0.0, 100.0)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		sampler2D _MainTex;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Placement;

		struct Input 
		{
			float2 uv_MainTex; // 2nd UV co-ordinate used for the actual texturing purposes
			float4 color : COLOR; // Vertex color
		};

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		float3 ConvertToDir(float3 val)
		{
			val.x = lerp(-val.x * 2, (val.x - 0.5) * 2, step(0.5, val.x));
			val.y = lerp(val.y * 2, (val.y - 0.5) * 2,  step(0.5, val.y));
			val.z = lerp(-val.z * 2, (val.z - 0.5) * 2, step(0.5, val.z));
			
			val = normalize(val);
			return val;
		}

		float4 RotateAroundZInDegrees (float4 vertex, float degrees)
		{
			float angle = radians(degrees);
			float c = cos(angle);
			float s = sin(angle);
			float4x4 rotateZMatrix	= float4x4(	c,	-s,	0,	0,
											 	s,	c,	0,	0,
								  				0,	0,	1,	0,
												0, 0, 0, 1);
			return mul(vertex , rotateZMatrix);
		}
    

		void vert (inout appdata_full v) 
		{
			 float val = max((_Placement - v.color.w*50), 0);
			// val=sqrt(val*5);
			v.vertex = RotateAroundZInDegrees(v.vertex, val * v.color.x*10);
			v.vertex.xyz += ConvertToDir(v.color.xyz) * val;
			
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
