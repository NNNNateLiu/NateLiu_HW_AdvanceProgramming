using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class AILifecycleManager
{
    private List<GameObject> AIs = new List<GameObject>();

    public List<GameObject> test = new List<GameObject>();

    private void Awake()
    {
        Service.AIManagerInGame = this;
    }

    public void Creation()
    {
        Service.EventManagerInGame.Register<Event_OnScore>(AI_OnScoer);
        Service.EventManagerInGame.Register<Event_OnGenerateCube>(AI_OnScoer);
        GenerateAI();
    }
    
    public void Updating()
    {
        //Debug.Log("AI are moving");
        foreach (var AI in AIs)
        {
            MoveToNearestCube(AI);
        }
    }
    
    public void Destruction()
    {
        Service.EventManagerInGame.Unregister<Event_OnScore>(AI_OnScoer);
        Service.EventManagerInGame.Unregister<Event_OnGenerateCube>(AI_OnScoer);
        foreach (var AI in AIs)
        {
            Object.Destroy(AI);
        }
        AIs.Clear();
    }
    
    public void Tracking()
    {
        
    }

    public void Move()
    {
        
    }

    private void GenerateAI()
    {
        for (int i = 0; i < Service.GameLevelSystemInGame.playerNumberForEachTeam * 2f; i++)
        {
            GameObject thisAI = UnityEngine.Object.Instantiate(Service.GameLevelSystemInGame.AIPrefab, 
                new Vector3(Random.Range(-14, 14), 1.5f, Random.Range(-14, 14)), Quaternion.identity);
            if (i > Service.GameLevelSystemInGame.playerNumberForEachTeam - 1)
            {
                thisAI.GetComponent<Renderer>().material = Service.GameLevelSystemInGame.redTeamMaterial;
                thisAI.GetComponent<AI>().teamNumber = 0;
                Debug.Log("red ai created");
            }
            else
            {
                thisAI.GetComponent<Renderer>().material = Service.GameLevelSystemInGame.blueTeamMaterial;
                thisAI.GetComponent<AI>().teamNumber = 1;
                Debug.Log("blue ai created");
            }
            AIs.Add(thisAI);
        }
    }

    public void SetNearestCube(GameObject AI)
    {

        if (Service.CubeLifeCycleManagerInGame.Cubes.Count == 0)
        {
            AI.GetComponent<AI>().Target = null;
        }
        else
        {
            GameObject nearestCube = Service.CubeLifeCycleManagerInGame.Cubes[0];
            float shortestDistance = float.MaxValue;
            for (int i = 0; i < Service.CubeLifeCycleManagerInGame.Cubes.Count; i++)
            {
                float nextDistance = Vector3.Distance(Service.CubeLifeCycleManagerInGame.Cubes[i].transform.position,
                    AI.transform.position);
                //Debug.Log(Service.CubeLifeCycleManagerInGame.Cubes[i].transform.position);
                //Debug.Log(nextDistance);
                if (nextDistance < shortestDistance)
                {
                    nearestCube = Service.CubeLifeCycleManagerInGame.Cubes[i];
                    shortestDistance = nextDistance;
                }
            }
            AI.GetComponent<AI>().Target = nearestCube;
            Debug.DrawLine(AI.transform.position, (Service.CubeLifeCycleManagerInGame.Cubes[Service.CubeLifeCycleManagerInGame.Cubes.Count -1].transform.position - 
                                                   AI.transform.position.normalized * Service.GameLevelSystemInGame.aiMoveSpeed), Color.red);
        }
        //Debug.Log(neatestCube.name + neatestCube.transform.position);
        //Debug.Log(AI.name + "speed: " + (Service.CubeLifeCycleManagerInGame.Cubes[Service.CubeLifeCycleManagerInGame.Cubes.Count -1].transform.position - AI.transform.position).normalized * Service.GameLevelSystemInGame.aiMoveSpeed);

    }

    private void MoveToNearestCube(GameObject AI)
    {
        if (AI.GetComponent<AI>().Target == null)
        {
            return;
        }
        else
        {
            AI.GetComponent<Rigidbody>().velocity =
                (AI.GetComponent<AI>().Target.transform.position - AI.transform.position).normalized *
                Service.GameLevelSystemInGame.aiMoveSpeed;
        }
    }

    public void AI_OnScoer(AGPEvent e)
    {
        foreach (var AI in AIs)
        {
            SetNearestCube(AI);
            Debug.Log("set cube");
        }
    }
}


