using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emergency : MonoBehaviour
{
    public enum E_Severity { Light, Medium, Severe };
    protected E_Severity Severity;
    protected UrbanArea MyArea;
    protected float Duration;
    public int NAmbulances;
    public int NFiretrucks;
    protected Material MyMaterial;
    public float LightWaitTime;
    public float MediumWaitTime;
    public float SevereWaitTime;
    public float WaitTime;

    public void Start()
    {

    }

    public void Update()
    {

    }

    public void InitEmergency(UrbanArea area, E_Severity severity, Material material)
    {
        this.MyArea = area;
        this.Duration = 0;
        this.NAmbulances = -1;
        this.NFiretrucks = -1;
        this.Severity = severity;
        this.MyMaterial = material;
        ChangeWaitTime();
    }

    public E_Severity GetEmergencySeverity()
    {
        return this.Severity;
    }

    public void SetEmergencySeverity(E_Severity severity)
    {
        this.Severity = severity;
    }

    public void ChangeWaitTime()
    {
        if (Severity == E_Severity.Light)
        {
            WaitTime = LightWaitTime;
        }
        else if (Severity == E_Severity.Medium)
        {
            WaitTime = MediumWaitTime;
        }
        else
        {
            WaitTime = SevereWaitTime;
        }
    }

    public float GetEmergencyDuration()
    {
        return Duration;
    }

    public virtual void ChangeSeverity()
    {

    }

    public virtual bool TreatEmergency(Ambulance a)
    {
        return false;
    }

    public virtual bool TreatEmergency(Firetruck f)
    {
        return false;
    }

    public virtual void CheckIfDead()
    {

    }

    public virtual void NewSeverity(E_Severity severity)
    {

    }

    public virtual void ChangeSeverity(E_Severity severity)
    {

    }

    public virtual void SendResources(int ambulances, int firetrucks)
    {

    }

    public virtual int GetEmergencyPeopleEnvolved()
    {
        return 0;
    }

    public virtual float GetEmergencyDisasterLife()
    {
        return 0;
    }

    public void IncreaseSeverity()
    {
        //yield return new WaitForSeconds(0.1f);
        var em_prob = UnityEngine.Random.Range(0f, 1f);
        if (em_prob <= 0.0005)

        {
            if (this.Severity == E_Severity.Light)
            {
                ChangeSeverity(E_Severity.Medium);
            }

            else if (this.Severity == E_Severity.Medium)
            {
                ChangeSeverity(E_Severity.Severe);
            }

        }
    }
}
