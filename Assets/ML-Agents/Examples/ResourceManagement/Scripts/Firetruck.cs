﻿using System;
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
    private bool done = false;
    private bool onDestination = false;
    private float waitTime = 0;
    static public int damage = 3;
    static public float waterDeposit = 30;
    private float curWaterDeposit = 30;

    public void SendFiretruck(Emergency em)
    {
        myEmergency = em;
    }

    void Start()
    {
        curWaterDeposit = waterDeposit;
    }

    void Update()
    {
        if (!done && !onDestination)
        {
            Move(myEmergency.gameObject);
        }
        else if (!done)
        {
            rb.velocity = Vector3.zero;
            TreatEmergency();
        }
        else if (!onDestination)
        {
            Move(myERC.gameObject);
        }
        else
        {
            myERC.ReturnFiretruck();
            Destroy(this.gameObject);
        }
    }

    private void TreatEmergency()
    {

        if (curWaterDeposit <= 0)
        {
            done = true;
            onDestination = false;
            myEmergency.NFiretrucks -= 1;
            return;
        }

        if (myEmergency.GetEmergencyDisasterLife() <= 0)
        {
            done = true;
            onDestination = false;
            myEmergency.NFiretrucks -= 1;
            return;
        }


        if (waitTime >= myEmergency.WaitTime)
        {
            if (myEmergency.TreatEmergency())
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
            onDestination = true;
            return;
        }

        Vector3 movement = target.transform.position - gameObject.transform.position;

        rb.velocity = movement * speed;

        //rb.position = new Vector3
        //(
        //    Mathf.Clamp(rb.position.x, myERC.myArea.transform.position.x - boundary.xMin, myERC.myArea.transform.position.x + boundary.xMax),
        //    1.0f,
        //    Mathf.Clamp(rb.position.z, myERC.myArea.transform.position.z - boundary.zMin, myERC.myArea.transform.position.z + boundary.zMax)
        //);
    }

}
