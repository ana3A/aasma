using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambulance : Resource
{
    public float speed;
    public float maxSpeed;
    public ERCAgent myERC;
    static public int maxPeople = 2;
    public Boundary boundary;
    public Rigidbody rb;
    public float maxWaitTime = 5;
    private Emergency myEmergency;
    private float epsilon = 3f;
    private int peopleToTransport = 0;
    private float waitTime = 0;

    //States
    public bool goingToEmergency;
    public bool onEmergency;
    public bool goingToERC;
    public bool returnedToERC;
    public bool free;

    public bool Decentralized { get; set; }

    public void Start()
    {
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
            DealWithEmergency();
        }

        else if (Decentralized)
        {
            if (myERC.TheresMedicalEmergency())
            {
                AssumeEmergency();
            }
        }
    }

    public void SendAmbulance(Emergency em)
    {
        myEmergency = em;
        free = false;
        goingToEmergency = true;
    }

    public void AssumeEmergency()
    {
        SendAmbulance(myERC.WorstMedicalEmergency());
        myERC.SendAmbulance(this);
        myEmergency.SendResources(1, 0);

        if (myEmergency.NeededAmbulances() < 1)
        {
            myERC.MedicalEmergencyControlled(myEmergency);
        }
    }
    

    private void DealWithEmergency()
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
            ReturnAmbulance();
        }
    }

    private void ReturnAmbulance()
    {
        myERC.ReturnAmbulance(this);
        free = true;
        returnedToERC = false;
        peopleToTransport = 0;
    }

    private void TreatEmergency()
    {

        if (peopleToTransport == maxPeople)
        {
            onEmergency = false;
            goingToERC = true;
            myEmergency.NAmbulances -= 1;
            return;
        }

        if (myEmergency.GetEmergencyPeopleEnvolved() < 1)
        {
            onEmergency = false;
            goingToERC = true;
            myEmergency.NAmbulances -= 1;
            return;
        }


        if (waitTime >= myEmergency.WaitTime)
        {
            if(myEmergency.TreatEmergency(this))
            {
                peopleToTransport++;
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
        if (Vector3.Distance(target.transform.position, gameObject.transform.position) < epsilon)
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
