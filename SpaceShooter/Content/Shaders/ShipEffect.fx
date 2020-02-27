float4x4 World;
float4x4 View;
float4x4 Projection;

float4 CelOutlineColor : COLOR = float4(0.0f, 0.0f, 0.0f, 1.0f);
float CelOutlineWidth = 0.03;

float3 TargetColor : COLOR = float3(1.0f, 1.0f, 1.0f);

texture DiffuseTexture;
sampler Diffuse = sampler_state
{
   Texture = (DiffuseTexture);
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

texture SpecularTexture;
sampler Specular = sampler_state
{
   Texture = (SpecularTexture);
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

texture NormalTexture;
sampler NormalMap = sampler_state
{
   Texture = (NormalTexture);
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

texture EmissiveTexture;
sampler EmissiveMap = sampler_state
{
   Texture = (EmissiveTexture);
   MAGFILTER = LINEAR;
   MINFILTER = LINEAR;
   MIPFILTER = LINEAR;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
	float2 TexCoord : TEXCOORD0;
	float3 Tangent : TANGENT;
    float3 Binormal : BINORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
    float3x3 tangentToWorld : TEXCOORD3;
};

float3 DirectionalLight;
float4 DirectionalLightColor : COLOR;

float specularPower;
float specularIntensity;

float3 cameraPosition;

float DiffuseLight(float3 Normal, float3 PointDirection)
{
	return saturate(dot(Normal, PointDirection));
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    output.WorldPosition = worldPosition;
    
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.WorldNormal = normalize(mul(input.Normal, World));
	output.TexCoord = input.TexCoord;

    // calculate tangent space to world space matrix using the world space tangent,
    // binormal, and normal as basis vectors.  the pixel shader will normalize these
    // in case the world matrix has scaling.
    output.tangentToWorld[0] = mul(input.Tangent, World);
    output.tangentToWorld[1] = mul(input.Binormal, World);
    output.tangentToWorld[2] = mul(input.Normal, World);
    
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//Direction of light
	float3 lightDir;
	lightDir = normalize(DirectionalLight);
	
	//Diffuse light with bump
	float3 localNormal = tex2D(Diffuse, input.TexCoord) * 2.0 - 1.0f;
	
	float3 bumpNormal;
	bumpNormal.y = -dot(input.tangentToWorld[0], lightDir);
	bumpNormal.x = dot(input.tangentToWorld[1], lightDir);
	bumpNormal.z = dot(input.tangentToWorld[2], lightDir);
	float DiffuseDot = DiffuseLight(localNormal, bumpNormal);
	float4 DiffuseColor = DirectionalLightColor * DiffuseDot;
	
	/*
	//Specular component
    float3 reflectionVector = normalize(reflect(lightDir, input.WorldNormal));
    float3 directionToCamera = normalize(cameraPosition - input.WorldPosition);
    float4 specular = DirectionalLightColor * specularIntensity * 
                      pow( saturate(dot(reflectionVector, directionToCamera)), 
                      specularPower);    
*/
	
	//apply palette shift.
	float4 diffTexture = tex2D(Diffuse, input.TexCoord);
    float3 color = lerp(DiffuseColor.rgb, TargetColor, diffTexture.a);
    float4 finalColor = float4(color, 1);
    DiffuseColor += finalColor;
    
    
    //Combine colors   
    float4 output = tex2D(Diffuse, input.TexCoord) * DiffuseColor;
    //output += specular * tex2D(Specular, input.TexCoord);
    //output += tex2D(EmissiveMap, input.TexCoord);
  
    output.a = 1;
    
    return output;
 
}

struct VS_INPUT
{
	float4 Position		: POSITION0;
	float3 Normal		: NORMAL0;
	float2 Texcoord		: TEXCOORD0;

};

//Output for the outline vertex shader
struct VS_OUTPUT2
{
	float4 Position			: POSITION0;
	float4 Normal			: TEXCOORD1;
};

VS_OUTPUT2 Outline(VS_INPUT Input)
{
	//Here's the important value. It determins the thickness of the outline.
	//The value is completely dependent on the size of the model.
	//My model is very tiny so my outine is very tiny.
	//You may need to increase this or better yet, caluclat it based on the distance
	//between your camera and your model.
	float offset = CelOutlineWidth;
	
	float4x4 WorldViewProjection = mul(mul(World, View), Projection);
	
	VS_OUTPUT2 Output;
	Output.Normal			= mul(Input.Normal, World);
	Output.Position			= mul(Input.Position, WorldViewProjection)+(mul(offset, mul(Input.Normal, WorldViewProjection)));
	
	return Output;
}

//This is the ouline pixel shader. It just outputs unlit black.
float4 Black() : COLOR
{
   return CelOutlineColor;
}

technique Technique1
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
        CullMode = CCW;
    }
    
	pass P1
	{
		VertexShader = compile vs_3_0 Outline();
		PixelShader  = compile ps_3_0 Black();
		CullMode = CW;
	}    
}
