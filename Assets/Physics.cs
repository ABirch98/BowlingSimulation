using System.Collections;
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
    private bool collidedWithFloor = false;
    private bool collidedWithPins = false;
    private float Pindistance;
    private float FloorDistance;
    private float MaxPinDistance;
    private float MaxFloorDistance;
    private float ballRadius;
    private float PinXPosition;
    private float pinRadius;
    private float AngleB;
    private float AngleC;
    const float mass = 1;
    const float FrictionCoefficient = 0.02f;
    private Vector3 BallForce;
    private Vector3 A;
    private Vector3 B;
    private float InitialXForce = -2.0f;
    private float momentum;
    private float CentripetalAcceleration;
    private float KineticFriction;
    private Vector3 Ballacceleration;
    private float Gravity = -9.81f;
    private Vector3 VectorVelocity;
    private float PinDistance;
    private float collisionTime = 0;
    private float currentTime = 0;
    private float OldTime = 0;
    private float dt = 1;
    // Start is called before the first frame update
    void Start()
    {

        //initialise some varaiables on startup;
        MaxPinDistance = 1;
        MaxFloorDistance = (ballRadius + Alley.transform.localScale.y/2);
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            currentTime = Time.realtimeSinceStartup;
            dt = currentTime - OldTime;
            OldTime = currentTime;
            //apply forces
            //update positions and velocities
            calcBallInitForce();
            calcNewAcceleration();
            calcVelocity();
            checkPositions(); //check for collisions
                              //solve constraints 
                              //update on screen
            if (!collidedWithFloor)
            {
                Ball.transform.position += new Vector3(0.0f, VectorVelocity.y * dt, 0.0f);
            }
            Ball.transform.position += new Vector3(VectorVelocity.x * dt, 0.0f, 0.0f);
            if (collidedWithFloor && !collidedWithPins)
            {
                VectorVelocity.y = 0.0f;
                GameCamera.transform.position += new Vector3(VectorVelocity.x * dt, 0.0f, 0.0f);

                if (!Roll.isPlaying)
                {
                    Roll.Play();
                }

            }
            if (collidedWithPins)
            {
                // CalculateCollisionAngle(PinXdistance);
            }

            currentTime += dt;

        }
    }

  
    void calcBallInitForce()
    {
        BallForce = (new Vector3(InitialXForce, 0, 0));
    }

    void calcNewAcceleration()
    {
        Ballacceleration = new Vector3(BallForce.x / mass, BallForce.y / mass, BallForce.z / mass);
    }

    void calcVelocity()
    {
        VectorVelocity.x += Ballacceleration.x * dt;
        VectorVelocity.y += Ballacceleration.y * dt;
        VectorVelocity.z += Ballacceleration.z * dt;
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
        //FloorDistance = Mathf.Abs(Alley.transform.position.y - Ball.transform.position.y);
            collidedWithFloor = true;
            print(FloorDistance);
            print("collidedWithFloor");

        if (Ball.transform.position.x < (Alley.transform.position.x -(Alley.transform.localScale.x/2)))
        { 
            collidedWithFloor = false;
        }

        foreach (GameObject pin in Pins)
        {
            Pindistance = Mathf.Abs(Vector3.Distance(Ball.transform.position, pin.transform.position));
            if (Pindistance <= MaxPinDistance)
            { 
                //fix overlaps
                float DistanceA = Mathf.Sqrt((Ball.transform.position.x - pin.transform.position.x)*(Ball.transform.position.x - pin.transform.position.x)
                    + (Ball.transform.position.z - pin.transform.position.z) * (Ball.transform.position.z - pin.transform.position.z));
                collidedWithPins = true;
                float DistanceOV = 0.5f * (DistanceA - 1.0f);
                //tranform by half the overlap in the vector direction of the collision
                Ball.transform.position -= new Vector3(DistanceOV * (Ball.transform.position.x - pin.transform.position.x) / DistanceA, 0, DistanceOV * (Ball.transform.position.z - pin.transform.position.z) / DistanceA);
                pin.transform.position += new Vector3(DistanceOV * (Ball.transform.position.x - pin.transform.position.x) / DistanceA, 0, DistanceOV * (Ball.transform.position.z - pin.transform.position.z) / DistanceA);
                
                collisionTime = currentTime;
                CollidedPin = pin;
                print("collidedWithpin");
                Roll.Stop();
                if (!Strike.isPlaying)
                {
                   Strike.Play();
                }

            }

        }
            
    }
}
