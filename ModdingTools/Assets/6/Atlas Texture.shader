 
Shader "Atlas Texture"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _LightMap ("Lightmap (RGB)", 2D) = "white" {}
        
		_SrcBlend ("_SrcBlend", Float) = 1
		_DstBlend ("_DstBlend", Float) = 0
		_ZWrite ("_ZWrite", Float) = 1
		_AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "LightMode" = "ForwardBase"
            "Queue" = "Geometry"
            "RenderType"="Opaque" 
            "ForceNoShadowCasting" = "False"
        }
 
        Cull Off
 
        Pass
        {
        	Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			
            CGPROGRAM
            #define mod(x, y) (x - y * floor(x / y))
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
           // #pragma enable_d3d11_debug_symbols
            #pragma shader_feature _ _RENDERING_FADE _RENDERING_CUTOUT
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 lightmap : TEXCOORD1;
                float4 frontRect : TEXCOORD2;
                 
            };
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 lightmap : TEXCOORD1;
                float4 frontRect : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                SHADOW_COORDS(4)
                fixed4 diff : COLOR0; 
            };
           
            v2f vert (appdata v)
            {
                v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
				TRANSFER_SHADOW(o)
				
                o.normal = v.normal;
                o.uv = v.uv;
                o.uv*=v.frontRect.zw; //scaling fix added
                o.frontRect = v.frontRect;
                
                o.lightmap = v.lightmap;
                
                //diffuse
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0;
                o.diff.rgb += ShadeSH9(half4(worldNormal,1));
                
                
                
                return o;
            }
           
           float _AlphaCutoff;
            
 
            sampler2D _MainTex;
            sampler2D _LightMap;
            float4 frag (v2f i) : SV_Target
            {
                
                fixed shadow = SHADOW_ATTENUATION(i);               
                float3 worldNormal = normalize(mul((float3x3)UNITY_MATRIX_M, i.normal));
 
                
                
 
                // Select the correct UV coordinate and bounding box.
                float2 uv = i.uv.xy ;
                float4 rect =  i.frontRect;
                 
                // uv derivatives
                float2 uv_ddx = ddx(uv);
                float2 uv_ddy = ddy(uv);
                
                // apply modul wrap to uv
                uv.x = mod(uv.x , rect.z) + rect.x;
                uv.y = mod(uv.y , rect.w) + rect.y;

 
                float4 sample = tex2Dgrad(_MainTex, uv, uv_ddx, uv_ddy);
                
                #if defined(_RENDERING_CUTOUT)
                     clip(sample.a - _AlphaCutoff);
                #endif
                
                
                return  (1.0+i.diff) *sample *  min(tex2D(_LightMap, i.lightmap), max(shadow,.5));
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    fallback "VertexLit"
    CustomEditor "AtlasTextureGUI"
}
 