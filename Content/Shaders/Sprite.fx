
float4x4 WVPMatrix;
texture SpriteTexture;

float4x4 SpriteMatrix[62];

sampler Sampler = sampler_state
{
    Texture = <SpriteTexture>;
	AddressU = Clamp;
	AddressV = Clamp;
};  

struct VS_QUAD_INPUT
{
	float2 TexCoord : TEXCOORD0;
	float Index : TEXCOORD1;
};

struct VS_Output
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

VS_Output VS_Function(VS_QUAD_INPUT quad)
{
	VS_Output output;

	output.Position = mul(float4((quad.TexCoord * SpriteMatrix[quad.Index]._m02_m03) + SpriteMatrix[quad.Index]._m00_m01, 0.0, 1.0), WVPMatrix);
	output.TexCoord = quad.TexCoord;
	output.Color = SpriteMatrix[quad.Index]._m10_m11_m12_m13;

    return output;
}

float4 PS_Function(VS_Output input) : COLOR0
{
	float4 color = input.Color;// *tex2D(Sampler, input.TexCoord);
    return color;
}

technique Technique0
{
    pass Pass0
    {
		VertexShader = compile vs_4_0_level_9_1 VS_Function();
		PixelShader = compile ps_4_0_level_9_1 PS_Function();
    }
}
