using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

/// <summary>
/// A hummingbird Machine Learning Agent
/// </summary>
public class HummingbirdAgent : Agent
{
	//Force to apply to the bird
	private float moveForce = 2f;
	//For rotation, pitch direction
	private float pitchSpeed = 100f;
	//For rotation, yaw direction
	private float yawSpeed = 100f;
	//Bird mouth's tip
	public Transform beakTip;
	//Agent's camera
	public Camera agentCamera;
	//For gameMode / trainingMode check
	public bool trainingMode;
	//Flower manager have all the data of flower
	private FlowerManager flowerManager;
	//bird Rigidbody
	new Rigidbody rigidbody;
	public float NectarObtained { get; private set; }
	//Save the nearest Flower
	private Flower nearestFlower;

	//Smoother yaw and pitch change
	private float smoothPitchChange = 0f;
	private float smoothYawChange = 0f;

	//Max value for pitch
	private const float MaxPitchAngle = 80f;
	//Max distance from the beak tip to accept nectar collision
	private const float BeakTipRadius = 0.008f;
	//Whether the agent is frozen(intentionally not flying)
	private bool frozen = false;
	
	
	
	public override void Initialize()
	{
		//不在游戏中的时候，鸟在游戏结束的时候停止运动
		if (!trainingMode)
		{
			MaxStep = 0;
		}

		rigidbody = GetComponent<Rigidbody>();
		flowerManager = GetComponentInParent<FlowerManager>();
	}

	public override void OnEpisodeBegin()
	{
		if (trainingMode)
		{
			//in game mode, game manager control reset flower
			//reset rotation, position, fill up all necters
			flowerManager.ResetFlowers();
		}

		NectarObtained = 0;
		//zero out velocities so that movement stops before a new episode begins
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		
		//Default to spawning in front of a flower
		bool infrontOfFlower = true;
		if (trainingMode)
		{
			//Spawn in front of flower 50% of the time during training
			infrontOfFlower = UnityEngine.Random.value > .5f;
		}
		//Move the agent to a new random position
		MoveToSafeRandomPosition(infrontOfFlower);
		//Recalculate the nearest flower now that the agent has moved
		UpdateNearestFlower();
	}
	
