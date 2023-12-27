using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {
    public Vector2Int Position;
    public Vector2Int Size;
    public Bounds ChunkBounds;
    public bool Generated = false;

    public Chunk(Vector2Int coord, int size) {
        Position = coord * size;
        Size = Vector2Int.one * size;

        ChunkBounds = new Bounds(
            new Vector2(Position.x, Position.y),
            Vector2.one * size
        );

        this.UpdateChunk();
    }

    public void UpdateChunk() {
        if (Generated) return;

        float distFromChunk = Mathf.Sqrt(ChunkBounds.SqrDistance(MapGenerator.current.ViewerPosition));
        bool visible = distFromChunk <= MapGenerator.current.ViewerDistance;

        if (visible && !Generated) {
            Generate();
        }
    }

    public void Generate() {
        Generated = true;
        MapGenerator.current.GenerateChunk(Position, Size);
    }
}

public class MapGenerator : MonoBehaviour {

    [FoldoutGroup("World"), SerializeField] private int m_Seed;
    [FoldoutGroup("World"), SerializeField] private float m_Contrast = 5.0f;
    [FoldoutGroup("World"), SerializeField] private int m_MapSize = 512;
    [FoldoutGroup("World"), SerializeField] private SimplexSettings m_NoiseSettings;

    [FoldoutGroup("Chunk"), SerializeField] private int m_ChunkSize = 32;
    [FoldoutGroup("Chunk"), SerializeField] private Transform m_Viewer;
    [FoldoutGroup("Chunk"), SerializeField] private int m_ChunkViewerDistance = 300;
    [FoldoutGroup("Chunk"), SerializeField] private int m_ChunkSurroundLoad = 1;


    public WorldSettings settings {
        get => new WorldSettings(
            m_Seed,
            m_Contrast,
            m_MapSize,
            m_NoiseSettings
        );
    }


    private WorldGenerator worldGen;
    private float[,] weights;
    private Dictionary<Vector2Int, Chunk> worldChunks = new Dictionary<Vector2Int, Chunk>();
    private List<Chunk> lastLoadedChunks = new List<Chunk>();
    private int MaxChunkCount => settings.MapSize / m_ChunkSize;

    public Vector3 ViewerPosition => m_Viewer.position;
    public int ViewerDistance => m_ChunkViewerDistance;
    public static MapGenerator current;
    private void Awake() { current = this; }

    private void Start() {
        worldChunks = new Dictionary<Vector2Int, Chunk>();
        worldChunks.Clear();
    }

    private void Update() {
        UpdateChunk();
    }

    private void UpdateChunk() {
        int viewerChunkX = Mathf.RoundToInt(ViewerPosition.x / m_ChunkSize);
        int viewerChunkY = Mathf.RoundToInt(ViewerPosition.y / m_ChunkSize);

        Vector2Int viewerChunkPos = new Vector2Int(viewerChunkX, viewerChunkY);

        for (int x = viewerChunkX - m_ChunkSurroundLoad; x <= viewerChunkX + m_ChunkSurroundLoad; x++) {
            for (int y = viewerChunkY - m_ChunkSurroundLoad; y <= viewerChunkY + m_ChunkSurroundLoad; y++) {
                if (x >= 0 && x <= MaxChunkCount && y >= 0 && y <= MaxChunkCount) {
                    Vector2Int chunkPos = new Vector2Int(x, y);

                    if (worldChunks.ContainsKey(chunkPos)) {
                        worldChunks[chunkPos].UpdateChunk();
                    } else {
                        worldChunks.Add(chunkPos, new Chunk(chunkPos, m_ChunkSize));
                    }
                }
            }
        }
    }


    public void GenerateChunk(Vector2Int center, Vector2Int size) {
        for (int x = center.x - size.x / 2; x < center.x + size.x / 2; x++) {
            for (int y = center.y - size.y / 2; y < center.y + size.y / 2; y++) {
                //tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;

        foreach (KeyValuePair<Vector2Int, Chunk> chunkData in worldChunks) {

            if (chunkData.Value.Generated) {
                Color _c = Color.green;
                _c.a = 0.1f;
                Gizmos.color = _c;

                Gizmos.DrawCube(
                    new Vector3(chunkData.Value.Position.x, chunkData.Value.Position.y, 1),
                    new Vector3(m_ChunkSize, m_ChunkSize, 1)
                );
            } else {
                Gizmos.color = Color.red;

                Gizmos.DrawWireCube(
                    new Vector3(chunkData.Value.Position.x, chunkData.Value.Position.y, 1),
                    new Vector3(m_ChunkSize, m_ChunkSize, 1)
                );
            }
        }
    }
#endif

}
