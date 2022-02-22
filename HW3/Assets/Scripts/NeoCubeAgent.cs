using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class NeoCubeAgent : Agent
{
    public GameObject ball;
    private Rigidbody ball_rb;
    private float scale;
    public float rorateSpeed;
    
    // Start is called before the first frame update
    public override void Initialize()
    {
        ball_rb = ball.GetComponent<Rigidbody>();
        SetBall();
    }

    public void SetBall()
    {
        scale = Random.Range(1f, 5f);
        ball_rb.mass = scale;
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }
    
    public override void OnEpisodeBegin()
    {
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        ball_rb.velocity = new Vector3(0f, 0f, 0f);
        ball.transform.position = new Vector3(Random.Range(-1.2f, 1.2f), 5f, Random.Range(-1.2f, 1.2f)) + gameObject.transform.position;
        SetBall();
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
        sensor.AddObservation(ball_rb.velocity);
        //8+1 = 9
        //scale of the ball
        sensor.AddObservation(scale);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionZ = rorateSpeed * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var actionX = rorateSpeed * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

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
