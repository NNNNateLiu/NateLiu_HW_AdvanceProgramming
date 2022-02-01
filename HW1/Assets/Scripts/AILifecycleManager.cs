using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILifecycleManager : MonoBehaviour
{
    public List<AI> AIs;
    public float AIMoveSpeed;

    private void Awake()
    {
        Service.AIManagerInGame = this;
    }

    public void Creation()
    {
        
    }
    
    public void Updating(List<AI> ais)
    {
        Debug.Log("AI are moving");
        foreach (var AI in ais)
        {
            AI.AIRigidbody.velocity = (
                (Service.GameLevelSystemInGame.collectableCubes[Service.GameLevelSystemInGame.collectableCubes.Count -1].transform.position - 
                AI.gameObject.transform.position).normalized * AIMoveSpeed);
            Debug.Log(AI.name + "speed: " + (Service.GameLevelSystemInGame.collectableCubes[Service.GameLevelSystemInGame.collectableCubes.Count -1].transform.position - 
                      AI.transform.position).normalized);
            Debug.DrawLine(AI.transform.position, (Service.GameLevelSystemInGame.collectableCubes[Service.GameLevelSystemInGame.collectableCubes.Count -1].transform.position - 
                                                              AI.transform.position.normalized * AIMoveSpeed), Color.red);
        }
    }
    
    public void Destruction()
    {
        
    }
    
    public void Tracking()
    {
        
    }

    public void Move()
    {
        
    }
}


