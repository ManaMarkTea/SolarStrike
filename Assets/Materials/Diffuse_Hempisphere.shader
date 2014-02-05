Shader "Custom/Diffuse_Hempisphere"{
    Properties {
		_SkyColor("SkyColor",Color) = (.4,1.0,1.0,1.0)
		_BackColor("BackColor",Color) = (.514,.384,.2,1.0)
		
		_EnvStrength ("Hemisphere Strength", Range(0.0,5.0)) = 1.0
		_ReflectStrength ("Reflect Strength", Range(0.0,5.0)) = .2
		
		_Cube ("Cubemap", CUBE) = "" {}
      	_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      LOD 200
      CGPROGRAM
      #pragma surface surf WrapLambert

		uniform sampler2D _MainTex;
		uniform samplerCUBE _Cube;
				
		uniform float4 _SkyColor;
		uniform float4 _BackColor;
		
		uniform float _HorizLine;
		uniform float _EnvStrength;
		uniform float _ReflectStrength;


		struct Input 
		{
		  float2 uv_MainTex;
		  float3 worldRefl;
		};

		void surf (Input IN, inout SurfaceOutput OUT) 
		{
		  	OUT.Albedo = lerp( tex2D (_MainTex, IN.uv_MainTex).rgb, texCUBE(_Cube, (IN.worldRefl * float3(-1.0,1.0,1.0)) ).rgb,_ReflectStrength);

		}

		half4 LightingWrapLambert (SurfaceOutput IN, half3 lightDir, half atten) 
		{
			float4 OUT;

			half NdotL = dot(IN.Normal, lightDir);
			float3 _EnviColor =  lerp( float3(NdotL,NdotL,NdotL), lerp(_BackColor.rgb, _SkyColor.rgb, NdotL ), _EnvStrength);

			OUT.rgb = IN.Albedo * _LightColor0.rgb * ((_EnviColor * .5 + .5) * atten * 2);


			OUT.a = IN.Alpha;
			return OUT;
		}

		ENDCG
    }
    Fallback "Diffuse"
  }
