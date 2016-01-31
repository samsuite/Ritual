Shader "Custom/Unlit Emission" {
     Properties {
         _Color ("Main Color", Color) = (1,1,1,1)
		 _EmColor ("Emission Color", Color) = (1,1,1,1)
		 _EmIntensity ("Emission Intensity", Float) = 1.0
     } 
 
     SubShader {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		 Cull Off
	
         CGPROGRAM
         #pragma surface surf Lambert
         
 
         fixed4 _Color;
		 fixed4 _EmColor;
		 float _EmIntensity;
 
         struct Input {
             float2 uv_MainTex;
             float3 viewDir;
         };
 
         void surf (Input IN, inout SurfaceOutput o) {
			 o.Albedo = _Color.rgb;
             o.Emission = _EmColor.rgb * _EmIntensity;
         }
         ENDCG
     }
     FallBack "Self-Illumin/Diffuse"
 }