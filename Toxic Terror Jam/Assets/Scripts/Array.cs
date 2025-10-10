using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Array : MonoBehaviour
{
    public GameObject[] Coworkers;

    void Start()
    {
       Coworkers = GameObject.FindGameObjectsWithTag("Coworkers");

       for (int i = 0; i < Coworkers.Length; i++)
       {
            Debug.Log("Coworker Number "+i+" is named "+Coworkers[i].name);
       }
    }
}
