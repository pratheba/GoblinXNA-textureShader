struct VertexToPixel
{
	float4 Position		: POSITION0;
	float4 worldPosition: TEXCOORD0;
	float3 worldNormal	: TEXCOORD1;
	float4 Color		: COLOR;
	float LightingFactor : TEXCOORD2;
};

struct PixelToFrame
{
	float4 Color			: COLOR;
};

struct Light
{
	float4 color;
    float3 position;
    float3 direction;
    int type; // 0: directional; 1: point; 2: spot;
    
};

Light light;
int numberOfLights;

// shared scene parameters
shared float4x4 viewProjection;
shared float3 cameraPosition;
shared float4 ambientLightColor;
shared float4 diffuseColor;

// World Parameters
float4x4 world;
float4x4 worldForNormal;

bool EnableLighting;

VertexToPixel ColoredVS( float3 position : POSITION, float3 normal :  NORMAL,
     float4 color : COLOR)
{
    VertexToPixel output = (VertexToPixel)0;
	float4x4 wvp = mul(world, viewProjection);
	output.Position = mul(float4(position, 1.0), wvp);
	float4 worldPosition = mul(float4(position, 1.0),world);
	output.worldPosition = worldPosition / worldPosition.w;
	output.Color = color;

	float3 Normal = normalize(mul(normalize(normal), worldForNormal));
	output.worldNormal = Normal;
	output.LightingFactor = 1;
	if(EnableLighting)
		output.LightingFactor = dot( -light.direction,Normal);
    return output;
}

float4 ColoredPS(VertexToPixel input) : COLOR
{
   
	float4 color = input.Color;
	color.rgb *= saturate(input.LightingFactor) + ambientLightColor ;
	
    return color;
}

technique TerrainColored
{
    pass terrain
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 ColoredVS();
        PixelShader = compile ps_2_0 ColoredPS();
    }
}


/// Textured Output ///

Texture grassTexture;

sampler grass_sampler = sampler_state
{
	Texture = <grassTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};


struct TexturedVertexToPixel
{
	float4 Position : POSITION;
	float3 worldPosition : TEXCOORD1;
	float3 worldNormal : TEXCOORD2;
	float LightingFactor : TEXCOORD3;
	float2 TexCoord : TEXCOORD0;
};

struct TexturedPixelToFrame
{
	float4 Color : COLOR;
};


TexturedVertexToPixel TexturedVertexShader(float3 inPos : POSITION, float4 inNormal : NORMAL, float2 inTexCoord : TEXCOORD0)
{
	TexturedVertexToPixel output = (TexturedVertexToPixel)0;
	float4x4 wvp = mul(world, viewProjection);

	output.Position = mul(float4(inPos, 1.0), wvp);
	float4 worldPosition = mul(float4(inPos, 1.0),world);
	output.worldPosition = worldPosition / worldPosition.w;
	output.TexCoord = inTexCoord;

	float3 normal = normalize(mul(normalize(inNormal), worldForNormal));
	output.worldNormal = normal;

	output.LightingFactor = 1.0f;
	if(EnableLighting)
	{
		output.LightingFactor = saturate (dot (normal, -light.direction));
	}

	return output;
}

TexturedPixelToFrame TexturedPixelShader(TexturedVertexToPixel PSIn )
{
	TexturedPixelToFrame output = (TexturedPixelToFrame)0;

	output.Color = tex2D (grass_sampler,PSIn.TexCoord );
	output.Color.rgb *= saturate (PSIn.LightingFactor + ambientLightColor );
	
	return output;
}


technique Textured
{
	pass grass
	{
		VertexShader = compile vs_2_0 TexturedVertexShader();
		PixelShader = compile ps_2_0 TexturedPixelShader();
	}
}


Texture sandTexture;

sampler sand_sampler = sampler_state
{
	texture = <sandTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

Texture rockTexture;

sampler rock_sampler = sampler_state
{
	texture = <rockTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

Texture snowTexture;

sampler snow_sampler = sampler_state
{
	texture = <snowTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};



struct MultiTextureVertexToPixel
{
	float4 Position : POSITION;
	float3 worldPosition : TEXCOORD0;
	float3 worldNormal : TEXCOORD1;
	float2 TextureCoordinate : TEXCOORD2;
	float4 TextureWeight : TEXCOORD3;
	float LightingFactor : TEXCOORD4;
	float depth : TEXCOORD5; // Meant for Near and Far Viewing with Pixel Clamping according to Distance
};

struct MultiTexturePixelToFrame
{
	float4 color : COLOR0;
};

MultiTextureVertexToPixel MultiTexturedVertexShader(float3 inPos : POSITION, float3 inNormal :NORMAL, 
						float2 inTexCoord : TEXCOORD0, float4 inTexWeight : TEXCOORD1)
{
	MultiTextureVertexToPixel output = (MultiTextureVertexToPixel)0;

	float4x4 wvp = mul(world, viewProjection);
	output.Position = mul(float4(inPos, 1.0), wvp);
	float4 worldPosition = mul(float4(inPos, 1.0), world);
	output.worldPosition = worldPosition / worldPosition.w;
	
	float3 normal = normalize(mul(normalize(inNormal), worldForNormal));
	output.worldNormal = normal;

	output.TextureCoordinate = inTexCoord;
	output.TextureWeight = inTexWeight;

	// Depth is th Z Co-rdinate. The distance between the camera and the Object in Camera Space
	output.depth = output.Position.z / output.Position.w;

	output.LightingFactor = 1.0;
	if(EnableLighting)
	{
		float lightingFactor = saturate(dot(normal, -light.direction));
		output.LightingFactor = lightingFactor;
	}
	
	return output;
}


MultiTexturePixelToFrame MultiTexturedPixelShader(MultiTextureVertexToPixel PSIn)
{
	MultiTexturePixelToFrame output = (MultiTexturePixelToFrame)0;

	float blendingDistance = 0.99;
	float blendingWidth = 0.005;

	// Clamping between near and far clipping plane (0 and 1)
	float blendingFactor = clamp ( (PSIn.depth - blendingDistance) / blendingWidth, 0, 1);

	float4 colorFarCamera = tex2D(sand_sampler, PSIn.TextureCoordinate) * PSIn.TextureWeight.x;
	colorFarCamera += tex2D(grass_sampler, PSIn.TextureCoordinate) * PSIn.TextureWeight.y;
	colorFarCamera += tex2D(rock_sampler, PSIn.TextureCoordinate) * PSIn.TextureWeight.z;
	colorFarCamera += tex2D(snow_sampler, PSIn.TextureCoordinate) * PSIn.TextureWeight.w;

	// Increase the Coordinate width
	float2 NearTexCoord = PSIn.TextureCoordinate * 3;
	float4 colorNearCamera = tex2D(sand_sampler, NearTexCoord) * PSIn.TextureWeight.x;
	colorNearCamera += tex2D(grass_sampler, NearTexCoord) * PSIn.TextureWeight.y;
	colorNearCamera += tex2D(rock_sampler, NearTexCoord) * PSIn.TextureWeight.z;
	colorNearCamera += tex2D(snow_sampler, NearTexCoord) * PSIn.TextureWeight.w;

	output.color = lerp (colorNearCamera, colorFarCamera, blendingFactor);
	output.color.rgb *= saturate(PSIn.LightingFactor + ambientLightColor);

	return output;
}

technique MultiTextured
{
	pass multitexturedpass
	{
		VertexShader = compile vs_2_0 MultiTexturedVertexShader();
		PixelShader = compile ps_2_0 MultiTexturedPixelShader();
	}
}