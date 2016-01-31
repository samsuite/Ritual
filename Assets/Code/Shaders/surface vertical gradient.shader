Shader "Custom/Surface Vertical Gradient" {
	Properties {
	_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color1 ("Color 1", Color) = (0.5, 0.5, 0.5, 1)
        _Color2 ("Color 2", Color) = (0.5, 0.5, 0.5, 1)
		_MaxHeight ("Max Height", Float) = 0.0
        _MinHeight ("Min Height", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Off
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		float4 _Color1;
        float4 _Color2;
		float _MaxHeight;
        float _MinHeight;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float height = IN.worldPos.y;
			float lerpValue = clamp((height - _MinHeight) / (_MaxHeight - _MinHeight), 0, 1);
			o.Albedo = lerp (_Color2.rgb, _Color1.rgb, lerpValue);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
