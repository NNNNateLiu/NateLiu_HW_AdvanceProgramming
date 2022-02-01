using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    public static void Initlization()
    {
        //AIManagerInGame = new AILifecycleManager();
    }
    
    public static PlayerController PlayerControllerInGame;
    public static AILifecycleManager AIManagerInGame;
    public static GameLevelSystem GameLevelSystemInGame;

    private void Update()
    {
        AIManagerInGame.Updating(GameLevelSystemInGame.AIs);
    }
}
