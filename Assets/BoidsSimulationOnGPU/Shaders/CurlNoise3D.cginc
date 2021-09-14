

#ifndef CURL_NOISE_3D
#define CURL_NOISE_3D

// Reference from Unity Graphics Programming Vol1
// 
// CurlNoise
float3 curlNoise(float3 coord)
{
	float3 dx = float3(EPSILON, 0.0, 0.0);
	float3 dy = float3(0.0, EPSILON, 0.0);
	float3 dz = float3(0.0, 0.0, EPSILON);

	float3 dpdx0 = simplexNoise(coord - dx);
	float3 dpdx1 = simplexNoise(coord + dx);
	float3 dpdy0 = simplexNoise(coord - dy);
	float3 dpdy1 = simplexNoise(coord + dy);
	float3 dpdz0 = simplexNoise(coord - dz);
	float3 dpdz1 = simplexNoise(coord + dz);

	float x = dpdy1.z - dpdy0.z + dpdz1.y - dpdz0.y;
	float y = dpdz1.x - dpdz0.x + dpdx1.z - dpdx0.z;
	float z = dpdx1.y - dpdx0.y + dpdy1.x - dpdy0.x;

	return float3(x, y, z) / EPSILON * 2.0;
}

#endif

