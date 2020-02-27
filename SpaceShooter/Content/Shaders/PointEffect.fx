float4x4 WorldViewProj;

float4 VSBasicNoFog(
    float4 Position : SV_Position,
    inout float4 DiffuseColor : COLOR0,
    inout float PointSize : PSIZE0
) : SV_Position {
    return mul(Position, WorldViewProj);
}

float4 PSBasicNoFog(float4 Diffuse : COLOR0) : SV_Target0
{
    return Diffuse;
}

Technique PointEffect
{
    Pass
    {
        VertexShader = compile vs_3_0 VSBasicNoFog();
        PixelShader  = compile ps_3_0 PSBasicNoFog();
    }
}
