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
        Cubes.Add(thisCube);
    }
    
    public void Updating()
    {
        
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