	public override void CollectObservations(VectorSensor sensor)
	{
		//if there is no nearest flower, give an empty value
		if (nearestFlower == null)
		{
			sensor.AddObservation(new float[10]);
			return;
		}
		
		//rotation of the bird, 4
		sensor.AddObservation(transform.localRotation.normalized);
		//direction bird to nearest flower, 3 + 4 = 7
		Vector3 toFlower = nearestFlower.FlowerCenterPosition - beakTip.position;
		sensor.AddObservation(toFlower.normalized);
		//a dot product beak tip in the front of the flower, 7 + 1 = 8
		sensor.AddObservation(Vector3.Dot(toFlower.normalized, -nearestFlower.FlowerUpVector.normalized));
		//whether the beak is pointing toward the flower, 8 + 1 = 9
		sensor.AddObservation(Vector3.Dot(beakTip.forward.normalized, -nearestFlower.FlowerUpVector.normalized));
		//Relative distance from the beak tip to the flower, 9 + 1 = 10
		sensor.AddObservation(toFlower.magnitude / FlowerManager.AreaDiameter);
	}
	public override void OnActionReceived(ActionBuffers actions)
	{
		if (frozen) return;

		Vector3 move = new Vector3(actions.ContinuousActions[0], actions.ContinuousActions[1],
			actions.ContinuousActions[2]);
		rigidbody.AddForce(move * moveForce);

		Vector3 rotationVector = transform.rotation.eulerAngles;

		float pitchChange = actions.ContinuousActions[3];
		float yawChange = actions.ContinuousActions[4];

		smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.deltaTime);
		smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.deltaTime);

		float pitch = rotationVector.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
		if (pitch > 180f)
		{
			pitch -= 360f;
		}

		pitch = Mathf.Clamp(pitch, -MaxPitchAngle, MaxPitchAngle);
		float yaw = rotationVector.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;
		
		transform.rotation = Quaternion.Euler(pitch,yaw,0f);
	}

	//Move agent to a safe position, inside the boundary without touching racks / bushes
	//inFrontOfFlower to give randomize value for training, sometimes the bird will generate in front of the flower
	private void MoveToSafeRandomPosition(bool inFrontOfFlower)
	{
		bool safePositionFound = false;
		// avoid infinite loop
		int attemptRemaining = 100;
		Vector3 potentialPosition = Vector3.zero;
		Quaternion potentialRotation = new Quaternion();

		while (!safePositionFound && attemptRemaining > 0)
		{
			attemptRemaining--;
			if (inFrontOfFlower)
			{
				//Pick a random flower in Flower Manager
				Flower randomFlower =
					flowerManager.Flowers[UnityEngine.Random.Range(0, flowerManager.Flowers.Count)];
				//Position 10 to 20 cm in front of the flower
				float distanceFromFlower = UnityEngine.Random.Range(.1f, .2f);
				potentialPosition = randomFlower.transform.position + randomFlower.FlowerUpVector;
				
				//Point beak at flower (bird's head is center of transform)
				Vector3 toFlower = randomFlower.FlowerCenterPosition - potentialPosition;
				potentialRotation = Quaternion.LookRotation(toFlower, Vector3.up);
			}
			else
			{
				//Random height from the ground
				float height = UnityEngine.Random.Range(1.2f, 2.5f);
				float radius = UnityEngine.Random.Range(2f, 7f);
				
				//Pick random direction rotated around y axis
				Quaternion direction = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
				
				//Combine height, radius, direction
				potentialPosition = flowerManager.transform.position 
				                    // above the ground
				                    + Vector3.up * height 
				                    // somewhere 2-7 away from the island center
				                    + direction * Vector3.forward * radius;
				
				//Random Rotation
				float pitch = UnityEngine.Random.Range(-60f, 60f);
				float yaw = UnityEngine.Random.Range(-180f, 180f);
				potentialRotation = Quaternion.Euler(pitch, yaw, 0f);
			}
			
			//Check if the agent too close to anything
			Collider[] colliders = Physics.OverlapSphere(potentialPosition, 0.05f);

			safePositionFound = colliders.Length == 0;
		}

		transform.position = potentialPosition;
		transform.rotation = potentialRotation;
	}
	
	//Update nearest flower to the agent
	private void UpdateNearestFlower()
	{
		foreach (var flower in flowerManager.Flowers)
		{
			//currently the bird does not has any nearest flower registered and the flower that looped through has nectar
			if (nearestFlower == null && flower.HasNectar)
			{
				nearestFlower = flower;
			}
			else if(flower.HasNectar)
			{
				//Calculate distance to this flower and distance to the current nearest flower
				float distanceToFlower = Vector3.Distance(flower.transform.position, beakTip.position);
				float distanceToCurrentNearestFlower =
					Vector3.Distance(nearestFlower.transform.position, beakTip.position);
				
				//if nearestflower is empty of nectar OR the flower looped through is closer, use the new flower as the nearest flower
				if (!nearestFlower.HasNectar || distanceToFlower < distanceToCurrentNearestFlower)
				{
					nearestFlower = flower;
				}
			}
		}
	}

	//For gameplay, freeze the agent in Menu
	//Controlled by GM
	public void FreezeAgent()
	{
		Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training Mode");
		frozen = true;
		rigidbody.Sleep();
	}

	public void UnfreezeAgent()
	{
		Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training Mode");
		frozen = false;
		rigidbody.WakeUp();
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		Vector3 forward = Vector3.zero;
		Vector3 left = Vector3.zero;
		Vector3 up = Vector3.zero;
		float pitch = 0f;
		float yaw = 0f;
		
		//convert keyboard inputs to movement / turning
		//Constraint all the value from -1 to 1

		// Forward / Backword
		if (Input.GetKey(KeyCode.W)) forward = transform.forward;
		else if (Input.GetKey(KeyCode.S)) forward = -transform.forward;
		
		// Left / Right
		if (Input.GetKey(KeyCode.A)) left = - transform.right;
		else if (Input.GetKey(KeyCode.D)) left = transform.right;
		
		// Up / Down
		if (Input.GetKey(KeyCode.E)) up = transform.up;
		else if (Input.GetKey(KeyCode.C)) up = transform.up;
		
		// pitch up/ down
		if (Input.GetKey(KeyCode.UpArrow)) pitch = 1;
		else if (Input.GetKey(KeyCode.DownArrow)) pitch = -1;
		
		// pitch up/ down
		if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1;
		else if (Input.GetKey(KeyCode.RightArrow)) yaw = 1;

		//combine all directions action player just pressed
		Vector3 combine = (forward + left + up).normalized;

		//add the 3 movement values, pitch and yaw to the actionout array to override it;
		var continiousActionsOut = actionsOut.ContinuousActions;
		continiousActionsOut[0] = combine.x;
		continiousActionsOut[1] = combine.y;
		continiousActionsOut[2] = combine.z;
		continiousActionsOut[3] = pitch;
		continiousActionsOut[4] = yaw;
	}

	private void OnTriggerEnter(Collider other)
	{
		TriggerEnterOrStay(other);
	}

	private void OnTriggerStay(Collider other)
	{
		TriggerEnterOrStay(other);
	}

	//Both execute TriggerEnterOrStay when trigger enter and stay
	private void TriggerEnterOrStay(Collider collider)
	{
		if (collider.CompareTag("nectar"))
		{
			Vector3 closetPointToBeakTip = collider.ClosestPoint(beakTip.position);

			//check if the closet collision point is close to the beak tip
			if (Vector3.Distance(beakTip.position, closetPointToBeakTip) < BeakTipRadius)
			{
				Flower flower = flowerManager.GetFlowerFromNectar(collider);
				float nectarReceived = flower.Feed(.01f);

				NectarObtained += nectarReceived;

				if (trainingMode)
				{
					//点积？？Dot
					//If the bird actually facing the same direction to the flower
					float bonus = .02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized,
						-nearestFlower.FlowerUpVector.normalized));
					AddReward(.01f + bonus);
				}

				if (!flower.HasNectar)
				{
					UpdateNearestFlower();
				}
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (trainingMode && collision.collider.CompareTag("boundary"))
		{
			AddReward(-.5f);
		}
	}

	private void Update()
	{
		if (nearestFlower != null)
		{
			Debug.DrawLine(beakTip.position, nearestFlower.FlowerCenterPosition, Color.green);
		}
	}

	private void FixedUpdate()
	{
		//Avoid scenario where nearest flower nectar is stolen by opponent and not updated
		if (nearestFlower != null && !nearestFlower.HasNectar)
		{
			UpdateNearestFlower();
		}
	}
}
