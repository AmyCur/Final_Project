float3 Unity_Remap(float3 In, float2 InMinMax, float2 OutMinMax)
{
    return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void ConvertToCelShader_float3(float3 lightDirection, float3 normal, Texture2D tex, out float3 final){
    float3 negated=-1*lightDirection;
    float value=dot(negated, normal);
    value=Unity_Remap(value, float2(-1,1), float2(0,1));
    float a = step(1, value) + step(-1,value) + step(0.5,value) + step(-0.5,value) + step(0.25,value) + step(-0.25,value) + step(0,value);
    a=a/9;
    float4 sampledTexture = SAMPLE_TEXTURE2D(tex,UV0);
    sampledTexture*=a;
    final=sampledTexture;
}

