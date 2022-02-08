using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager {
	
	//List of our registered handler (functions)
	private Dictionary<Type, AGPEvent.Handler> _registeredHandlers = new Dictionary<Type, AGPEvent.Handler>();
	//Type could be: Event_GameScored, Event_TimedOut
	//Function to register event
	public void Register<T>(AGPEvent.Handler handler) where T : AGPEvent 
	{
		var type = typeof(T);
		//Check dictionary already have this event type
		if (_registeredHandlers.ContainsKey(type)) 
		{
			if (!IsEventHandlerRegistered(type, handler))
				_registeredHandlers[type] += handler;         
		} 
		else 
		{
			//If not, generate new type as key, assign handler as value
			_registeredHandlers.Add(type, handler);         
		}     
	} 

	public void Unregister<T>(AGPEvent.Handler handler) where T : AGPEvent 
	{         
		var type = typeof(T);
		if (!_registeredHandlers.TryGetValue(type, out var handlers)) return;
		
		handlers -= handler;  
		
		if (handlers == null) 
		{                 
			_registeredHandlers.Remove(type);             
		} 
		else
		{
			_registeredHandlers[type] = handlers;             
		}
	}      
		
	//Triggered this event "e"
	public void Fire(AGPEvent e) 
	{       
		var type = e.GetType();

		if (_registeredHandlers.TryGetValue(type, out var handlers)) 
		{             
			handlers(e);
		}     
	} 

	public bool IsEventHandlerRegistered (Type typeIn, Delegate prospectiveHandler)
	{
		return _registeredHandlers[typeIn].GetInvocationList().Any(existingHandler => existingHandler == prospectiveHandler);
	}

}


//Basic event
public abstract class AGPEvent 
{
	public readonly float creationTime;

	public AGPEvent ()
	{
		creationTime = Time.time;
	}

	public delegate void Handler (AGPEvent e);
}

public class Event_OnScore : AGPEvent
{
	public readonly int teamIDScored;
	
	public Event_OnScore(int teamIDScored)
	{
		this.teamIDScored = teamIDScored;
	}
}

public class Event_OnGenerateCube : AGPEvent
{
	
}

public class Event_OnGameStart : AGPEvent
{
	
}

public class Event_OnTimeUp : AGPEvent
{
	public readonly int redTeamScore;
	public readonly int blueTeamScore;
	
	public Event_OnTimeUp(int redTeamScore, int blueTeamScore)
	{
		this.redTeamScore = redTeamScore;
		this.blueTeamScore = blueTeamScore;
	}
}