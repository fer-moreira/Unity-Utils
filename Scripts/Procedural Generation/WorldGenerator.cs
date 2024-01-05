using UnityEngine;

[System.Serializable]
public class SimplexSettings {
    public float Depth = 0f;
    public int Octaves = 4;
    public int Multiplier = 200;
    public float Amplitude = 2f;
    public float Lacunarity = 2f;
    public float Persistence = 0.5f;
    public float Cut = 0.3f;


    public SimplexSettings(
        float depth = 0f,
        int octaves = 5,
        int multiplier = 50,
        float amplitude = 1,
        float lacunarity = 2,
        float persistence = 0.5f,
        float cut = 0.5f
    ) {
        Depth = depth;
        Octaves = octaves;
        Multiplier = multiplier;
        Amplitude = amplitude;
        Lacunarity = lacunarity;
        Persistence = persistence;
        Cut = cut;
    }
}

[System.Serializable]
public class WorldSettings {
    public int Seed;
    public float Contrast = 5.0f;
    public int MapSize = 512;
    public SimplexSettings Settings;

    public string SeedString => Seed.ToString();

    public WorldSettings(
        int seed = 0,
        float contrast = 1f,
        int mapSize = 64,
        SimplexSettings noiseSettings = null
    ) {
        Seed = seed;
        Contrast = contrast;
        MapSize = mapSize;
        Settings = noiseSettings;
    }

}

public class WorldGenerator {

    private WorldSettings worldSettings;
    private SimplexNoiseGenerator simplex;

    private float[,] simplexWeights;
    public float[,] SimplexWeights => simplexWeights;

    public WorldGenerator(WorldSettings settings) {
        this.worldSettings = settings;
        simplex = new SimplexNoiseGenerator(settings.SeedString);
    }

    public void SetSettings(WorldSettings settings) {
        this.worldSettings = settings;
    }

    public void GenerateSimplexMasked(float[,] mask, float min, float max) {
        if (simplex == null) {
            Debug.LogWarning("Simplex missing, initialization needed");
            return;
        } else {
            simplexWeights = new float[worldSettings.MapSize, worldSettings.MapSize];

            for (int x = 0; x < worldSettings.MapSize; x++) {
                for (int y = 0; y < worldSettings.MapSize; y++) {
                    float maskHeight = mask[x, y];

                    if (maskHeight >= min && maskHeight <= max) {
                        float height = SampleCoord(x, y);
                        simplexWeights[x, y] = height;
                    } else {
                        simplexWeights[x, y] = -1f;
                    }
                }
            }
        }
    }

    public void GenerateSimplex() {
        if (simplex == null) {
            Debug.LogWarning("Simplex missing, initialization needed");
            return;
        } else {
            simplexWeights = new float[worldSettings.MapSize, worldSettings.MapSize];

            for (int x = 0; x < worldSettings.MapSize; x++) {
                for (int y = 0; y < worldSettings.MapSize; y++) {
                    float height = this.SampleCoord(x, y);
                    simplexWeights[x, y] = height;
                }
            }
        }
    }

    /// <summary>
    /// Sample a weight from a generated simplex array, 
    /// value is not clamped, can have values below 0 and above 1
    /// </summary>
    /// <param name="x">X Position</param>
    /// <param name="y">Y Position</param>
    /// <returns>value range from [-infinity, +infinity]</returns>
    public float SampleCoord(int x, int y) {
        float distance = Vector2.Distance(new Vector2(x, y) / worldSettings.MapSize, Vector2.one * 0.5f);
        float gradient = Mathf.SmoothStep(0, 1, distance);

        gradient = Mathf.InverseLerp(1, 0, gradient);
        gradient = Mathf.Pow(gradient, worldSettings.Contrast);

        float simplexValue = simplex.coherentNoise(
            x, y,
            worldSettings.Settings.Depth,
            worldSettings.Settings.Octaves,
            worldSettings.Settings.Multiplier,
            worldSettings.Settings.Amplitude,
            worldSettings.Settings.Lacunarity,
            worldSettings.Settings.Persistence,
            worldSettings.Settings.Cut
        );

        gradient *= simplexValue;

        return gradient;
    }

    public float GetSimplexWeight(int x, int y) => GetWeight(x, y, simplexWeights);

    private float GetWeight(int x, int y, float[,] weights) {
        if (x >= 0 && x < worldSettings.MapSize && y >= 0 && y < worldSettings.MapSize)
            return weights[x, y];
        else
            return 0f;
    }
}
