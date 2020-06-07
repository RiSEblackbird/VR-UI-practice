using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class ButtonSpawnObject : MonoBehaviour
{
    public GameObject player;
    public GameObject obj;
    public Canvas canvas;

    public void SpawnObject()
    {
        var spawnPosition = new Vector3(player.transform.position.x, player.transform.position.y + 1.5f, player.transform.position.z);
        var spawnRotation = Quaternion.Euler(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180));
        GameObject spawnedObj = Instantiate(obj, spawnPosition, spawnRotation) as GameObject;
        Destroy(spawnedObj, 60.0f);

        Debug.Log(obj.name);
    }
}
