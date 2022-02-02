using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILifecycleManager
{
    private List<GameObject> AIs = new List<GameObject>();

    private void Awake()
    {
        Service.AIManagerInGame = this;
    }

    public void Creation(GameObject thisAI)
    {
        AIs.Add(thisAI);
    }
    
    public void Updating()
    {
        Debug.Log("AI are moving");
        foreach (var AI in AIs)
        {
            AI.GetComponent<Rigidbody>().velocity = (
                (Service.CubeLifeCycleManagerInGame.Cubes[Service.CubeLifeCycleManagerInGame.Cubes.Count -1].transform.position - 
                AI.gameObject.transform.position).normalized * Service.GameLevelSystemInGame.aiMoveSpeed);
            Debug.Log(AI.name + "speed: " + (Service.CubeLifeCycleManagerInGame.Cubes[Service.CubeLifeCycleManagerInGame.Cubes.Count -1].transform.position - 
                                             AI.transform.position).normalized * Service.GameLevelSystemInGame.aiMoveSpeed);
            Debug.DrawLine(AI.transform.position, (Service.CubeLifeCycleManagerInGame.Cubes[Service.CubeLifeCycleManagerInGame.Cubes.Count -1].transform.position - 
                                                   AI.transform.position.normalized * Service.GameLevelSystemInGame.aiMoveSpeed), Color.red);
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


