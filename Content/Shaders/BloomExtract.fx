// Pixel shader extracts the brighter areas of an image.
// This is the first step in applying a bloom postprocess.

sampler TextureSampler : register(s0);

float BloomThreshold;


float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    // Look up the original image color.
    float4 c = tex2D(TextureSampler, texCoord);

    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass Pass1
    {
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
