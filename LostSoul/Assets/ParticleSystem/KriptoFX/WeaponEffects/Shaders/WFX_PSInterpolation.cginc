
inline float4 Tex2DInterpolated(sampler2D Tex, float2 TexCoord, float4 _Tiling)
{
	float2 grid = (TexCoord * _Tiling.xy - float2(0, _Tiling.y/_Tiling.w)); 
	float2 gridFloor = floor(grid); 
				
	float frameWithLerp = (((gridFloor.x +(_Tiling.x * (_Tiling.y - gridFloor.y)) / (_Tiling.x * _Tiling.y)) * (_Tiling.z * _Tiling.w))); 
	float frame = floor(frameWithLerp);
	float lerpVal = ceil(frameWithLerp);
	
	float2 prefOffset;
	float texCell = floor(_Tiling.z);
	prefOffset.x = ((float((float(frame) % float(texCell)))) / _Tiling.z);
	prefOffset.y = ((_Tiling.w - floor(float (frame / texCell))) / _Tiling.w);

	float2 nextOffset;
	nextOffset.x = ((float((float(lerpVal) % float(texCell)))) / _Tiling.z);
	nextOffset.y = ((_Tiling.w - floor(float(lerpVal / texCell))) / _Tiling.w);
	float2 tiling = ((grid - gridFloor) / _Tiling.zw);

	float d = 1;
	float2 edge = 2.0 / _Tiling.xy;
	d *= step(edge.x, tiling.x);
    d *= step(tiling.x, 1.0/_Tiling.z - edge.x);
    d *= step(edge.y, tiling.y);
    d *= step(tiling.y, 1.0/_Tiling.w - edge.y);
	float4 tex1 = tex2D (Tex, tiling + prefOffset);
	float4 tex2 = tex2D (Tex, tiling + nextOffset);
	return lerp (tex1, tex2, frameWithLerp - frame) * d;
} 