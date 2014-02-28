Shader "Custom/OrenNayar_Rim"{
    Properties {
		_SkyColor("SkyColor",Color) = (.4,1.0,1.0,1.0)
		_BackColor("BackColor",Color) = (.514,.384,.2,1.0)
		
		_LightDir("Light Direction", Color) = (-0.577, -0.577, 0.577)
		_SelfIllum("Self Illumination", Color) = (0.5,0.5,0.5)
		_Roughness("Roughness", Range(0.0, 25.0)) = 5.0
		_HorizonLine("HorizonLine", Range(-1.0, 1.0)) = 5.0
		_EnvStrength ("Hemisphere Strength", Range(0.0,5.0)) = 1.0
		
		_RimColor("Rim Color", Color) = (0, .4, .5)
		
      	_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      LOD 200
      CGPROGRAM
      #pragma surface PixelProgram WrapLambert

	 
	uniform float3 _LightDir;
	uniform sampler2D _MainTex;
	uniform float3 _SelfIllum;
	uniform float _Roughness;
	uniform float _HorizonLine;
	uniform float _EnvStrength;	
	uniform float3 _RimColor;
	
	struct VERTEX_IN
	{
	#ifdef MGFX
		float4 	Position   		: SV_POSITION;
	#else 
		float4	Position   		: POSITION;
	#endif
		float3 	Normal			: NORMAL;
		float4 	Color				: COLOR0;
		float2 	uv0				: TEXCOORD0;
	};

	struct VERTEX_OUT
	{
	#ifdef MGFX
		float4 	position   		: SV_POSITION;
	#else 
		float4 	position   		: POSITION;
	#endif
		float2	uv0				: TEXCOORD0;
		float3	eyeVector		: TEXCOORD1;	
		float3	normal			: TEXCOORD2;
		float4	vertColor		: COLOR0;

	};

		struct PIXEL_OUT
		{
			float4 Color : COLOR0;
		};

		VERTEX_OUT VertexProgram(VERTEX_IN IN)
		{
			VERTEX_OUT OUT;
		  
			float4 WorldPos = mul(IN.Position, World);

			OUT.position		= mul(IN.Position, WorldViewProj);
			OUT.normal			= normalize((mul(float4(IN.Normal, 0), World)).xyz);
			OUT.eyeVector	= CameraPos - WorldPos.xyz;

			#ifndef MGFX
			OUT.vertColor 	= float4(1,1,1,1);
			#else
			OUT.vertColor 	= IN.Color;
			#endif
			
			OUT.uv0				= IN.uv0;

			return OUT;
		}

		void PixelProgram(VERTEX_OUT IN, inout SurfaceOutput OUT)
		{
			

			float3 N = normalize(IN.normal);
			float3 V = normalize(IN.eyeVector);
			float3 L = normalize(_LightDir);
			
			float VdotN = dot(V,N);
			float NdotL = saturate(dot(N,L));

			half4 dColor	= tex2D(_MainTex, IN.uv0);
			float blend 		= min(NdotL+ _HorizonLine,1.0);
			
			dColor.rgb = lerp( dColor, lerp( _RimColor, dColor, blend), _EnvStrength);
			
			
			// Nayar Commands
			//-----------------------------
			float AngleViewNormal = acos(VdotN);
			float AngleLightNormal = acos(NdotL);

			float AngleDifference =
				max(0.0,
					dot(
						normalize(IN.eyeVector - IN.normal*VdotN),
						normalize(LightDir - IN.normal*NdotL)
					)
				);

			float LProjected = max(AngleViewNormal, AngleLightNormal); // light projection
			float VProjected = min(AngleViewNormal, AngleLightNormal); // view projection

			float A = 1.0 - (0.5 * _Roughness) / (_Roughness + 0.33);
			float B = (0.45 * _Roughness) / (_Roughness + 0.09);

			float diffuse = (B * AngleDifference * sin(LProjected) * tan(VProjected) + A) * NdotL;


			OUT.Albedo.rgb 	= dColor * saturate(IN.vertColor.rgb * (diffuse + _gSelfIllum) );

			//OUT.Albedo.a 	= dColor.a * IN.vertColor.a;

			//return OUT;
		}


		ENDCG
    }
    Fallback "Diffuse"
  }

  
  
  
  
  
  
  
  