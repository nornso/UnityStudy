﻿Shader "Fractal/Fractal Surface GPU"
{

	Properties{
		_Color("Albedo" ,Color) = (1.0,1.0,1.0)
		_Smoothness("Smoothness", Range(0,1)) = 0.5
	}

		SubShader
	{
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
		#pragma instancing_options procedural:ConfigureProcedural
		#pragma editor_sync_compilation

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 4.5
		#include "FractalGPU.hlsl"

		struct Input {
			float3 worldPos;
		};

		float4 _Color;
		float _Smoothness;

		void ConfigureSurface(Input input, inout SurfaceOutputStandard surface)
		{
			surface.Albedo = _Color.rgb;
			surface.Smoothness = _Smoothness;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
