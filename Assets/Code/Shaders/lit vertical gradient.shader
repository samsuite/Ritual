// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Custom/Lit Vertical Gradient" {
   Properties {
		_LightColor ("Light Color", Color) = (1, 1, 1, 0.5)
		_Color1 ("Color 1", Color) = (0.5, 0.5, 0.5, 1)
        _Color2 ("Color 2", Color) = (0.5, 0.5, 0.5, 1)
		_MaxHeight ("Max Height", Float) = 0.0
        _MinHeight ("Min Height", Float) = 0.0
   }
   SubShader {
      Pass {
		 Tags { "LightMode" = "ForwardBase" "RenderType" = "Opaque" }
		 Cull Off

         CGPROGRAM

         #pragma vertex vert  
         #pragma fragment frag

		 float4 _LightColor;
		 float4 _Color1;
         float4 _Color2;
		 float _MaxHeight;
         float _MinHeight;


         struct vertexInput {
            float4 vertex : POSITION;
			float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 position_in_world_space : TEXCOORD0;
			float4 col : COLOR;
         };

         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output; 

			// very simple directional lighting
			float4x4 modelMatrix = _Object2World;
            float4x4 modelMatrixInverse = _World2Object;
 
            float3 normalDirection = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float3 diffuseReflection = _LightColor.rgb * max(0.0, dot(normalDirection, lightDirection));
 
            output.col = float4(diffuseReflection, 1.0);


			// position
            output.pos =  mul(UNITY_MATRIX_MVP, input.vertex);
            output.position_in_world_space = mul(_Object2World, input.vertex);

            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR 
         {
            float height = input.position_in_world_space.y;

			float lerpValue = clamp((height - _MinHeight) / (_MaxHeight - _MinHeight), 0, 1);
			return float4(lerp (_Color1.rgb, _Color2.rgb, lerpValue), 1.0) + input.col;
         }
 
         ENDCG  
      }
   }
}