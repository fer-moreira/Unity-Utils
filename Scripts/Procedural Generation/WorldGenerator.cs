using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class SimplexSettings {
    [SerializeField] private float m_Depth = 0f;
    [SerializeField] private int m_Octaves = 4;
    [SerializeField] private int m_Multiplier = 200;
    [SerializeField] private float m_Amplitude = 2f;
    [SerializeField] private float m_Lacunarity = 2f;
    [SerializeField] private float m_Persistence = 0.5f;
    [SerializeField] private float m_Cut = 0.3f;

    public float Depth => m_Depth;
    public int Octaves => m_Octaves;
    public int Size => m_Multiplier;
    public float Amplitude => m_Amplitude;
    public float Lacunarity => m_Lacunarity;
    public float Persistence => m_Persistence;
    public float Cut => m_Cut;

    public SimplexSettings(float depth, int octaves, int multiplier, float amplitude, float lacunarity, float persistence, float cut) {
        m_Depth = depth;
        m_Octaves = octaves;
        m_Multiplier = multiplier;
        m_Amplitude = amplitude;
        m_Lacunarity = lacunarity;
        m_Persistence = persistence;
        m_Cut = cut;
    }
}

[System.Serializable]
public class WorldSettings {
    [SerializeField] private int m_Seed;
    [SerializeField] private float m_Contrast = 5.0f;
    [SerializeField] private int m_MapSize = 512;
    [SerializeField] private SimplexSettings m_NoiseSettings;

    public string Seed => m_Seed.ToString();
    public float Contrast => m_Contrast;
    public int MapSize => m_MapSize;
    public SimplexSettings Settings => m_NoiseSettings;

    public WorldSettings(int seed, float contrast, int mapSize, SimplexSettings noiseSettings) {
        m_Seed = seed;
        m_Contrast = contrast;
        m_MapSize = mapSize;
        m_NoiseSettings = noiseSettings;
    }

}

public class WorldGenerator {

    private WorldSettings worldSettings;
    private SimplexNoiseGenerator simplex;
    private float[,] weights;

    public WorldGenerator(WorldSettings settings) {
        this.worldSettings = settings;
        simplex = new SimplexNoiseGenerator(settings.Seed);
    }

    public void GenerateWeights() {
        if (simplex == null) {
            Debug.LogWarning("Simplex missing, initialization needed");
            return;
        } else {
            weights = new float[worldSettings.MapSize, worldSettings.MapSize];

            for (int x = 0; x < worldSettings.MapSize; x++) {
                for (int y = 0; y < worldSettings.MapSize; y++) {
                    float distance = Vector2.Distance(new Vector2(x, y) / worldSettings.MapSize, Vector2.one * 0.5f);
                    float gradient = Mathf.SmoothStep(0, 1, distance);

                    gradient = Mathf.InverseLerp(1, 0, gradient);
                    gradient = Mathf.Pow(gradient, worldSettings.Contrast);

                    float simplexValue = simplex.coherentNoise(
                        x, y,
                        worldSettings.Settings.Depth,
                        worldSettings.Settings.Octaves,
                        worldSettings.Settings.Size,
                        worldSettings.Settings.Amplitude,
                        worldSettings.Settings.Lacunarity,
                        worldSettings.Settings.Persistence,
                        worldSettings.Settings.Cut
                    );

                    gradient *= simplexValue;
                    weights[x, y] = gradient;
                }
            }
        }
    }

    public float GetWeight(int x, int y) {
        if (x >= 0 && x < weights.GetLength(0) && y >= 0 && y < weights.GetLength(1)) {
            return weights[x, y];
        } else {
            return 0f;
        }
    }
}
