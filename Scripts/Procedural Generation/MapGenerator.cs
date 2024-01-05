using FFM;
using FFM.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[System.Serializable]
public class TileLayers {
    [FoldoutGroup("$LayerID")]
    public bool EnableLayer = true;

    [FoldoutGroup("$LayerID")]
    [Range(0f, 1f)] public float LayerLevel = 0f;

    [FoldoutGroup("$LayerID")]
    public TileBase Tile;

    [FoldoutGroup("$LayerID")]
    public Tilemap Tilemap;

    public bool HeightMatch(float value) => value >= LayerLevel;

    public void Place(int x, int y) => Tilemap.SetTile(new Vector3Int(x, y), Tile);

    private string LayerID {
        get {
            if (Tile != null) {
                return $"{Tile.name} ({LayerLevel.ToString()})";
            } else {
                return "Missing Tile";
            }
        }
    }
}

[System.Serializable]
public class EntityLayer {
    [FoldoutGroup("$LayerName")]
    public string LayerName = "Layer";

    [FoldoutGroup("$LayerName")]
    public bool RandomEntity;

    [FoldoutGroup("$LayerName"), HideIf("RandomEntity")]
    public EntityBaseScriptable Entity;

    [FoldoutGroup("$LayerName"), ShowIf("RandomEntity")]
    public EntityBaseScriptable[] Entities;

    [FoldoutGroup("$LayerName"), MinMaxSlider(0, 1, ShowFields = true)]
    public Vector2 MaskRange = new Vector2(0.5f, 1f);

    [FoldoutGroup("$LayerName"), MinMaxSlider(0, 1, ShowFields = true)]
    public Vector2 HeightRange = new Vector2(0.25f, 0.5f);

    [FoldoutGroup("$LayerName")]
    [Range(0f, 1f)] public float SpawnRate = 1;

    [FoldoutGroup("$LayerName")]
    public Color DebugColor = Color.white;

    [FoldoutGroup("$LayerName/Noise Settings")] public float Depth = 0f;
    [FoldoutGroup("$LayerName/Noise Settings")] public int Octaves = 8;
    [FoldoutGroup("$LayerName/Noise Settings")] public int Multiplier = 40;
    [FoldoutGroup("$LayerName/Noise Settings")] public float Amplitude = 1f;
    [FoldoutGroup("$LayerName/Noise Settings")] public float Lacunarity = 2f;
    [FoldoutGroup("$LayerName/Noise Settings")] public float Persistence = 0.4f;
    [FoldoutGroup("$LayerName/Noise Settings")] public float Cut = 0.5f;

    public bool HeightMatch(float value) => value >= HeightRange.x && value <= HeightRange.y;


    public SimplexSettings NoiseSettings {
        get {
            return new SimplexSettings(
                Depth,
                Octaves,
                Multiplier,
                Amplitude,
                Lacunarity,
                Persistence,
                Cut
            );
        }
    }


    public EntityBaseScriptable GetEntity(string seed) {
        if (!RandomEntity) {
            return Entity;
        } else {
            EntityBaseScriptable randomEntity = null;
            var oldState = Random.state;

            Random.InitState(seed.GetHashCode());

            int desiredEntity = Random.Range(0, Entities.Length);

            if (desiredEntity >= 0 && desiredEntity <= Entities.Length)
                randomEntity = Entities[desiredEntity];

            Random.state = oldState;

            return randomEntity;
        }
    }
}

public class MapGenerator : MonoBehaviour {
    public static MapGenerator current;
    private void Awake() { current = this; }

    private WorldGenerator worldGenerator;
    private WorldGenerator entityGenerator;
    public int MapSize => m_MapSize;

    [FoldoutGroup("World"), SerializeField] private int m_Seed;
    [FoldoutGroup("World"), SerializeField] private int m_MapSize = 512;
    [FoldoutGroup("World"), SerializeField] private float m_Contrast = 2f;

    [FoldoutGroup("World/Noise Settings"), SerializeField] private float m_Depth = 0f;
    [FoldoutGroup("World/Noise Settings"), SerializeField] private int m_Octaves = 8;
    [FoldoutGroup("World/Noise Settings"), SerializeField] private int m_Multiplier = 150;
    [FoldoutGroup("World/Noise Settings"), SerializeField] private float m_Amplitude = 1f;
    [FoldoutGroup("World/Noise Settings"), SerializeField] private float m_Lacunarity = 2f;
    [FoldoutGroup("World/Noise Settings"), SerializeField] private float m_Persistence = 0.4f;
    [FoldoutGroup("World/Noise Settings"), SerializeField] private float m_Cut = 0.4f;

    [FoldoutGroup("World"), SerializeField] private TileLayers[] m_TileLayers;
    [FoldoutGroup("World"), SerializeField] private EntityLayer[] m_Entities;

    private int[,] entityMap;

    private void Start() {
        this.GenerateWorldTilemap();
        this.GenerateEntitiesMap();
    }

