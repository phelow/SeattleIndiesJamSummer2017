// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

 Shader "FX/FlatShadedWaterWithEdgeBlend" {
Properties { 

	_BaseColor ("Base color", COLOR)  = ( .54, .95, .99, 0.5) 
	_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
	_InnerColor ("Foam Fade Color", Color) = (1,1,1,1) 
    _Shininess ("Shininess", Float) = 10
	_ShoreTex ("Shore & Foam texture ", 2D) = "black" {} 
	 
	_InvFadeParemeter ("Auto blend parameter (Edge, Shore, Distance scale)", Vector) = (0.2 ,0.39, 0.5, 1.0)

	_BumpTiling ("Foam Tiling", Vector) = (1.0 ,1.0, -2.0, 3.0)
	_BumpDirection ("Foam movement", Vector) = (1.0 ,1.0, -1.0, 1.0) 

	_Foam ("Foam (intensity, cutoff)", Vector) = (0.1, 0.375, 0.0, 0.0) 

	_WaveHeight ("Wave height", Float) = 2.0
	_WaveSpeed ("Wave speed", Float) = 1.0
	_WavePeriod ("Wave period", Float) = 50.0
	_RandomHeight ("Random wave height", Float) = 0.5
	_RandomSpeed ("Random wave speed", Float) = 0.5

	[MaterialToggle] _isInnerAlphaBlendOrColor("Fade inner to color or alpha?", Float) = 0 
}


CGINCLUDE 


	#include "UnityCG.cginc" 
	#include "UnityStandardCore.cginc"
	#include "UnityLightingCommon.cginc" // for _LightColor0


	sampler2D _ShoreTex;
	sampler2D_float _CameraDepthTexture;
  
	uniform float4 _BaseColor;  
    uniform float4 _InnerColor;
    uniform float _Shininess;
	 
	uniform float4 _InvFadeParemeter;
    
	uniform float4 _BumpTiling;
	uniform float4 _BumpDirection;
 
	uniform float4 _Foam; 
  	float _isInnerAlphaBlendOrColor; 

	float _WaveHeight;
	float _WaveSpeed;
	float _WavePeriod;
	float _RandomHeight;
	float _RandomSpeed;


	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
 
	
	struct v2f
	{
		float4 pos : SV_POSITION;
		float4 viewInterpolator : TEXCOORD0;
		float4 bumpCoords : TEXCOORD1;
		float4 screenPos : TEXCOORD2;
		float4 grabPassPos : TEXCOORD3; 
		half3 worldRefl : TEXCOORD4;
		float4 posWorld : TEXCOORD5;
		float4 posLocal : TEXCOORD6;
        float3 normalDir : TEXCOORD7;

		UNITY_FOG_COORDS(5)
	}; 
 
	float rand(float3 co)
	{
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
	}

	float rand2(float3 co)
	{
		return frac(sin(dot(co.xyz, float3(19.9128, 75.2, 34.5122))) * 12765.5213);
	}
 

	inline half4 Foam(sampler2D shoreTex, half4 coords) 
	{
		half4 foam = (tex2D(shoreTex, coords.xy) * tex2D(shoreTex,coords.zw)) - 0.125;
		return foam;
	}

	v2f vert(appdata_full v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		
		float4 worldSpaceVertex = mul(unity_ObjectToWorld,(v.vertex));
		half3 offsets = half3(0,0,0);

		float phase0 = (_WaveHeight)* sin((_Time[1] * _WaveSpeed) + (worldSpaceVertex.x + worldSpaceVertex.y) / _WavePeriod + rand2(worldSpaceVertex.xyy));
		float phase0_1 = (_RandomHeight) * sin(cos(rand(worldSpaceVertex.xyy) * _RandomHeight * cos(_Time[1] * _RandomSpeed * sin(rand(worldSpaceVertex.xxy)))));
		float phase0_2 = (_WaveHeight / 5.0) * sin((_Time[1] * _WaveSpeed * 3.0) + (worldSpaceVertex.x + worldSpaceVertex.y) / (_WavePeriod * 4.0) + rand2(worldSpaceVertex.xyy));

		offsets.z = phase0 + phase0_1 + phase0_2;
		worldSpaceVertex.z += offsets.z;
		v.vertex = mul(unity_WorldToObject, worldSpaceVertex);

		half3 vtxForAni = (worldSpaceVertex).xyy;
		half2 tileableUv = mul(unity_ObjectToWorld,(v.vertex)).xy;
		
		o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;

		o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.screenPos=ComputeScreenPos(o.pos); 
		o.viewInterpolator.w = saturate(offsets.y);
		
		UNITY_TRANSFER_FOG(o,UnityObjectToClipPos(v.vertex));
 		half3 worldNormal = UnityObjectToWorldNormal(v.normal); 
   		float4x4 modelMatrix = unity_ObjectToWorld;
        float4x4 modelMatrixInverse = unity_WorldToObject; 
	 	o.posLocal = v.vertex;
	 	o.posWorld = mul(modelMatrix, v.vertex);
        o.normalDir = normalize( mul(float4(v.normal, 0.0), modelMatrixInverse).xyz); 

        //float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        //float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos)); 
        //o.worldRefl = reflect(-worldViewDir, worldNormal);

		return o;
	}


	[maxvertexcount(3)]
	void geom(triangle v2f IN[3], inout TriangleStream<v2f> triStream)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);

		float3 v0 = IN[0].posLocal.xyz;
		float3 v1 = IN[1].posLocal.xyz;
		float3 v2 = IN[2].posLocal.xyz;

		float3 centerPos = (v0 + v1 + v2) / 3.0;
		float3 vn = normalize(cross(v1 - v0, v2 - v0));

		float4x4 modelMatrix = unity_ObjectToWorld;
		float4x4 modelMatrixInverse = unity_WorldToObject;

		float3 normalDirection = normalize(mul(float4(vn, 0.0), modelMatrixInverse).xyz);
		float3 viewDirection = normalize(_WorldSpaceCameraPos - mul(modelMatrix, float4(centerPos, 0.0)).xyz);
		half3 worldRefl = reflect(-viewDirection, normalDirection);

		o.pos = IN[0].pos;
		o.viewInterpolator = IN[0].viewInterpolator;
		o.bumpCoords = IN[0].bumpCoords;
		o.screenPos = IN[0].screenPos;
		o.grabPassPos = IN[0].grabPassPos; 
        o.worldRefl = worldRefl;
		o.posWorld = IN[0].posWorld;
        o.normalDir = normalDirection; 
		TRANSFER_SHADOW(o);
		UNITY_TRANSFER_FOG(o, o.pos);
		triStream.Append(o);

		o.pos = IN[1].pos;
		o.viewInterpolator = IN[1].viewInterpolator;
		o.bumpCoords = IN[1].bumpCoords;
		o.screenPos = IN[1].screenPos;
		o.grabPassPos = IN[1].grabPassPos; 
        o.worldRefl = worldRefl;
		o.posWorld = IN[1].posWorld;
        o.normalDir = normalDirection;
		TRANSFER_SHADOW(o);
		UNITY_TRANSFER_FOG(o, o.pos);
		triStream.Append(o);

		o.pos = IN[2].pos;
		o.viewInterpolator = IN[2].viewInterpolator;
		o.bumpCoords = IN[2].bumpCoords;
		o.screenPos = IN[2].screenPos;
		o.grabPassPos = IN[2].grabPassPos; 
        o.worldRefl = worldRefl;
		o.posWorld = IN[2].posWorld;
        o.normalDir = normalDirection;
		TRANSFER_SHADOW(o);
		UNITY_TRANSFER_FOG(o, o.pos);
		triStream.Append(o);
	}

 
	 half4 calculateBaseColor(v2f input)  
         {
            float3 normalDirection = normalize(input.normalDir);
 
            float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float3 ambientLighting = 
               UNITY_LIGHTMODEL_AMBIENT.rgb * _BaseColor.rgb;
 
            float3 diffuseReflection = 
               attenuation * _LightColor0.rgb * _BaseColor.rgb
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else  
            {
               specularReflection = attenuation * _LightColor0.rgb  * _SpecColor.rgb * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
            }

            return half4(ambientLighting + diffuseReflection  + specularReflection, 1.0);
         }

	half4 frag( v2f i ) : SV_Target
	{ 
 
		half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
		
		#ifdef WATER_EDGEBLEND_ON
			half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
			depth = LinearEyeDepth(depth);
			edgeBlendFactors = saturate(_InvFadeParemeter * (depth-i.screenPos.w));
			edgeBlendFactors.y = 1.0-edgeBlendFactors.y;
		#endif
		
 
        half4 baseColor = calculateBaseColor(i);
       
 
		half4 foam = Foam(_ShoreTex, i.bumpCoords * 2.0);
		baseColor.rgb += foam.rgb * _Foam.x * (edgeBlendFactors.y + saturate(i.viewInterpolator.w - _Foam.y));
		if( _isInnerAlphaBlendOrColor==0)
			baseColor.rgb += _InnerColor * (1.0f - edgeBlendFactors.x);
		if(  _isInnerAlphaBlendOrColor==1.0)
			baseColor.a  =  edgeBlendFactors.x;
		UNITY_APPLY_FOG(i.fogCoord, baseColor);
		return baseColor;
	}
	
ENDCG

Subshader
{
	Tags {"RenderType"="Transparent" "Queue"="Transparent"}
	
	Lod 500
	ColorMask RGB
	
	GrabPass { "_RefractionTex" }
	
	Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Cull Off
		
			CGPROGRAM
		
			#pragma target 4.0
		
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#pragma multi_compile_fog
		
			#pragma multi_compile WATER_EDGEBLEND_ON WATER_EDGEBLEND_OFF 
		
			ENDCG
	}
}


Fallback "Transparent/Diffuse"
}
