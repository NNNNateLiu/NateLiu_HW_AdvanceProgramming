using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCubes : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Service.CubeLifeCycleManagerInGame.Cubes.Remove(gameObject);
            int teamNumber = collision.gameObject.GetComponent<AI>().teamNumber;
            Service.EventManagerInGame.Fire(new Event_OnScore(teamNumber));
            Destroy(gameObject);
        }
    }
}
