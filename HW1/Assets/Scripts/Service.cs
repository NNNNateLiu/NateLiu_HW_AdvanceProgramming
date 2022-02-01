using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    public static void Initlization()
    {
        AIManagerInGame = new AIManager();
    }
    
    public static PlayerController PlayerControllerInGame;
    public static AIManager AIManagerInGame;

    private void Update()
    {
        AIManagerInGame.Updating();
    }
}
