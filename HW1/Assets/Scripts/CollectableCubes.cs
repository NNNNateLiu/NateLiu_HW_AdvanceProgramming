using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCubes : MonoBehaviour
{
    private void Start()
    {
        Service.GameLevelSystemInGame.collectableCubes.Add(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Service.GameLevelSystemInGame.collectableCubes.Remove(this);
            Destroy(gameObject);
        }
    }
}
