float4x4 WVPMatrix;
float4 Color;

struct VSOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VSOutput VertexShaderFunction(float4 Position : POSITION0)
{
    VSOutput output;

	output.TexCoord = float2(0.0, 0.0);
    output.Position = mul(Position, WVPMatrix);

    return output;
}

float4 PixelShaderFunction(float2 TexCoord : TEXCOORD0) : COLOR0
{
    return Color;
}

technique Technique0
{
    pass Pass0
    {
		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
