using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{

    // generic
    GameObject fork;
    float time = 0;
    float controlLoopPeriod = 0.2f; // update control every ... second
    VehicleData vehicleData;

    // drive
    Vector3 targetPosition;
    float velocity;
    float maxVelocity = 3.0f;
    float acceleration = 1.0f;
    float deceleration = 2.0f;

    // steer
    Vector3 targetRotation;
    float angularVelocity;
    float maxAngularVelocity = 15.0f;
    float angularAcceleration = 10.0f;

    // fork
    float targetForkPosition;
    Vector3 initialForkPosition;
    float maxForkVelocity = 0.5f;
    float maxForkPosition = 2.0f;

    void Start()
    {
        vehicleData = new VehicleData();
        velocity = 0.0f;
        angularVelocity = 0.0f;

        // fork
        fork = transform.Find("fork").gameObject;
        initialForkPosition = fork.transform.localPosition;
        targetForkPosition = 0.0f;

        //StartCoroutine(UpdateVehicleState());
    }

    void Update()
    {

        time += Time.deltaTime;
        if (time > controlLoopPeriod)
        {
            time = 0;

            // update velocity
            Vector3 vehicleForward = -gameObject.transform.right;
            if (Input.GetKey(KeyCode.W))
            {
                velocity += acceleration;
                if (velocity > maxVelocity)
                {
                    velocity = maxVelocity;
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                velocity -= acceleration;
                if (velocity < -maxVelocity)
                {
                    velocity = -maxVelocity;
                }
            }
            else
            {
                // decelerate to zero
                if (velocity > deceleration)
                {
                    velocity -= deceleration;
                }
                else if (velocity < -deceleration)
                {
                    velocity += deceleration;
                }
                else
                {
                    velocity = 0.0f;
                }
            }
            targetPosition += vehicleForward * velocity;


            // update angular velocity
            Vector3 vehicleRotation = gameObject.transform.rotation.eulerAngles;
            if (Input.GetKey(KeyCode.D))
            {
                angularVelocity += angularAcceleration;
                if (angularVelocity > maxAngularVelocity)
                {
                    angularVelocity = maxAngularVelocity;
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                angularVelocity -= angularAcceleration;
                if (angularVelocity < -maxAngularVelocity)
                {
                    angularVelocity = -maxAngularVelocity;
                }
            }
            else
            {
                // decelerate to zero
                if (angularVelocity > angularAcceleration)
                {
                    angularVelocity -= angularAcceleration;
                }
                else if (angularVelocity < -angularAcceleration)
                {
                    angularVelocity += angularAcceleration;
                }
                else
                {
                    angularVelocity = 0.0f;
                }
            }
            targetRotation += gameObject.transform.up * angularVelocity;

            // fork
            if (Input.GetKey(KeyCode.O))
            {
                targetForkPosition += maxForkVelocity;
            }
            else if (Input.GetKey(KeyCode.K))
            {
                targetForkPosition -= maxForkVelocity;
            }
            if (targetForkPosition > maxForkPosition)
            {
                targetForkPosition = maxForkPosition;
            }
            else if (targetForkPosition < 0) 
            {
                targetForkPosition = 0;
            }

            //Debug.Log($"Velocity= {velocity}, Steering= {angularVelocity}, Fork= {targetForkPosition}");
        }

        // vehicle motion
        Vector3 currentPosition = gameObject.transform.position;
        gameObject.transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime);
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime);

        // fork motion
        Vector3 forkP = initialForkPosition - fork.transform.InverseTransformDirection(transform.forward) * targetForkPosition;
        fork.transform.localPosition = Vector3.Lerp(fork.transform.localPosition, forkP, Time.deltaTime);

        // serialize
        vehicleData.Set(transform.position, transform.rotation.eulerAngles, targetForkPosition);


    }

    IEnumerator UpdateVehicleState()
    {
        for (; ; )
        {
            string msg = JsonUtility.ToJson(vehicleData);
            Connector.SendMessage(msg);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
