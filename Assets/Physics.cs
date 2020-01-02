using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float InitialXForce = -8.0f;
    private float momentum;
    private float CentripetalAcceleration;
    private float KineticFriction;
    private Vector3 Ballacceleration;
    private float Gravity = -9.81f;
    private Vector3 VectorVelocity;
    private float PinDistance;
    private float currentTime = 0;
    private float OldTime = 0;
    private float dt = 1;
    private int ID = 0;
    // Start is called before the first frame update
    void Start()
    {

        //initialise some varaiables on startup;
        MaxPinDistance = 1;
        MaxFloorDistance = (ballRadius + Alley.transform.localScale.y/2);
        Ball.GetComponent<Pin_Properties>().Mass = 1;
        foreach(GameObject pin in Pins)
        {
            pin.GetComponent<Pin_Properties>().LocalID = ID;
            ID++;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            currentTime = Time.realtimeSinceStartup;
            dt = currentTime - OldTime;
            OldTime = currentTime;
            if (Input.GetMouseButton(0))
            {
                Ball.GetComponent<Pin_Properties>().Velocity.x += -1.0f;
               
            }
                //apply forces
                //update positions and velocities
            foreach (GameObject pin in Pins)
            {

                calcVelocity(pin);
                calcAcceleration(pin);
                Move(pin);
                //if velocity is close to 0 stop moving(fixes floating point errors) TODO: Change to max static friction
                if(((pin.GetComponent<Pin_Properties>().Velocity.x)*(pin.GetComponent<Pin_Properties>().Velocity.x) + (pin.GetComponent<Pin_Properties>().Velocity.y) * (pin.GetComponent<Pin_Properties>().Velocity.y)) < 0.05f)
                {
                    pin.GetComponent<Pin_Properties>().Velocity = new Vector2(0.0f, 0.0f);
                }
            }
            checkPositions(); //check for collisions
                              //solve constraints 
                              //update on screen

            //TODO: gravity 
            //if (!collidedWithFloor)
            //{
            //    Ball.transform.position += new Vector3(0.0f, VectorVelocity.y * dt, 0.0f);
            //}
            //Ball.transform.position += new Vector3((Ball.GetComponent<Pin_Properties>().Velocity.x * dt), 0.0f, 0.0f);
            if (!collidedWithPins)
            {
                GameCamera.transform.position += new Vector3(Ball.GetComponent<Pin_Properties>().Velocity.x * dt, 0.0f, 0.0f);

                if (!Roll.isPlaying)
                {
                    //Roll.Play();
                }

            }

            currentTime += dt;

        }
    }

  
    void calcBallInitForce()
    {
        Ball.GetComponent<Pin_Properties>().Force = (new Vector3(InitialXForce, 0, 0));
    }
    void calcVelocity(GameObject ThisObject)
    {
        ThisObject.GetComponent<Pin_Properties>().Velocity.x += ThisObject.GetComponent<Pin_Properties>().acceleration.x * dt;
        ThisObject.GetComponent<Pin_Properties>().Velocity.y += ThisObject.GetComponent<Pin_Properties>().acceleration.y * dt;
        //ThisObject.GetComponent<Pin_Properties>().Velocity.z += ThisObject.GetComponent<Pin_Properties>().acceleration.z * dt;
    }
    void calcAcceleration(GameObject ThisObject)
    {
        ThisObject.GetComponent<Pin_Properties>().acceleration.x = -(ThisObject.GetComponent<Pin_Properties>().Velocity.x * 0.8f);
        ThisObject.GetComponent<Pin_Properties>().acceleration.y = -(ThisObject.GetComponent<Pin_Properties>().Velocity.y * 0.8f);
        //ThisObject.GetComponent<Pin_Properties>().acceleration.z = ThisObject.GetComponent<Pin_Properties>().Force.z / mass;
    }
    void calcAccelerationDueToForce()
    {
        Ball.GetComponent<Pin_Properties>().acceleration.x = (Ball.GetComponent<Pin_Properties>().Force.x / mass);
        Ball.GetComponent<Pin_Properties>().acceleration.y = (Ball.GetComponent<Pin_Properties>().Force.y / mass);
    }
    void Move(GameObject ThisObject)
    {
        ThisObject.transform.position += new Vector3((ThisObject.GetComponent<Pin_Properties>().Velocity.x * dt), 0.0f, 0.0f);
        ThisObject.transform.position += new Vector3(0.0f, 0.0f, (ThisObject.GetComponent<Pin_Properties>().Velocity.y * dt));
        //ThisObject.transform.position += new Vector3(0.0f, 0.0f, (ThisObject.GetComponent<Pin_Properties>().Velocity.z * dt));
    }

    public void checkPositions()
    {
        //FloorDistance = Mathf.Abs(Alley.transform.position.y - Ball.transform.position.y);
            collidedWithFloor = true;

        if (Ball.transform.position.x < (Alley.transform.position.x -(Alley.transform.localScale.x/2)))
        { 
            collidedWithFloor = false;
        }

        foreach (GameObject pin in Pins)
        {
            foreach(GameObject Other in Pins)
        {
            if (pin.GetComponent<Pin_Properties>().LocalID != Other.GetComponent<Pin_Properties>().LocalID)
            {
                Pindistance = Mathf.Abs(Vector3.Distance(pin.transform.position, Other.transform.position));
                if (Pindistance <= MaxPinDistance)
                {
                    //fix overlaps
                        
                    float DistanceA = Mathf.Sqrt((pin.transform.position.x - Other.transform.position.x) * (pin.transform.position.x - Other.transform.position.x)
                        + (pin.transform.position.z - Other.transform.position.z) * (pin.transform.position.z - Other.transform.position.z));
                    CalcDynamic(pin, Other, DistanceA);
                    collidedWithPins = true;
                    float DistanceOV = 0.5f * (DistanceA - 1.0f);
                    //tranform by half the overlap in the vector direction of the collision
                    pin.transform.position -= new Vector3(DistanceOV * (pin.transform.position.x - Other.transform.position.x) / DistanceA, 0, DistanceOV * (pin.transform.position.z - Other.transform.position.z) / DistanceA);
                    Other.transform.position += new Vector3(DistanceOV * (pin.transform.position.x - Other.transform.position.x) / DistanceA, 0, DistanceOV * (pin.transform.position.z - Other.transform.position.z) / DistanceA);
                    CollidedPin = pin;
                    print("collidedWithpin");
                    Roll.Stop();
                    if (!Strike.isPlaying && CollidedPin.GetComponent<Pin_Properties>().LocalID == 10)
                    {
                        Strike.Play();
                    }
                       
                        
                }
               

            }
            }
           

        }
            
    }
    void CalcDynamic(GameObject p1, GameObject p2, float DistanceB)
    {
        //Normal Reaction
        float Normalx = (p2.transform.position.x - p1.transform.position.x) / DistanceB;
        float Normaly = (p2.transform.position.z - p1.transform.position.z) / DistanceB;

        //Tangent Reaction
        float Tangentx = -Normaly;
        float Tangenty = Normalx;

        //Tangental Dot Product
        float TDP1 = p1.GetComponent<Pin_Properties>().Velocity.x * Tangentx + p1.GetComponent<Pin_Properties>().Velocity.y * Tangenty;
        float TDP2 = p2.GetComponent<Pin_Properties>().Velocity.x * Tangentx + p2.GetComponent<Pin_Properties>().Velocity.y * Tangenty;

        float NDP1 = p1.GetComponent<Pin_Properties>().Velocity.x * Normalx + p1.GetComponent<Pin_Properties>().Velocity.y * Normaly;
        float NDP2 = p2.GetComponent<Pin_Properties>().Velocity.x * Normalx + p2.GetComponent<Pin_Properties>().Velocity.y * Normalx;

        //Conservation of Momentum for elastic collision
        float Momentum1 = (NDP1 * (p1.GetComponent<Pin_Properties>().Mass - p2.GetComponent<Pin_Properties>().Mass) + 2.0f * p2.GetComponent<Pin_Properties>().Mass * NDP2) / p1.GetComponent<Pin_Properties>().Mass * p2.GetComponent<Pin_Properties>().Mass;
        float Momentum2 = (NDP2 * (p2.GetComponent<Pin_Properties>().Mass - p1.GetComponent<Pin_Properties>().Mass) + 2.0f * p1.GetComponent<Pin_Properties>().Mass * NDP1) / p1.GetComponent<Pin_Properties>().Mass * p2.GetComponent<Pin_Properties>().Mass;
        //Update Velocities
        p1.GetComponent<Pin_Properties>().Velocity.x = Tangentx * TDP1 + Normalx * Momentum1;
        p1.GetComponent<Pin_Properties>().Velocity.y = Tangenty * TDP1 + Normaly * Momentum1;
        p2.GetComponent<Pin_Properties>().Velocity.x = Tangentx * TDP2 + Normalx * Momentum2;
        p2.GetComponent<Pin_Properties>().Velocity.y = Tangenty * TDP2 + Normaly * Momentum2;
    }

}
