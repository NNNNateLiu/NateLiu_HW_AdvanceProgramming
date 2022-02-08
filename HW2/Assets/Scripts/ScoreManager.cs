using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager
{
    public int redTeamScore;
    public int blueTeamScore;
    
    public void Creation()
    {
        redTeamScore = 0;
        blueTeamScore = 0;
        Service.EventManagerInGame.Register<Event_OnScore>(AddScore);
        Service.EventManagerInGame.Register<Event_OnTimeUp>(OnTimeUp);
    }

    public void Update()
    {
        
    }

    public void Destruction()
    {
        Service.EventManagerInGame.Unregister<Event_OnScore>(AddScore);
        Service.EventManagerInGame.Unregister<Event_OnTimeUp>(OnTimeUp);
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

    private void OnTimeUp(AGPEvent e)
    {
        if (blueTeamScore > redTeamScore)
        {
            Service.GameLevelSystemInGame.txt_EndGameWinMessage.text = "Blue Win";
        }
        else if (blueTeamScore < redTeamScore)
        {
            Service.GameLevelSystemInGame.txt_EndGameWinMessage.text = "Red Win";
        }
        else
        {
            Service.GameLevelSystemInGame.txt_EndGameWinMessage.text = "Draw";
        }
    }
}
