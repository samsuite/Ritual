Shader "Custom/Transparent Halo" {
     Properties {
         _Color ("Main Color", Color) = (1,1,1,1)
         _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
         _RimPower ("Rim Exponent", Range(0.5,25.0)) = 3.0
		 _EmIntensity ("Emission Intensity", Float) = 1.0
         _BaseAlpha ("Base Alpha", Range(0.0,1.0)) = 1.0
     }
 
 
 
     SubShader {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
 
         CGPROGRAM
         #pragma surface surf Lambert alpha
         
 
         sampler2D _MainTex;
         fixed4 _Color;
		 fixed4 _EmColor;
         float _RimPower;
         float _BaseAlpha;
		 float _EmIntensity;
 
         struct Input {
             float2 uv_MainTex;
             float3 viewDir;
         };
 
         void surf (Input IN, inout SurfaceOutput o) {
             fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
             half rim = saturate(dot (normalize(IN.viewDir), o.Normal));
             o.Emission = c.rgb * (1- pow (rim, _RimPower) * _BaseAlpha * c.a) * _EmIntensity;
             o.Alpha =  1- pow (rim, _RimPower) * _BaseAlpha * c.a;
         }
         ENDCG
     }
     FallBack "Self-Illumin/Diffuse"
 }