Shader "Effects/WeaponFX/GlowCutoutGradient" {
	Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,1)
	_TimeScale ("Time Scale", Vector) = (1,1,1,1) 
	_MainTex ("Noise Texture", 2D) = "white" {}
	_BorderScale ("Border Scale (XY) Offset (Z)", Vector) = (0.5,0.05,1,1) 
}

	SubShader {
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend SrcAlpha One
			Cull Off 
			Lighting Off 
			ZWrite Off	
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _TintColor;
			float4 _TimeScale;
			float4 _BorderScale;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				v.vertex.xyz += v.normal/100 * _BorderScale.z;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D _CameraDepthTexture;
			
			half4 frag (v2f i) : COLOR
			{	
				half4 mask = tex2D(_MainTex, i.texcoord + _Time.xx * _TimeScale.xy);
				half4 tex = tex2D(_MainTex, i.texcoord  + _Time.xx * _TimeScale.zw);
				half alphaMask = tex2D(_MainTex, i.texcoord * 2).b;
				//tex *= mask;
				float4 res = 0;
				//res.r = tex.r * step(tex.r, _BorderScale.x);
				//res.r -= tex.r * step(tex.r, _BorderScale.x - _BorderScale.y);
				//res.g = tex.r * step(tex.g, _BorderScale.x);
				//res.g -= step(tex.r, _BorderScale.x - _BorderScale.y);
				//res.r *= tex.g;
				res.r = tex.r * mask.r;
				res = i.color * _TintColor * lerp(0, res.r, alphaMask);
				res.rgb = pow(res.rgb, _BorderScale.w);
				res.a = saturate(res.a * 4);
				return  res;
			}
			ENDCG 
		}

		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			Blend DstColor Zero
			Cull Off 
			Lighting Off 
			ZWrite Off	
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _TintColor;
			float4 _TimeScale;
			float4 _BorderScale;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				v.vertex.xyz += v.normal/100 * _BorderScale.z;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			sampler2D _CameraDepthTexture;
			
			half4 frag (v2f i) : COLOR
			{	
				half4 mask = tex2D(_MainTex, i.texcoord + _Time.xx * _TimeScale.xy);
				half4 tex = tex2D(_MainTex, i.texcoord  + _Time.xx * _TimeScale.zw);
				half alphaMask = tex2D(_MainTex, i.texcoord * 2).b;
				//tex *= mask;
				float4 res = 0;
				//res.r = tex.r * step(tex.r, _BorderScale.x);
				//res.r -= tex.r * step(tex.r, _BorderScale.x - _BorderScale.y);
				//res.g = tex.r * step(tex.g, _BorderScale.x);
				//res.g -= step(tex.r, _BorderScale.x - _BorderScale.y);
				//res.r *= tex.g;
				res.r = tex.r * mask.r;
				res = i.color * _TintColor * lerp(0, res.r, alphaMask);
				res.rgb *= res.rgb;
				res.a = saturate(res.a * 4);
				return  0.5 + saturate(res)*0.5;
			}
			ENDCG 
		}
	}	


}