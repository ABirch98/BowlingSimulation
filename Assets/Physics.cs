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
    public bool collidedWithFloor = false;
    private bool collidedWithPins = false;
    public float PinXdistance;
    public float PinZdistance;
    public float FloorDistance;
    private float MaxPinDistance;
    public float MaxFloorDistance;
    public float ballRadius;
    public float pinRadius;
    private Vector3 Gravity = new Vector3(0.0f, -9.81f, 0.0f);
    private Vector3 Velocity = new Vector3(-10.0f, 0.0f, 0.0f);

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
            Ball.transform.position += Gravity * Time.deltaTime;
        }
        Ball.transform.position += Velocity * Time.deltaTime;
        if(collidedWithFloor && !collidedWithPins)
        {
            GameCamera.transform.position += Velocity * Time.deltaTime;
        }
       
    
        
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
            PinXdistance = Vector3.Distance(pin.transform.position, Ball.transform.position);
            PinZdistance = (pin.transform.position.z - Ball.transform.position.z);
            if (PinXdistance <= MaxPinDistance)
            {
                collidedWithPins = true;
                Strike.Play();
               // print(Pindistance);
                print("collidedWithpin");
                pin.transform.position += new Vector3(20.0f, 0.0f, 0.0f);

            }

        }
            
    }
}
