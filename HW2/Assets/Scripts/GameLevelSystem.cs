using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GameLevelSystem : MonoBehaviour
{
    public GameObject AIPrefab;
    public GameObject CubePrefab;

    [Header("UI")]
    public GameObject startMenuObject;
    public GameObject endMenuObject;
    public GameObject inGameMenuObject;
    public Text txt_EndGameWinMessage;
    public Text txt_RedTeamScore;
    public Text txt_BlueTeamScore;
    public Text txt_TimeCountDown;

    [Header("Time & Score")]
    public int cubesNumberPerWave;
    public float totalGameTime;
    public float intervalPerWaveTime;
    public float currentGameTime;
    public int currentWave;
    
    [Header("Player & AI")]
    public int playerNumberForEachTeam;
    public Material redTeamMaterial;
    public Material blueTeamMaterial;
    public Transform playerStartPoint;
    public GameObject player;
    
    
    //public List<CollectableCubes> collectableCubes;

    public float aiMoveSpeed;

    public FiniteStateMachine<GameLevelSystem> _fsm;

    private void Awake()
    {
        Service.GameLevelSystemInGame = this;
        Service.Initlization();
        Service.EventManagerInGame.Register<Event_OnGameStart>(OnGameStart);
        Service.EventManagerInGame.Register<Event_OnTimeUp>(OnTimeUp);
    }

    // Start is called before the first frame update
    void Start()
    {
        _fsm.TransitionTo<State_MainMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        _fsm.Update();
    }

    private class State_MainMenu : FiniteStateMachine<GameLevelSystem>.State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Context.startMenuObject.SetActive(true);
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.T))
            {
                Service.EventManagerInGame.Fire(new Event_OnGameStart());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Context.startMenuObject.SetActive(false);
        }
    }
    
    private class State_InGame : FiniteStateMachine<GameLevelSystem>.State
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Context.currentGameTime = 0;
            Context.currentWave = 0;
            Service.GameLevelSystemInGame.txt_RedTeamScore.text = "Red: " + 0;
            Service.GameLevelSystemInGame.txt_BlueTeamScore.text = "Blue: " + 0;
            Service.AIManagerInGame.Creation();
            Service.ScoreManagerInGame.Creation();
            Context.inGameMenuObject.SetActive(true);
            Context.player.transform.position = Context.playerStartPoint.position;
        }

        public override void Update()
        {
            base.Update();
            
            //track game time to generate new wave of cubes
            Service.CubeLifeCycleManagerInGame.Updating();
            //track nearest cube and run towards it
            Service.AIManagerInGame.Updating();
            Context.currentGameTime += Time.deltaTime;
            Context.txt_TimeCountDown.text = "Time Left: " + Math.Floor(Context.totalGameTime - Context.currentGameTime);
            if (Context.currentGameTime >= Context.totalGameTime)
            {
                int _blueTeamScore = Service.ScoreManagerInGame.blueTeamScore;
                int _redTeamScore = Service.ScoreManagerInGame.redTeamScore;
                
                Service.EventManagerInGame.Fire(new Event_OnTimeUp(_blueTeamScore,_redTeamScore));
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Service.AIManagerInGame.Destruction();
            Service.ScoreManagerInGame.Destruction();
            Service.CubeLifeCycleManagerInGame.Destruction();
            Context.inGameMenuObject.SetActive(false);
        }
    }
    
    private class State_EndGame : FiniteStateMachine<GameLevelSystem>.State
    {
        public override void OnEnter()
        {

            base.OnEnter();
            Context.endMenuObject.SetActive(true);
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.T))
            {
                Service.EventManagerInGame.Fire(new Event_OnGameStart());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Context.endMenuObject.SetActive(false);
        }
    }

    public void OnGameStart(AGPEvent e)
    {
        Service.GameLevelSystemInGame._fsm.TransitionTo<State_InGame>();
    }
    
    public void OnTimeUp(AGPEvent e)
    {
        Service.GameLevelSystemInGame._fsm.TransitionTo<State_EndGame>();
    }
}
