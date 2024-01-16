using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemySO : ScriptableObject
{
    public Transform prefab;
    public int cost;
    public Vector3[] spawnPoints;
    public float spawnDelay;
    public int firstWave; //the first wave this enemy can start appearing

}
