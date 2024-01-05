using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPooler : PoolerBase<Chunk> {
    public static ChunkPooler instance;
    private void Awake() => instance = this;

    public Chunk chunkPrefab;

    private void Start() {
        InitPool(chunkPrefab);
    }
}
