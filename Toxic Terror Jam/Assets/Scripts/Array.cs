using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Array : MonoBehaviour
{
    public GameObject[] coworkersPrefabs;

    public Transform spawnpoint;

    void Start()
    {
       SpawnRandomCoworker();
    }

    void SpawnRandomCoworker()
    {
        if (coworkersPrefabs.Length == 0)
        {
            Debug.LogWarning("KeineCopworkers");
            return;
        }

        int randomIndex = Random.Range(0, coworkersPrefabs.Length);

        Vector3 position = spawnpoint != null ? spawnpoint.position : transform.position;

        GameObject spawned = Instantiate(coworkersPrefabs[randomIndex], position, Quaternion.identity);

        Debug.Log("Coworker gespawnt: " + spawned.name);
    
    
    
    }
}
