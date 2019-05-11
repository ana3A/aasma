using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Firetruck : Resource
{
    public float speed;
    public float maxSpeed;
    public ERCAgent myERC;
    public Boundary boundary;
    public Rigidbody rb;
    public float maxWaitTime = 5;
    private Emergency myEmergency;
    private float epson = 3f;
    private float waitTime = 0;
    static public int damage = 3;
    static public float waterDeposit = 30;
    private float curWaterDeposit = 30;

    //States
    public bool goingToEmergency;
    public bool onEmergency;
    public bool goingToERC;
    public bool returnedToERC;
    public bool free;

    public void SendFiretruck(Emergency em)
    {
        myEmergency = em;
        goingToEmergency = true;
        returnedToERC = false;
        free = false;
    }

    void Start()
    {
        curWaterDeposit = waterDeposit;
        returnedToERC = true;
        onEmergency = false;
        goingToEmergency = false;
        goingToERC = false;
        free = true;
    }

    void Update()
    {
        if (!free)
        {
            if (goingToEmergency)
            {
                Move(myEmergency.gameObject);
            }

            else if (onEmergency)
            {
                rb.velocity = Vector3.zero;
                TreatEmergency();
            }

            else if (goingToERC)
            {
                Move(myERC.gameObject);
            }

            else if (returnedToERC)
            {
                myERC.ReturnFiretruck(this);
                free = true;
            }
        }
    }

    private void TreatEmergency()
    {

        if (curWaterDeposit <= 0)
        {
            onEmergency = false;
            goingToERC = true;
            myEmergency.NFiretrucks -= 1;
            return;
        }

        if (myEmergency.GetEmergencyDisasterLife() <= 0)
        {
            onEmergency = false;
            goingToERC = true;
            myEmergency.NFiretrucks -= 1;
            return;
        }


        if (waitTime >= myEmergency.WaitTime)
        {
            if (myEmergency.TreatEmergency(this))
            {
                curWaterDeposit -= damage;
            }
            waitTime = 0;
        }

        else
        {
            waitTime += Time.deltaTime;
        }

    }

    private void Move(GameObject target)
    {
        if (Vector3.Distance(target.transform.position, gameObject.transform.position) < epson)
        {
            if (goingToEmergency)
            {
                goingToEmergency = false;
                onEmergency = true;
            }
            else
            {
                goingToERC = false;
                returnedToERC = true;
            }
            return;
        }

        Vector3 movement = target.transform.position - gameObject.transform.position;

        rb.velocity = movement * speed;

        //rb.position = new Vector3
        //(
        //    Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
        //    1.0f,
        //    Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        //);
    }

}
