using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Rigidbody AIRigidbody;

    private void Start()
    {
        AIRigidbody = gameObject.GetComponent<Rigidbody>();
    }
}
