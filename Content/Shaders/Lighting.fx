float4x4 World;
float4x4 View;
float4x4 Projection;
uniform texture LightMap;
uniform texture Scene;
uniform float4 AmbientColour;

// TODO: add effect parameters here.

sampler LightMapSampler = sampler_state { texture = <LightMap>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};
sampler SceneSampler = sampler_state { texture = <Scene>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};

float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{ 
	float4 colour = tex2D(SceneSampler, texCoord);
	

	if(colour.w!=0)	colour*=(AmbientColour+(tex2D(LightMapSampler, texCoord)));
	
	return colour +tex2D(LightMapSampler, texCoord)*0.1f ;
}

technique Technique1
{
    pass Pass1
    {
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
