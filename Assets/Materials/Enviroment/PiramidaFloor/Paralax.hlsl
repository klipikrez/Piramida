{
  // determine required number of layers
  const float minLayers = 30;
  const float maxLayers = 60;
  float numLayers = lerp(maxLayers, minLayers, abs(dot(float3(0, 0, 1), viewDir)));

  float numSteps = numLayers;//60.0f; // How many steps the UV ray tracing should take
  float height = 1.0;
  float step = 1.0 / numSteps;
  
  float2 offset = uv.xy;
  float4 HeightMap = HeightTex.Sample(sampleState, offset);
  
  float2 delta = viewDir.xy * heightScale / (viewDir.z * numSteps);
  
  // find UV offset
  for (float i = 0.0f; i < numSteps; i++) {
    if (HeightMap.r < height) {
      height -= step;
      offset += delta;
      HeightMap = HeightTex.Sample(sampleState, offset);
    } else {
      break;
    }
  }
  Out = offset;
}  