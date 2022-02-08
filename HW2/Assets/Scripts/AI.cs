using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Rigidbody AIRigidbody;
    public GameObject Target;
    public int teamNumber;

    private void Start()
    {
        AIRigidbody = gameObject.GetComponent<Rigidbody>();
        
        Service.AIManagerInGame.test.Add(gameObject);
    }
}
