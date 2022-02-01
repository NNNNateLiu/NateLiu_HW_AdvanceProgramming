using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    public float PlayerMoveSpeed;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //if(Input.GetAxisRaw("Horizontal"))
        m_rigidbody.AddForce(Input.GetAxisRaw("Horizontal") * PlayerMoveSpeed * Vector3.right);
        m_rigidbody.AddForce(Input.GetAxisRaw("Vertical") * PlayerMoveSpeed * Vector3.forward);        
    }
}
