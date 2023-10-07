
struct VSIn
{
	float4 pos : POSITION0;
	float4 normal : NORMAL0;
	float2 uv : TEXCOORD0;
};

struct PSIn
{
	float4 pos : POSITION0;
	float4 normal : TEXCOORD1;
	float2 uv : TEXCOORD0;
};

float4x4 modelView;
float4x4 modelViewProj;
float4x4 modelViewInverse;

float time;

sampler2D primary;
sampler2D detail;
sampler2D detail2;
sampler2D detail3;

PSIn vsMain(VSIn vsIn)
{
	PSIn ret;

	ret.pos = mul(vsIn.pos, modelViewProj);
	ret.normal = vsIn.normal;
	ret.uv = vsIn.uv;

	return ret;
}

float4 psMain(PSIn psIn) : COLOR0
{
	float4 tex = tex2D(primary, psIn.uv);

	return tex;
}

technique Main
{
	pass P0
	{
		VertexShader = compile vs_2_0 vsMain();
		PixelShader = compile ps_2_0 psMain();
	}
}