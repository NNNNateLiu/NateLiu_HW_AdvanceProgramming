using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    public static void Initlization()
    {
        AIManagerInGame = new AILifecycleManager();
        CubeLifeCycleManagerInGame = new CubeLifeCycleManager();
        EventManagerInGame = new EventManager();
        GameLevelSystemInGame._fsm = new FiniteStateMachine<GameLevelSystem>(GameLevelSystemInGame);
        ScoreManagerInGame = new ScoreManager();
    }

    public static EventManager EventManagerInGame;
    public static PlayerController PlayerControllerInGame;
    public static AILifecycleManager AIManagerInGame;
    public static GameLevelSystem GameLevelSystemInGame;
    public static CubeLifeCycleManager CubeLifeCycleManagerInGame;
    public static ScoreManager ScoreManagerInGame;

    private void Update()
    {
        
    }
}
