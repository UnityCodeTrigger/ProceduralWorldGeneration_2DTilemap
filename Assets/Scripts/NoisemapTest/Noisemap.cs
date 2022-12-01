using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noisemap
{
    public static float[,] GetNoiseMap(int width, int height, float scale, Wave[] wavesFunc)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                float normalization = 0.0f;

                float samplePosX = (float)x * scale;
                float samplePosY = (float)y * scale;

                foreach (var wave in wavesFunc)
                {
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise((float)samplePosX * wave.frequency + wave.seed, (float)samplePosY * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                noiseMap[x, y] /= normalization;
            }
        }
        return noiseMap;
    }
}
