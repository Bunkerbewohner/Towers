//-----------------------------------------------------------------------------
// InstancedModel.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Camera settings.
float4x4 World;
float4x4 View;
float4x4 Projection;


// This sample uses a simple Lambert lighting model.
float3 LightDirection = normalize(float3(-1, -1, -1));
float3 DiffuseLight = 0.8;
float3 AmbientColor = 0.6;

texture Texture;

sampler Sampler = sampler_state
{
    Texture = (Texture);
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
	float4x4 Transform : BLENDWEIGHT;
	float3 Color : COLOR0;
	float3 Size : TEXCOORD0;
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;    
};


// Vertex shader helper function shared between the two techniques.
VertexShaderOutput VertexShaderCommon(VertexShaderInput input)
{
    VertexShaderOutput output;

	input.Position *= float4(input.Size, 1);

    // Apply the world and camera matrices to compute the output position.
    float4 worldPosition = mul(input.Position, input.Transform);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // Compute lighting, using a simple Lambert model.
    float3 worldNormal = mul(input.Normal, input.Transform);
    
    float diffuseAmount = max(-dot(worldNormal, LightDirection), 0);
    
    float3 lightingResult = saturate(diffuseAmount * DiffuseLight * input.Color + AmbientColor * input.Color);
    
    output.Color = float4(lightingResult, 1);    

    return output;
}


// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
VertexShaderOutput HardwareInstancingVertexShader(VertexShaderInput input)
{
    return VertexShaderCommon(input);
}

// Both techniques share this same pixel shader.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}


// Hardware instancing technique.
technique HardwareInstancing
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 HardwareInstancingVertexShader();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
