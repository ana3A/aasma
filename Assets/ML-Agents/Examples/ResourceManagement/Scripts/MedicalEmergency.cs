using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalEmergency : Emergency
{
    public int NPeopleInvolved;
    public int totalPeople;
    public int savedPeople;
    public float successRate;
    private float SalvationProb;

    public void InitEmergency(E_Severity severity, int peopleInvolved, UrbanArea area)
    {
        base.InitEmergency(area, severity, this.GetComponent<Renderer>().material);
        this.NPeopleInvolved = peopleInvolved;
        this.totalPeople = peopleInvolved;
        this.successRate = -1;
        this.SalvationProb = 0.99f;
        NewSeverity(severity);
        ChangeWaitTime();
    }

    public override int GetEmergencyPeopleEnvolved()
    {
        return NPeopleInvolved;
    }

    // Start is called before the first frame update
    new void Start()
    {

    }

    // Update is called once per frame
    new void Update()
    {
        Duration += Time.deltaTime;

        if (this.NAmbulances == 0)
        {
            if (NPeopleInvolved < 1)
            {
                //notify area + central
                MyArea.RemoveEmergency(this, successRate);
                Destroy(this.gameObject);
            }
            else
            {

                IncreaseSeverity();

                this.NAmbulances = -1;
                MyArea.ReOpenEmergency(this);
            }
        }

        else
        {
            CheckIfDead();
            
            IncreaseSeverity();
        }

    }

    public override void CheckIfDead()
    {
        float dyingProb = UnityEngine.Random.Range(0f, 1f);
        if (Severity == E_Severity.Medium)
        {
            if (dyingProb <= 0.001)
            {
                Debug.Log("medium dead" + dyingProb);
                NPeopleInvolved -= 1;
                MyArea.NotSaved();
            }
        }
        else if (Severity == E_Severity.Severe)
        {
            if (dyingProb <= 0.005)
            {
                Debug.Log("severe dead" + dyingProb);
                NPeopleInvolved -= 1;
                MyArea.NotSaved();
            }
        }
        
    }

    public override void SendResources(int a, int f)
    {
        this.NAmbulances = a;
    }

    public override void NewSeverity(E_Severity severity)
    {
        if (severity == E_Severity.Light)
        {
            SalvationProb = 0.99f;
            MyMaterial.color = new Color(179f / 255f, 223f / 255f, 255f / 255f);
        }
        else if (severity == E_Severity.Medium)
        {
            SalvationProb = 0.70f;
            MyMaterial.color = new Color(0f, 149f / 255f, 255f / 255f);
        }
        else
        {
            SalvationProb = 0.50f;
            MyMaterial.color = new Color(0f, 62f / 255f, 107f / 255f);
        }
    }

    public override void ChangeSeverity(E_Severity severity)
    {
        if (severity == E_Severity.Medium)
        {
            SetEmergencySeverity(severity);
            SalvationProb = 0.70f;
            MyMaterial.color = new Color(0f, 149f / 255f, 255f / 255f);
        }
        else
        {
            SetEmergencySeverity(severity);
            SalvationProb = 0.50f;
            MyMaterial.color = new Color(0f, 62f / 255f, 107f / 255f);
        }
    }

    public override bool TreatEmergency()
    {
        float salvationProb = UnityEngine.Random.Range(0f, 1f);
        if (salvationProb <= this.SalvationProb)
        {
            NPeopleInvolved -= 1;
            savedPeople += 1;
            successRate = savedPeople/totalPeople;
            MyArea.Saved();
            return true;
        }
        NPeopleInvolved -= 1;
        successRate = savedPeople/totalPeople;
        MyArea.NotSaved();
        return false;
    }

}
