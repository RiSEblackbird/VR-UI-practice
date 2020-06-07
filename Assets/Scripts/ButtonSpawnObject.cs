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
        for (int i = 0; i < 9; i++)
        {
            var spawnPosition = new Vector3(player.transform.position.x + UnityEngine.Random.Range(-1.0f, 1.0f), player.transform.position.y + UnityEngine.Random.Range(1.5f, 5.0f), player.transform.position.z + UnityEngine.Random.Range(-1.0f, 1.0f));
            var spawnRotation = Quaternion.Euler(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(0, 180));
            GameObject spawnedObj = Instantiate(obj, spawnPosition, spawnRotation) as GameObject;
            Destroy(spawnedObj, 30.0f);
        }
    }
}
