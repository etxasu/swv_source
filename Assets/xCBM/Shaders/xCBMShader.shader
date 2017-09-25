Shader "Hidden/xCBMShader" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off
				
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		// features are enabled via enum!
		#pragma shader_feature PROTANOPIA PROTANOMALY DEUTERANOPIA DEUTERANOMALY TRITANOPIA TRITANOMALY ACHROMATOPSIA ACHROMATOMALY
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

		fixed4 frag (v2f_img i) : SV_Target	{

			// float3x3/mat3 source: https://github.com/PlanetCentauri/ColorblindFilter (see also subfolder "ColorblindFilter")
			#ifdef PROTANOPIA
			float3x3 m = float3x3(0.567, 0.433, 0.0  ,  0.558, 0.442, 0.0  ,  0.0  , 0.242, 0.758); 	// protanopia
			#endif
			#ifdef PROTANOMALY
			float3x3 m = float3x3(0.817, 0.183, 0.0  ,  0.333, 0.667, 0.0  ,  0.0  , 0.125 ,0.875); 	// protanomaly
			#endif
			#ifdef DEUTERANOPIA
			float3x3 m = float3x3(0.625, 0.375, 0.0  ,  0.7  , 0.3  , 0.0  ,  0.0  , 0.3   ,0.7  ); 	// deuteranopia
			#endif
			#ifdef DEUTERANOMALY
			float3x3 m = float3x3(0.8  , 0.2  , 0.0  ,  0.258, 0.742, 0.0  ,  0.0  , 0.142 ,0.858); 	// deuteranomaly
			#endif
			#ifdef TRITANOPIA
			float3x3 m = float3x3(0.95 , 0.05 , 0.0  ,  0.0  , 0.433, 0.567,  0.0  , 0.475 ,0.525); 	// tritanopia
			#endif
			#ifdef TRITANOMALY
			float3x3 m = float3x3(0.967, 0.033, 0.0  ,  0.0  , 0.733, 0.267,  0.0  , 0.183 ,0.817); 	// tritanomaly
			#endif
			#ifdef ACHROMATOPSIA
			float3x3 m = float3x3(0.299, 0.587, 0.114,  0.299, 0.587, 0.114,  0.299, 0.587 ,0.114); 	// achromatopsia
			#endif
			#ifdef ACHROMATOMALY
			float3x3 m = float3x3(0.618, 0.320, 0.062,  0.163, 0.775, 0.062,  0.163, 0.320 ,0.516);		// achromatomaly
			#endif


			fixed4 original = tex2D(_MainTex, i.uv);

			fixed3 o = original.rgb;
			o = mul(o, m);
			fixed4 output = float4(o.r, o.g, o.b, 1.0);
			output.a = original.a;
			
			return output;
		}
		ENDCG

		}
	}

	Fallback off
}
