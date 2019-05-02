using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

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
    private float epson = 3f;
    private int peopleToTransport = 0;
    private bool done = false;
    private bool onDestination = false;
    private float waitTime = 0;

    public void SendAmbulance(Emergency em)
    {
        myEmergency = em;
    }

    void Update()
    {
        if(!done && !onDestination)
        {
            Move(myEmergency.gameObject);
        }
        else if(!done)
        {
            rb.velocity = Vector3.zero;
            TreatEmergency();
        }
        else if(!onDestination)
        {
            Move(myERC.gameObject);
        }
        else
        {
            myERC.ReturnAmbulance();
            Destroy(this.gameObject);
        }
    }

    private void TreatEmergency()
    {

        if (peopleToTransport == maxPeople)
        {
            done = true;
            onDestination = false;
            myEmergency.NAmbulances -= 1;
            return;
        }

        if (myEmergency.GetEmergencyPeopleEnvolved() < 1)
        {
            done = true;
            onDestination = false;
            myEmergency.NAmbulances -= 1;
            return;
        }


        if (waitTime >= 1)
        {
            float salvationProb = UnityEngine.Random.Range(0, 1);
            if (salvationProb <= myEmergency.GetEmergencySalvationProbability())
            {
                myEmergency.NPeopleEvolved -= 1;
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
        if (Vector3.Distance(target.transform.localPosition, gameObject.transform.localPosition) < epson)
        {
            onDestination = true;
            return;
        }

        Vector3 movement = target.transform.localPosition - gameObject.transform.localPosition;

        rb.velocity = movement * speed;

        rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            1.0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );
    }

}
