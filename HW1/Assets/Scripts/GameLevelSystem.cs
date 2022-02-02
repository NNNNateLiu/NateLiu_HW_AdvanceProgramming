using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameLevelSystem : MonoBehaviour
{
    public GameObject AIPrefab;
    public GameObject CubePrefab;
    //public List<CollectableCubes> collectableCubes;

    public float aiMoveSpeed;

    private void Awake()
    {
        Service.GameLevelSystemInGame = this;
        Service.Initlization();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject thisAI = Instantiate(AIPrefab, 
                new Vector3(Random.Range(-14, 14), 1.5f, Random.Range(-14, 14)), Quaternion.identity);
            Service.AIManagerInGame.Creation(thisAI);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject thisCube = Instantiate(CubePrefab,
                new Vector3(Random.Range(-14, 14), 1.5f, Random.Range(-14, 14)), Quaternion.identity);
            Service.CubeLifeCycleManagerInGame.Creation(thisCube);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