    private void GenerateWorldTilemap() {
        void GenerateHeightmap() {
            worldGenerator = new WorldGenerator(
            new WorldSettings(
                m_Seed,
                m_Contrast,
                m_MapSize,
                new SimplexSettings(
                    m_Depth,
                    m_Octaves,
                    m_Multiplier,
                    m_Amplitude,
                    m_Lacunarity,
                    m_Persistence,
                    m_Cut
                )
            )
        );

            worldGenerator.GenerateSimplex();
        }
        void GenerateTiles() {
            var center = Vector2Int.one * (m_MapSize / 2);
            var size = Vector2Int.one * m_MapSize;

            for (int x = center.x - size.x / 2; x < center.x + size.x / 2; x++) {
                for (int y = center.y - size.y / 2; y < center.y + size.y / 2; y++) {
                    for (int layerIndex = 0; layerIndex < m_TileLayers.Length; layerIndex++) {
                        TileLayers tileLayer = m_TileLayers[layerIndex];

                        if (!tileLayer.EnableLayer)
                            continue;

                        float height = worldGenerator.SampleCoord(x, y);

                        height = Mathf.Clamp01(height);

                        if (tileLayer.HeightMatch(height)) {
                            tileLayer.Place(x, y);
                        }
                    }
                }
            }
        }

        foreach (var tileLayers in m_TileLayers)
            tileLayers.Tilemap.ClearAllTiles();

        GenerateHeightmap();
        GenerateTiles();
    }

    private void GenerateEntitiesMap() {
        entityMap = new int[m_MapSize, m_MapSize];
        var oldState = Random.state;

        for (int layerIndex = 0; layerIndex < m_Entities.Length; layerIndex++) {
            int layerDimensionIndex = layerIndex + 1;
            EntityLayer entityLayer = m_Entities[layerIndex];

            WorldGenerator noiseGen = new WorldGenerator(new WorldSettings(
                m_Seed,
                m_Contrast,
                m_MapSize,
                entityLayer.NoiseSettings
            ));

            noiseGen.GenerateSimplexMasked(
                worldGenerator.SimplexWeights,
                entityLayer.MaskRange.x,
                entityLayer.MaskRange.y
            );

            for (int x = 0; x < m_MapSize; x++) {
                for (int y = 0; y < m_MapSize; y++) {
                    Random.InitState($"{m_Seed}_{layerIndex}_{x}_{y}".GetHashCode());

                    float r = Random.value;

                    if (r >= 0 && r <= entityLayer.SpawnRate) {
                        float w = noiseGen.GetSimplexWeight(x, y);
                        w = Mathf.Clamp01(w);

                        if (w >= entityLayer.HeightRange.x && w <= entityLayer.HeightRange.y)
                            entityMap[x, y] = layerDimensionIndex;
                    }
                }
            }
        }

        Random.state = oldState;
    }

    public DestructibleEntityScriptable GetEntity(int x, int y) {
        string seed = $"{x}_{y}_{m_Seed}";

        bool xInRange = (x >= 0 && x < entityMap.GetLength(0));
        bool yInRange = (y >= 0 && y < entityMap.GetLength(1));

        if (xInRange && yInRange) {
            int entityAtCoord = entityMap[x, y] - 1;

            if (entityAtCoord >= 0 && entityAtCoord <= m_Entities.Length) {
                EntityLayer entityLayer = m_Entities[entityAtCoord];
                DestructibleEntityScriptable entityScriptable = entityLayer.GetEntity(seed) as DestructibleEntityScriptable;
                return entityScriptable;
            }

            return null;
        } else {
            return null;
        }
    }


#if UNITY_EDITOR
    Texture2D debugTexture;
    bool showTexture = false; // Initially show the texture

    void GenerateDebugTexture() {
        debugTexture = new Texture2D(m_MapSize, m_MapSize);
        Color[] pixels = new Color[m_MapSize * m_MapSize];

        for (int x = 0; x < m_MapSize; x++) {
            for (int y = 0; y < m_MapSize; y++) {
                Color color = Color.black;

                int entityIndex = entityMap[x, y];
                entityIndex = entityIndex - 1;

                if (entityIndex >= 0 && entityIndex <= m_MapSize) {
                    color = m_Entities[entityIndex].DebugColor;
                }

                pixels[x + y * m_MapSize] = color;
            }
        }

        debugTexture.SetPixels(pixels);
        debugTexture.filterMode = FilterMode.Point;
        debugTexture.Apply();
    }

    private float scaleFactor;
    private void OnGUI() {
        Rect btnRect = new Rect((Screen.width / 2) - (150 / 2), 10, 150, 30);
        Rect sliderRect = btnRect;
        sliderRect.y = 40;

        if (GUI.Button(btnRect, "Toggle Texture")) {
            showTexture = !showTexture;
        }

        if (showTexture) {
            if (debugTexture == null)
                GenerateDebugTexture();

            scaleFactor = GUI.HorizontalSlider(sliderRect, scaleFactor, 0.5f, 3.0F);

            float tHeight = Screen.height / scaleFactor;

            float centerX = Screen.width / 2 - tHeight / 2;
            float centerY = Screen.height / 2 - tHeight / 2;

            GUI.DrawTexture(
                new Rect(
                    centerX, centerY,
                    tHeight, tHeight
                ),
                debugTexture
            );
        }
    }
#endif
}
