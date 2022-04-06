using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class EnemyController : MonoBehaviour
{
    private BehaviorTree.Tree<EnemyController> _tree;
    private NavMeshAgent _agent;
    
    //patrol
    public GameObject wayPointHolder;
    private List<Transform> wayPoints =  new List<Transform>();
    private Transform destination;
    private float changeDestinationDistance;

    //detection
    public bool isDetectPlayer;
    public GameObject player;
    public float detectPlayerDistance;
    
    //chase
    [SerializeField] private AudioSource chaseAudio;

    //[SerializeField] private GameObject visualizedWayPoint;

    //visually display enemies enter different states(chase/run away)
    private Color originalColor;
    private Material enemyMaterial;
    
    // Start is called before the first frame update
    void Start()
    {
        originalColor = gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color;
        enemyMaterial = gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        
        assignWayPoints();
        _agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
        SetEnemyDestination(destination.position);
        //_agent.SetDestination(destination.position);

        var reactTree = new Tree<EnemyController>
        (
            new Selector<EnemyController>
                (
                    new Sequence<EnemyController>
                        (
                            new Condition<EnemyController>(isPlayerStrongCondition),
                            new Do<EnemyController>(RunAwayAction)
                            ),
                    new Do<EnemyController>(ChaseAction)
                    )
        );
        var detectPlayerTree = new Tree<EnemyController>
        (
            new Sequence<EnemyController>
            (
                new Condition<EnemyController>(isDetectPlayerCondition),
                reactTree
            )
        );
        
        _tree = new Tree<EnemyController>
        (
            new Selector<EnemyController>
            (
                detectPlayerTree,
                new Do<EnemyController>(PatrolAction)
            )
        );
    }

    // Update is called once per frame
    void Update()
    {
        _tree.Update(this);
    }

    //patrol around waypoints, change to another waypoint within random distance
    private bool PatrolAction(EnemyController context)
    {
        Debug.Log("patrol");
        enemyMaterial.color = originalColor;
        detectPlayerDistance = 5;
        _agent.speed = 5f;
        chaseAudio.volume = 0;
        SetEnemyDestination(destination.position);
        //_agent.SetDestination(destination.position);
        if (Vector3.Distance(transform.position, destination.position) < changeDestinationDistance)
        {
            SetNewDestination();
        }
        return true;
    }

    //detect player within detectPlayerDistance
    private bool isDetectPlayerCondition(EnemyController context)
    {
        if (Vector3.Distance(transform.position, player.transform.position) < detectPlayerDistance)
        {
            Debug.Log("player nearby");
            isDetectPlayer = true;
        }
        else
        {
            isDetectPlayer = false;
        }
        return isDetectPlayer;
    }

    //if player is not strong, enemy will chase player
    private bool ChaseAction(EnemyController context)
    {
        Debug.Log("chase");
        enemyMaterial.color = Color.magenta;
        _agent.speed = 7f;
        chaseAudio.volume = 1f;
        SetEnemyDestination(player.transform.position);
        //_agent.SetDestination(player.transform.position);   
        return true;
    }

    //read if player is strong from PlayerController
    private bool isPlayerStrongCondition(EnemyController context)
    {
        bool _isPlayerStrong = player.GetComponent<PlayerController>().isPlayerStrong;
        return _isPlayerStrong;
    }

    //if player is strong, run away from player until outside detectPlayerDistance
    private bool RunAwayAction(EnemyController context)
    {
        Debug.Log("Run Away");
        detectPlayerDistance = 10;
        _agent.speed = 7f;
        enemyMaterial.color = Color.magenta;
        SetEnemyDestination(transform.position + (transform.position - player.transform.position).normalized * 15);
        //_agent.SetDestination(transform.position + (transform.position - player.transform.position).normalized * 15);
        SetNewDestination();
        return true;
    }
    
    //set a new nav destination
    public void SetNewDestination()
    {
        changeDestinationDistance = Random.Range(5f, 10f);
        destination = wayPoints[Random.Range(0, wayPoints.Count)];
        //_agent.SetDestination(destination.position);
    }

    private void assignWayPoints()
    {
        for (int i = 0; i < wayPointHolder.transform.childCount; i++)
        {
            Debug.Log("add one: " + wayPointHolder.transform.GetChild(i).name);
            wayPoints.Add(wayPointHolder.transform.GetChild(i));
        }
    }
    
    private void SetEnemyDestination(Vector3 position)
    {
        _agent.SetDestination(position);
        //visualizedWayPoint.transform.position = position;
    }
}
