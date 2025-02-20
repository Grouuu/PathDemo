

// https://discussions.unity.com/t/mathf-perlin-in-a-scripted-shader/254768/2
float perlinNoise(float2 position) {
    // Compute the positions of the four closest grid vertices
    float4 grid = float4(floor(position), ceil(position));
    float4 gridX = grid.xzxz;
    float4 gridY = grid.yyww;

    // Compute a pseudorandom number for each grid vertex
    // The hash function used here is pcg_hash:
    // https://www.reedbeta.com/blog/hash-functions-for-gpu-rendering/
    uint4 hash = int4(gridX) ^ int4(gridY) << 16;
    hash = hash * 747796405 + 2891336453;
    hash = (hash >> (hash >> 28) + 4 ^ hash) * 277803737;
    hash = hash >> 22 ^ hash;

    // Compute a pseudorandom gradient vector for each grid vertex
    // Sums of random variables approximate a normal distribution
    // Normally distributed coordinates result in uniformly distributed directions
    float4 gradientX = ((hash >> 0) & 1023) + ((hash >> 7) & 1023) - 1023.0;
    float4 gradientY = ((hash >> 15) & 1023) + ((hash >> 22) & 1023) - 1023.0;

    // Compute distance vectors from grid vertices to input point
    float4 deltaX = position.x - gridX;
    float4 deltaY = position.y - gridY;

    // The contribution of each grid vertex is the dot product of the gradients and distance vectors
    float4 contribution = deltaX * gradientX + deltaY * gradientY;

    // Apply gradient normalisation
    contribution *= rsqrt(gradientX * gradientX + gradientY * gradientY);

    // Interpolation parameters
    float2 t = position - grid.xy;

    // Apply smoothstep function for smooth transitions between grid cells
    t = t * t * t * (t * (t * 6 - 15) + 10);

    // Interpolate between the contributions of each grid vertex
    float2 temp = lerp(contribution.xz, contribution.yw, t.x);
    float noise = lerp(temp.x, temp.y, t.y);

    // Scale and offset values to get a result range of [0, 1]
    noise = noise * rsqrt(2.0) + 0.5;

    return noise;
}

float4 blendColors(float4 topColor, float4 bottomColor, float bottomWeight) {
    return topColor * topColor.a + bottomColor * (1.0 - topColor.a) * bottomWeight;
}