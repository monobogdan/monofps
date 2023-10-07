
struct VSIn
{
	float4 pos : POSITION0;
	float4 normal : NORMAL0;
	float2 uv : TEXCOORD0;
};

struct PSIn
{
	float4 pos : POSITION0;
	float3 tpos : TEXCOORD2;
	float3 normal : TEXCOORD1;
	float2 uv : TEXCOORD0;
};

float time;

float4x4 modelView;
float4x4 modelViewProj;
float4x4 modelViewInverse;

sampler2D primary; // Splatmap
sampler2D detail; // Tex 1
sampler2D detail2; // Tex 2
sampler2D detail3; // Tex 3

#define LIGHT_DIR float4(0.5f, 0.2f, 0.3f, 0)

const float ScaleFactor = 15.0f;

PSIn vsMain(VSIn vsIn)
{
	PSIn ret;

	float4 pos = mul(vsIn.pos, modelView);
	float3 normal = normalize(mul(vsIn.normal, (float3x3)modelView));

	ret.pos = mul(vsIn.pos, modelViewProj);
	ret.normal = normal;
	ret.tpos = pos.xyz;
	ret.uv = vsIn.uv;

	return ret;
}

float4 calcLight(PSIn psIn)
{
	float3 l = LIGHT_DIR.xyz - psIn.tpos;
	float3 e = normalize(-psIn.tpos);
	float3 r = normalize(-reflect(l, psIn.normal));

	float3 light = max(dot(psIn.normal, l), 0);
	float3 spec = pow(max(dot(r, e), 0), 64);

	light = clamp(light, 0, 1);
	spec = clamp(spec, 0, 1);

	return float4(light, 1);
}

float4 psMain(PSIn psIn) : COLOR0
{
	float4 splat = tex2D(primary, psIn.uv);
	float4 res = float4(1, 1, 1, 1);
	float4 t1 = tex2D(detail, psIn.uv * ScaleFactor);
	float4 t2 = tex2D(detail2, psIn.uv * ScaleFactor);
	float4 t3 = tex2D(detail3, psIn.uv * ScaleFactor);

	res = lerp(res, t1, splat.r);
	res = lerp(res, t2, splat.g);
	res = lerp(res, t3, splat.b);
	res.a = 1;

	return calcLight(psIn) * res;
}

technique Main
{
	pass P0
	{
		VertexShader = compile vs_2_0 vsMain();
		PixelShader = compile ps_2_0 psMain();
	}
}