﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//https://www.toptal.com/game/video-game-physics-part-i-an-introduction-to-rigid-body-dynamics
public class Physics : MonoBehaviour
{
    public GameObject Ball;
    public GameObject[] Pins;
    public GameObject Alley;
    public Camera GameCamera;
    public AudioSource Strike;
    public AudioSource Roll;
    private GameObject CollidedPin;
    public bool collidedWithFloor = false;
    private bool collidedWithPins = false;
    public float PinXdistance;
    public float PinZdistance;
    public float FloorDistance;
    public float MaxPinDistance;
    public float MaxFloorDistance;
    public float ballRadius;
    public float PinXPosition;
    public float pinRadius;
    public float AngleB;
    public float AngleC;
    const float mass = 1;
    const float FrictionCoefficient = 0.02f;
    public Vector3 force;
    public float momentum;
    public float CentripetalAcceleration;
    public float KineticFriction;
    public Vector3 acceleration;
    private float Gravity = -9.81f;
    private float Velocity = -10.0f;
    private Vector3 VectorVelocity;
    float PinDistance;

    // Start is called before the first frame update
    void Start()
    {
        ballRadius = Ball.transform.localScale.x / 2;
        pinRadius = Pins[0].transform.localScale.x / 2;
        MaxPinDistance = ((ballRadius) + (pinRadius));
        MaxFloorDistance = (ballRadius + Alley.transform.localScale.y/2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        checkPositions();
        if (!collidedWithFloor)
        {
            Ball.transform.position += (new Vector3(0.0f, Gravity, 0.0f) * Time.deltaTime);
        }
        Ball.transform.position += (new Vector3(Velocity, 0.0f, 0.0f) * Time.deltaTime);
        if (collidedWithFloor && !collidedWithPins)
        {
            
            GameCamera.transform.position += (new Vector3(Velocity, 0.0f, 0.0f) * Time.deltaTime);
            
            if (!Roll.isPlaying)
            {
                Roll.Play();
            }

        }
        if(collidedWithPins)
        {
            Velocity = 0;
            CalculateCollisionAngle(PinXdistance);
        }
        else
        {
            ResolvePhysics();
        }
  
        


    }

    void calcForce()
    {
        force.x = mass * acceleration.x;
        force.y = mass * acceleration.y;
    }

    void calcNewAcceleration()
    {
        acceleration.x = force.x / mass;
        acceleration.y = force.y / mass;
    }

    void calcVelocity()
    {
        Velocity = /*DistanceSinceLastTick / */ Time.deltaTime;
    }

    void calcVelocityChange()
    {
        //VelocityChange = (force / mass) * Time.deltaTime;
    }
    void calcDistanceSinceLastTick()
    {

    }
    
    void calcFrictionK()
    {
        KineticFriction = FrictionCoefficient * force.x;
    }

    void CalcCentripetalAcceleration()
    {
        CentripetalAcceleration = (Mathf.Pow(Velocity, 2) /*/ radiusofpath*/);
    }


    void calcMomentum()
    {
        momentum = mass * Velocity;
    }

    void InitaliseForces()
    {
        calcForce();
        calcDistanceSinceLastTick();
        calcVelocity();
        calcMomentum();
    }


    void ResolvePhysics()
    {
        //update Position
    }


    void CalculateCollisionAngle(float distanceToPin)
    {
        float LengthB;
        float LengthC;
        float SinOfAngleB;
        float SinOfAngleC;
        LengthB = CollidedPin.transform.position.x - Ball.transform.position.x;
        LengthC = Ball.transform.position.z - CollidedPin.transform.position.z;
        SinOfAngleB = ((LengthB * 1)/ distanceToPin);
        SinOfAngleC = ((LengthC * 1) / distanceToPin);
        AngleB = Mathf.Asin(SinOfAngleB) * Mathf.Rad2Deg;
        AngleC = Mathf.Asin(SinOfAngleC) * Mathf.Rad2Deg;
 
    }

    public void checkPositions()
    {
        FloorDistance = (Alley.transform.position.y - Ball.transform.position.y);
        if (Mathf.Pow(FloorDistance,2.0f) < Mathf.Pow(MaxFloorDistance,2.0f))
        {
            collidedWithFloor = true;
            print(FloorDistance);
            print("collidedWithFloor");
        }
        if (Ball.transform.position.x < (Alley.transform.position.x -(Alley.transform.localScale.x/2)))
        { 
            collidedWithFloor = false;
        }

        foreach (GameObject pin in Pins)
        {
            PinXdistance = Mathf.Abs(Ball.transform.position.x - pin.transform.position.x);
            PinZdistance = (pin.transform.position.z - Ball.transform.position.z);
            if (PinXdistance <= MaxPinDistance)
            {
               /* if (PinXdistance!= MaxPinDistance)
                {
                   Ball.transform.position += new Vector3(0.00001f, 0, 0);
                }*/
                
                collidedWithPins = true;
                CollidedPin = pin;
                PinXPosition = CollidedPin.transform.position.x;
                // print(Pindistance);
                print("collidedWithpin");
                //pin.transform.position += new Vector3(20.0f, 0.0f, 0.0f);
                Roll.Stop();
                if (!Strike.isPlaying)
                {
                   Strike.Play();
                }

            }

        }
            
    }
}
