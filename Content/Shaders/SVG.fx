float4x4 WVPMatrix;

float4 Color;

struct VS_Input
{
    float2 Position : POSITION0;
	float SplineType : NORMAL0;
	float2 SplineInterpolation : NORMAL1;
};

struct VS_Output
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
	float SplineType : TEXCOORD0;
	float2 SplineInterpolation : TEXCOORD1;
};

VS_Output VertexShaderFunction(VS_Input input)
{
    VS_Output output;

    output.Position = mul(float4(input.Position, 0.0, 1.0), WVPMatrix);
	output.Color = Color;
	output.SplineType = input.SplineType;
	output.SplineInterpolation = input.SplineInterpolation;

    return output;
}

float4 PixelShaderFunction(VS_Output input) : COLOR0
{
	float4 color = 0.0;

	if (input.SplineType == 0.0)
	{
		color = input.Color;
	}
	else if (input.SplineType == 1.0)
	{
		if ((input.SplineInterpolation.x * input.SplineInterpolation.x) - input.SplineInterpolation.y < 0.0) color = input.Color;
	}
    else if (input.SplineType == 2.0)
	{
		if ((input.SplineInterpolation.x * input.SplineInterpolation.x) - input.SplineInterpolation.y > 0.0) color = input.Color;
	}

	return color;
}

technique Technique1
{
    pass Pass1
    {
		VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
