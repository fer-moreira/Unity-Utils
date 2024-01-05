using FFM;
using FFM.Pooling;
using FFM.Scriptables;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public Vector2 Position { get; private set; }
    public Vector2Int Coordinates { get; private set; }
    public int Size { get; private set; }
    public bool Loaded { get; private set; }

    private Dictionary<Vector2Int, DestructibleEntity> entitiesOnChunk;

    public void Initialize(Vector2 position, Vector2Int coordinates, int size) {
        Position = position;
        Coordinates = coordinates;
        Size = size;

        this.transform.position = position;
        entitiesOnChunk = new Dictionary<Vector2Int, DestructibleEntity>();
    }

    public void OnLoad() {
        if (Loaded) return;
        Loaded = true;

        int xOffset = Size / 2;
        int yOffset = Size / 2;

        for (int x = -xOffset; x < xOffset; x++) {
            for (int y = -yOffset; y < yOffset; y++) {
                int xPos = (int)Position.x + x;
                int yPos = (int)Position.y + y;
                Vector2Int pos = new Vector2Int(xPos, yPos);

                if (!entitiesOnChunk.ContainsKey(pos)) {
                    var entityData = MapGenerator.current.GetEntity(xPos, yPos);

                    if (entityData != null) {
                        var entity = DestructibleEntityPooling.instance.Get();
                        entity.name = entityData.name;
                        entity.Initialize(entityData);
                        entity.Pivot.position = new Vector2(xPos, yPos);

                        entitiesOnChunk.Add(
                            pos,
                            entity
                        );
                    }
                }
            }
        }

    }

    public void OnUnload() {
        if (!Loaded) return;
        Loaded = false;

        foreach (var entities in entitiesOnChunk) {
            if (entities.Value != null) {
                DestructibleEntityPooling.instance.Release(entities.Value);
            }
        }

        entitiesOnChunk.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Position, Vector2.one * Size);
    }
#endif
}