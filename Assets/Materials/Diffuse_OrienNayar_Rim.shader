Shader "Custom/Diffuse_OrienNayar_Rim"{
    Properties {
		_RimColor("RimColor",Color) = (.4,1.0,1.0,1.0)
		_SelfIllum("SelfIllum",Color) = (.5,.5,.5,1.0)
		
		_Roughness("Roughness",Range(0.0,25.0)) = 5.0
		_EnvStrength ("Hemisphere Strength", Range(0.0,5.0)) = 1.0
		_HorizLine("Horizon Line",Range(0.0,5.0)) = 1.0
		
      	_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      LOD 200
      CGPROGRAM
      #pragma target 3.0
      #pragma surface surf WrapLambert

		uniform sampler2D _MainTex;

				
		uniform float4 _RimColor;
		uniform float4 _SelfIllum;
		
		uniform float _Roughness;
		uniform float _EnvStrength;
		uniform float _HorizLine;


		struct Input 
		{
		  float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput OUT) 
		{
		  	OUT.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;

		}

		half4 LightingWrapLambert (SurfaceOutput IN, half3 lightDir, half3 viewDir, half atten) 
		{
			float4 OUT;

			float3 N = normalize(IN.Normal);
			float3 V = normalize(viewDir);
			float3 L = normalize(lightDir);
			
			float VdotN = dot(V,N);
			float NdotL = saturate(dot(N,L));


			float blendV 	= min(NdotL + _HorizLine,1.0);

			
		//	float3 dColor	= IN.Albedo;
			float3 dColor 	= lerp( IN.Albedo.rgb, lerp( _RimColor, IN.Albedo.rgb, blendV), _EnvStrength);
			
			
			// Nayar Commands
			//-----------------------------
			float AngleViewNormal = acos(VdotN);
			float AngleLightNormal = acos(NdotL);

			float AngleDifference =
				max(0.0,
					dot(
						normalize(viewDir - IN.Normal*VdotN),
						normalize(lightDir - IN.Normal*NdotL)
					)
				);

			float LProjected = max(AngleViewNormal, AngleLightNormal); // light projection
			float VProjected = min(AngleViewNormal, AngleLightNormal); // view projection

			float A = 1.0 - (0.5 * _Roughness) / (_Roughness + 0.33);
			float B = (0.45 * _Roughness) / (_Roughness + 0.09);

			float diffuseV = (B * AngleDifference * sin(LProjected) * tan(VProjected) + A) * NdotL;

			OUT.rgb = dColor * saturate((diffuseV * _LightColor0.rgb * atten * 2) + _SelfIllum.rgb);
			OUT.a 	= IN.Alpha;

			return OUT;	
    }
    ENDCG
  }
    
  Fallback "Diffuse"
  
}
