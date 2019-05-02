﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emergency : MonoBehaviour
{
    public enum E_Severity { Light, Medium, Severe };
    public enum E_Type { Medical, Disaster, Both }

    private E_Severity Severity;
    private E_Type Type;
    public int NPeopleEvolved;
    private UrbanArea MyArea;
    private float Duration;
    private float SalvationProb;
    public int NAmbulances;
    
    public void InitEmergency(E_Severity severity, E_Type type, int peopleInvolved, UrbanArea area)
    {
        this.Severity = severity;
        this.Type = type;
        this.NPeopleEvolved = peopleInvolved;
        this.MyArea = area;
        this.Duration = 0;
        this.NAmbulances = -1;

        if (severity == E_Severity.Light)
        {
            SalvationProb = 0.99f;
        } 

        else if (severity == E_Severity.Medium)
        {
            SalvationProb = 0.70f;
        }
        else
        {
            SalvationProb = 0.50f;
        }

        //notify central that i exist
    }

    public float GetEmergencyDuration()
    {
        return Duration;
    }

    public float GetEmergencySalvationProbability()
    {
        return SalvationProb;
    }


    public int GetEmergencyPeopleEnvolved()
    {
        return this.NPeopleEvolved;
    }

    public void SetEmergencyPeopleEnvolved(int n)
    {
        this.NPeopleEvolved = n;
    }

    public E_Severity GetEmergencySeverity()
    {
        return this.Severity;
    }

    public void SetEmergencySeverity(E_Severity severity)
    {
        this.Severity = severity;
    }

    public E_Type GetEmergencyType()
    {
        return this.Type;
    }

    public Vector3 GetEmergencyPosition()
    {
        return this.gameObject.transform.localPosition;
    }

    public void SendAmbulance(int ambulances)
    {
        this.NAmbulances = ambulances;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Duration += Time.deltaTime;
        if (this.NAmbulances == 0)
        {
            if (NPeopleEvolved < 1) // && this.Type == E_Type.Medical)
            {
                //notify area + central
                MyArea.RemoveEmergency(this);
                Destroy(this.gameObject);
            }
            else
            {
                this.NAmbulances = -1;
                MyArea.ReOpenEmergency(this);
            }
        }

        //TODO: Make people die
        //TODO: Incendios/Desastres
    }
}
