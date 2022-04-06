using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public bool isPlayerStrong;
    public GameObject enemyPool;

    public List<Transform> respawnPoints;
    public List<GameObject> enemies;

    [SerializeField] private Material playerStrongMatreial;
    private Material originalMaterial;
    
    // Start is called before the first frame update
    void Start()
    {
        assignEnemy();
        originalMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //press Space to enter/exit strong state
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPlayerStrong = !isPlayerStrong;
            //change material to see in game view
            if (GetComponent<MeshRenderer>().material == originalMaterial)
            {
                GetComponent<MeshRenderer>().material = playerStrongMatreial;
            }
            else
            {
                GetComponent<MeshRenderer>().material = originalMaterial;
            }
        }
        // if enemy get too close, transport to another point in map
        if (calculateDistance())
        {
            gameObject.transform.position = respawnPoints[Random.Range(0, respawnPoints.Count)].position;
        }
    }

    //loop through all enemies and calculate their distance
    public bool calculateDistance()
    {
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(gameObject.transform.position, enemy.transform.position) <= 1f)
            {
                return true;
            }
        }
        return false;
    }
    
    //assign enemies under enemyPool to enemies list
    private void assignEnemy()
    {
        for (int i = 0; i < enemyPool.transform.childCount; i++)
        {
            //Debug.Log("add one: " + wayPointHolder.transform.GetChild(i).name);
            enemies.Add(enemyPool.transform.GetChild(i).gameObject);
        }
    }
}
