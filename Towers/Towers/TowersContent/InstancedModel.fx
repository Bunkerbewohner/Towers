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

texture DiffuseMap;
texture NormalMap;

sampler DiffuseMapSampler = sampler_state
{
    Texture = (DiffuseMap);
};

sampler NormalMapSampler = sampler_state
{
	Texture = (NormalMap);
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
	float3 Tangent : TANGENT0;
	float4x4 Transform : BLENDWEIGHT;
	float3 Color : COLOR0;
	float3 Size : TEXCOORD1;
};


struct VertexShaderOutput
{
	float2 TexCoord : TEXCOORD0;
    float4 Position : POSITION0;
	float4 PositionScreen : TEXCOORD2; // copy of position since POS0 cannot be access in pixel shader
    float4 Color : COLOR0;    	
	float3 LightDirT : TEXCOORD1;
	float4x4 Transform : BLENDWEIGHT;
};

struct PixelShaderOutput
{
	float4 Color : COLOR0;
	float4 Normal : COLOR1;
	float4 Depth : COLOR2;
}

// Vertex shader helper function shared between the two techniques.
VertexShaderOutput VertexShaderCommon(VertexShaderInput input)
{
    VertexShaderOutput output;

	input.Position *= float4(input.Size, 1);

    // Apply the world and camera matrices to compute the output position.
    float4 worldPosition = mul(input.Position, input.Transform);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.PositionScreen = output.Position;

	// Apply normal map
	float3 Binormal = cross(input.Tangent, input.Normal);
	float3x3 tangentToObject;
	tangentToObject[0] = normalize(Binormal);
	tangentToObject[1] = normalize(input.Tangent);
	tangentToObject[2] = normalize(input.Normal);
	float3x3 tangentToWorld = mul(tangentToObject, World);

	output.LightDirT = mul(tangentToWorld, LightDirection);       
	output.TexCoord = input.TexCoord;
	output.Transform = input.Transform;
	output.Color = float4(input.Color, 1);	

    return output;
}


// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
VertexShaderOutput HardwareInstancingVertexShader(VertexShaderInput input)
{
    return VertexShaderCommon(input);
}

// Both techniques share this same pixel shader.
PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	float4 bump = tex2D(NormalMapSampler, input.TexCoord);
	float3 normal = normalize((bump.xyz - 0.5f) * 2.0f);

	// Compute lighting, using a simple Lambert model.
    float3 worldNormal = mul(normal, input.Transform);
    
    float diffuseAmount = max(-dot(worldNormal, input.LightDirT), 0);
    
	input.Color *= tex2D(DiffuseMapSampler, input.TexCoord);

    float3 lightingResult = saturate(diffuseAmount * DiffuseLight * input.Color + AmbientColor * input.Color);
    
    float4 color = float4(lightingResult, 1); 

	output.Color = color;

    return output;
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
