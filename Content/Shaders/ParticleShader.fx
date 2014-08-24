
float4x4 WVPMatrix;
texture SpriteTexture;
bool IsTextured;

float4 Particle[100];
float ParticleGen[100];

sampler cake = sampler_state{ texture = < SpriteTexture > ;	AddressU = Clamp;	AddressV = Clamp; };

float2 PositionOffset;

float4 ColorStart;
float4 ColorEnd;

float2 Acceleration;

float Life;
float Time;
float Size;

struct VS_QUAD_INPUT
{
	float2 TexCoord : TEXCOORD0;
	float Count : TEXCOORD1;
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

	float ageF = Time - ParticleGen[quad.Count];
	float tmp = ageF / Life;

	output.Color = (tmp * (ColorEnd - ColorStart)) + ColorStart;

	float Age = ageF / 1000;

	float2 tmpPosition = (0.5f * Acceleration * (Age * Age)) + (Particle[quad.Count].zw * Age) + Particle[quad.Count].xy;

		///										///
		/// Rotation to align with rectangle	///
		///										///

		float alpha = 0;
	if (Particle[quad.Count].z > 0)
	{
		alpha = asin(-Particle[quad.Count].w / length(Particle[quad.Count].zw));
	}
	else
	{
		alpha = acos(Particle[quad.Count].z / length(Particle[quad.Count].zw));
		if (-Particle[quad.Count].w <= 0) alpha *= -1.0;
	}
	float2x2 rotationMat = { cos(alpha), -sin(alpha), sin(alpha), cos(alpha) };
		float4 outputpos = float4((mul(quad.TexCoord - 0.5, rotationMat) * Size) + tmpPosition + PositionOffset, 0.0, 1.0);

		output.Position = mul(outputpos, WVPMatrix);

	output.TexCoord = quad.TexCoord;

	if (tmp > 1 || ParticleGen[quad.Count] == 0)
	{
		output.Position = float4(0.0, 0.0, 0.0, 0.0);
	}
	return output;
}

float4 PS_Function(VS_Output input) : COLOR0
{
	float4 color = input.Color;
	float2 texcrd = input.TexCoord;
	if (IsTextured)
	{
		color *= tex2D(cake, texcrd);
	}
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
