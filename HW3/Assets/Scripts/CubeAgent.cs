using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CubeAgent : Agent
{
	public GameObject ball;
	Rigidbody m_BallRb;

	public override void Initialize()
	{
		m_BallRb = ball.GetComponent<Rigidbody>();
		SetResetParameters();
	}

	void SetResetParameters()
	{
		SetBall();
	}

	void SetBall()
	{
		var scale = 1;
		m_BallRb.mass = scale;

		ball.transform.localScale = new Vector3(scale, scale, scale);
	}

	public override void OnEpisodeBegin()
	{
		gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
		gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
		gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
		m_BallRb.velocity = new Vector3(0f, 0f, 0f);
		ball.transform.position = new Vector3(Random.Range(-1.2f, 1.2f), 4f, Random.Range(-1.2f, 1.2f)) + gameObject.transform.position;
		SetResetParameters();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		//1
		sensor.AddObservation(gameObject.transform.rotation.z);
		//2
		sensor.AddObservation(gameObject.transform.rotation.x);
		//2+3 = 5
		sensor.AddObservation(ball.transform.position - gameObject.transform.position);
		//5+3 = 8
		sensor.AddObservation(m_BallRb.velocity);
	}

	public override void OnActionReceived(ActionBuffers actions)
	{
		var actionZ = 2f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
		var actionX = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

		if((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) || (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
		{
			gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
		}
		if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) || (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
		{
			gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);

		}

		if((ball.transform.position.y - gameObject.transform.position.y) < -3f || 
			Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 5f || 
			Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 5f)
		{
			SetReward(-1f);
			EndEpisode();
		}
		else
		{
			SetReward(.1f);
		}
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		var continuousActionsOut = actionsOut.ContinuousActions;
		continuousActionsOut[0] = -Input.GetAxis("Horizontal");
		continuousActionsOut[1] = -Input.GetAxis("Vertical");
	}
}
