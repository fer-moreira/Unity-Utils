using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FFM.Gameplay {
    public class ChunkSystem : MonoBehaviour {
        [SerializeField] private Transform m_Viewer;
        [SerializeField] private int m_ChunkSize = 8;
        [SerializeField] private int m_ViewDistance = 2;

        public Vector2 ViewerPosition => m_Viewer.position;

        private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

        private void Update() {
            UpdateChunks();
        }

        private void UpdateChunks() {
            Vector2Int viewerChunkCoord = new Vector2Int(
                Mathf.RoundToInt(ViewerPosition.x / m_ChunkSize),
                Mathf.RoundToInt(ViewerPosition.y / m_ChunkSize)
            );

            List<Vector2Int> chunksToRemove = new List<Vector2Int>();

            foreach (var chunkCoord in chunks.Keys) {
                int distanceX = Mathf.Abs(viewerChunkCoord.x - chunkCoord.x);
                int distanceY = Mathf.Abs(viewerChunkCoord.y - chunkCoord.y);

                // Check if the chunk is outside the view distance
                if (distanceX > m_ViewDistance || distanceY > m_ViewDistance) {
                    chunksToRemove.Add(chunkCoord);
                }
            }

            // Unload chunks outside the view distance
            foreach (var coordToRemove in chunksToRemove) {
                UnloadChunk(coordToRemove);
            }

            // Load new chunks within the view distance
            for (int xOffset = -m_ViewDistance; xOffset <= m_ViewDistance; xOffset++) {
                for (int yOffset = -m_ViewDistance; yOffset <= m_ViewDistance; yOffset++) {
                    Vector2Int currentChunkCoord = new Vector2Int(
                        viewerChunkCoord.x + xOffset,
                        viewerChunkCoord.y + yOffset
                    );

                    if (!chunks.ContainsKey(currentChunkCoord)) {
                        CreateChunk(currentChunkCoord);
                    }
                }
            }
        }

        private void UnloadChunk(Vector2Int coordinates) {
            if (chunks.TryGetValue(coordinates, out Chunk chunk)) {
                chunk.OnUnload();
                chunks.Remove(coordinates);
                ChunkPooler.instance.Release(chunk);
            }
        }

        private void CreateChunk(Vector2Int coordinates) {
            Vector2 chunkPosition = new Vector2(
                coordinates.x * m_ChunkSize,
                coordinates.y * m_ChunkSize
            );

            Chunk newChunk = ChunkPooler.instance.Get();
            newChunk.Initialize(
                chunkPosition,
                coordinates,
                m_ChunkSize
            );
            newChunk.OnLoad();

            chunks.Add(coordinates, newChunk);
        }
    }
}
