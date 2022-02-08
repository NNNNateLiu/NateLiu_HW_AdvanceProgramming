using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLifeCycleManager
{
    public List<GameObject> Cubes = new List<GameObject>();

    private void Awake()
    {
        Service.CubeLifeCycleManagerInGame = this;
    }

    public void Creation(GameObject thisCube)
    {

    }
    
    public void Updating()
    {
        if (Service.GameLevelSystemInGame.currentWave == 0)
        {
            Service.GameLevelSystemInGame.currentWave++;
            GenerateCube();
        }
        if (Service.GameLevelSystemInGame.currentWave * Service.GameLevelSystemInGame.intervalPerWaveTime
            < Service.GameLevelSystemInGame.currentGameTime)
        {
            Service.GameLevelSystemInGame.currentWave++;
            GenerateCube();
        }
    }
    
    public void Destruction()
    {
        if (Cubes.Count == 0)
        {
            return;
        }
        else
        {
            foreach (var cube in Cubes)
            {
                Object.Destroy(cube);    
            }
            Cubes.Clear();
        }
    }
    
    public void Tracking()
    {
        
    }

    public void Move()
    {
        
    }

    private void GenerateCube()
    {
        for (int i = 0; i < Service.GameLevelSystemInGame.cubesNumberPerWave; i++)
        {
            GameObject thisCube = Object.Instantiate(Service.GameLevelSystemInGame.CubePrefab);
            thisCube.transform.position = new Vector3(Random.Range(-14, 14), 3f, Random.Range(-14, 14));
            Cubes.Add(thisCube);
            Debug.Log(Cubes[i].name + Cubes[i].transform.position);
        }
        Service.EventManagerInGame.Fire(new Event_OnGenerateCube());
    }
}
