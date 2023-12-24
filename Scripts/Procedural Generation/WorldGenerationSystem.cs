using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationSystem : MonoBehaviour {
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    public float Scale;
    public int Octaves;
    public float Persistance;
    public float Lacunarity;
    public Vector2 Offset;
    public Gradient Colors;

    float[,] weights;
    


    public void GenerateChunk(int size) {
        weights = new float[0,0];

        weights = GeneratePerlinNoiseMap(
            new Vector2(size, size),
            Scale,
            Octaves,
            Persistance,
            Lacunarity,
            Offset
        );

        Texture2D texture = GenerateTexture(weights);
        AssignTextureToSpriteRenderer(texture);
    }
    float GetPerlinNoiseWeight(float x, float y) {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < Octaves; i++) {
            float sampleX = ((x - weights.GetLength(0) / 2f) / Scale + Offset.x) * frequency;
            float sampleY = ((y - weights.GetLength(1) / 2f) / Scale + Offset.y) * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= Persistance;
            frequency *= Lacunarity;
        }

        return Mathf.Clamp01(noiseHeight / 2f + 0.5f);
    }

    float[,] GeneratePerlinNoiseMap(Vector2 size, float scale, int octaves, float persistence, float lacunarity, Vector2 startOffset) {
        float[,] noiseMap = new float[(int)size.x, (int)size.y];

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                noiseMap[x, y] = GetPerlinNoiseWeight(x, y);
            }
        }

        return noiseMap;
    }
    
    
    Texture2D GenerateTexture(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float weight = noiseMap[x, y];
                colorMap[y * width + x] = Colors.Evaluate(weight);
            }
        }

        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;
    }

    void AssignTextureToSpriteRenderer(Texture2D texture) {
        if (spriteRenderer == null) {
            Debug.LogError("SpriteRenderer not assigned.");
            return;
        }

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
