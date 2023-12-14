using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplePooling : PoolerBase<ExamplePooling> {
    public static ExamplePooling instance;
    private void Awake() => instance = this;

    [SerializeField] private GameObject m_ExamplePrefab;
    private void Start() => InitPool(m_ExamplePrefab);
}
