using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelSystem : MonoBehaviour
{
    public List<CollectableCubes> collectableCubes;
    public List<AI> AIs;

    private void Awake()
    {
        Service.GameLevelSystemInGame = this;
        Service.Initlization();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
