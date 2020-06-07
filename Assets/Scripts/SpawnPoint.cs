using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;
    public GameObject spawnObject;

    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }
    
}
