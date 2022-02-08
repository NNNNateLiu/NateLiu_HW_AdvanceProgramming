using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{
    public int redTeamScore;
    public int blueTeamScore;
    
    public void Creation()
    {
        Service.EventManagerInGame.Register<Event_OnScore>(AddScore);
    }

    public void Update()
    {
        
    }

    public void Destruction()
    {
        Service.EventManagerInGame.Unregister<Event_OnScore>(AddScore);
    }

    private void AddScore(AGPEvent e)
    {
        var scoredTeam = (Event_OnScore) e;
        Debug.Log("scored!");
        if (scoredTeam.teamIDScored == 0)
        {
            redTeamScore++;
            Service.GameLevelSystemInGame.txt_RedTeamScore.text = "Red: " + redTeamScore;
        }
        else
        {
            blueTeamScore++;
            Service.GameLevelSystemInGame.txt_BlueTeamScore.text = "Blue: " + blueTeamScore;
        }
        
    }
}
