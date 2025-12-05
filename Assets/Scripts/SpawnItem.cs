using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpawnItem
{
    public GameObject prefab;
    public float probability;

    public SpawnItem(GameObject prefab, float probability)
    {
        this.prefab = prefab;
        this.probability = probability;
    }
}
