Shader "Custom/Vertical Tint with Stripes"
{
    Properties 
    {
		_FogColor ("Top Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogColor2 ("Bottom Color", Color) = (0.5, 0.5, 0.5, 1)
		_StripeColor ("Stripe Top Color", Color) = (0.5, 0.5, 0.5, 1)
		_StripeColor2 ("Stripe Bottom Color", Color) = (0.5, 0.5, 0.5, 1)
        _FogMaxHeight ("Max Height", Float) = 1.0
        _FogMinHeight ("Min Height", Float) = 0.0
		_StripeHeight ("Stripe Height", Float) = 0.2
		_StripeDensity ("Stripe Density", Float) = 20
		_CelDensity ("Cel Density", Float) = 20
    }
  
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 200
        Cull Back
        ZWrite On
  
        CGPROGRAM
  
        #pragma surface surf Lambert finalcolor:finalcolor vertex:vert 
  
        float4 _FogColor;
        float4 _FogColor2;
		float4 _StripeColor;
		float4 _StripeColor2;
        float _FogMaxHeight;
        float _FogMinHeight;
		float _StripeHeight;
		float _StripeDensity;
		float _CelDensity;
  
        struct Input 
        {
            float2 uv_MainTex;
            float4 pos;
            float3 normalW;
        };
  
        void vert (inout appdata_full v, out Input o)
        {
        	UNITY_INITIALIZE_OUTPUT(Input,o);
            float4 hpos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.pos = mul(_Object2World, v.vertex);
            
            o.normalW = mul((float3x3)_Object2World, v.normal);
        }
  

        void surf (Input IN, inout SurfaceOutput o) {}

		void finalcolor (Input IN, SurfaceOutput o, inout fixed4 color)
        {

			_CelDensity *= (_FogMaxHeight-_FogMinHeight);
			float lerpValue = clamp((IN.pos.y - _FogMinHeight) / (_FogMaxHeight - _FogMinHeight), 0, 1);
			lerpValue = floor(lerpValue*_CelDensity)/_CelDensity;


			if ((IN.pos.y*_StripeDensity - floor(IN.pos.y*_StripeDensity)) < _StripeHeight){
				color.rgb = lerp (_StripeColor2.rgb, _StripeColor.rgb, lerpValue);
			}
			else {
				color.rgb = lerp (_FogColor2.rgb, _FogColor.rgb, lerpValue);
			}
		}
  
        ENDCG
    }
  
    FallBack "Diffuse"
}