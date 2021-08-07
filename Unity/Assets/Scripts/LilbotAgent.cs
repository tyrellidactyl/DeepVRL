using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class LilbotAgent : Agent
{
    Rigidbody rBody;
    public Transform Target;
    public float acceleration = 1;
    public float maxSpeed = 1;
    private bool collided = false;  

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Obstacle")
        {
            collided = true;
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Collision Detected, Stopping Test");
            //Debug.Log(collided);
        }
    }

    public override void OnEpisodeBegin()
    {

        // Reset agent position
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3(3.0f, 0.125f, 0);
        this.transform.rotation = new Quaternion(0, 0, 0, 0);
        
        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.0f,
                                           Random.value * 8 - 4);

        // Reset collision property
        collided = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        //rBody.AddForce(controlSignal * speed);

        //Vector3 movement = directionsForKeys[i] * acceleration * Time.deltaTime;
        transform.Translate(controlSignal * maxSpeed);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // If agent reached target
        if (distanceToTarget < 0.5f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // If agent exceeded domain boundaries
        if (distanceToTarget > 20.0f)
        {
            Debug.Log("Domain Boundary Exceeded, Stopping Test");
            EndEpisode();
        }

        // If collision occurred
        if (collided == true)
        {
            EndEpisode();
        }

        // If agent fell off platform
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }

    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

}
