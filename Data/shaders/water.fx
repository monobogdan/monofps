
struct VSIn
{
	float4 pos : POSITION0;
	float4 normal : NORMAL0;
	float2 uv : TEXCOORD0;
};

struct PSIn
{
	float4 pos : POSITION0;
	float3 normal : TEXCOORD1;
	float3 refl : TEXCOORD2;
	float2 uv : TEXCOORD0;
};

float4x4 modelView;
float4x4 modelViewProj;
float4x4 modelViewInverse;

float time;

sampler2D primary; // Layer 1
samplerCUBE detail; // Layer 2
sampler2D detail2; 
sampler2D detail3; 

static const float ScaleFactor = 15.0f;
static const float ScrollFactor = 0.5f;

#define IOR 2.5

static const float R0Constant = ((1.0 - (1.0 / IOR))*(1.0 - (1.0 / IOR))) / ((1.0 + (1.0 / IOR))*(1.0 + (1.0 / IOR)));
static const float R0Inv = 1.0 - ((1.0 - (1.0 / IOR))*(1.0 - (1.0 / IOR))) / ((1.0 + (1.0 / IOR))*(1.0 + (1.0 / IOR)));

float fresnelApprox(float3 incident, float3 normal)
{
	return R0Constant + R0Inv * pow(1.0 - dot(incident, normal), 5.0);
}

float3 computeRefl(float3 normal, float4 camSpace)
{
	float3 eyeR = -normalize(camSpace.xyz);

	return 2 * dot(eyeR, normal) * normal - eyeR;
}

PSIn vsMain(VSIn vsIn)
{
	PSIn ret;

	float4 pos = mul(vsIn.pos, modelView);
	float3 normal = normalize(mul(vsIn.normal, (float3x3)modelView));
	
	ret.normal = normal;
	ret.uv = vsIn.uv;
	ret.refl = computeRefl(normal, pos);

	ret.pos = mul(vsIn.pos, modelViewProj);

	return ret;
}

float4 psMain(PSIn psIn) : COLOR0
{
	float2 uv = (psIn.uv * ScaleFactor) + (sin(time) * ScrollFactor);
	float4 l1 = tex2D(primary, uv);
	float4 reflCube = texCUBE(detail, psIn.refl);
	float4 shellac = fresnelApprox(float3(0, 0, -1), psIn.normal);

	float4 res = l1 * reflCube;
	res.a = 1;

	return res;
}

technique Main
{
	pass P0
	{
		VertexShader = compile vs_2_0 vsMain();
		PixelShader = compile ps_2_0 psMain();
	}
}