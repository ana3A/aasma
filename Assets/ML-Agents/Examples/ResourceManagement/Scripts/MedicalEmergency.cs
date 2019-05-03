using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalEmergency : Emergency
{
    public int NPeopleInvolved;
    private float SalvationProb;

    public void InitEmergency(E_Severity severity, int peopleInvolved, UrbanArea area)
    {
        base.InitEmergency(area, severity, this.GetComponent<Renderer>().material);
        this.NPeopleInvolved = peopleInvolved;
        this.SalvationProb = 0.99f;
        ChangeSeverity(severity);
        ChangeWaitTime();
    }

    public override int GetEmergencyPeopleEnvolved()
    {
        return NPeopleInvolved;
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
            if (NPeopleInvolved < 1)
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
    }

    public override void SendResources(int a, int f)
    {
        this.NAmbulances = a;
    }

    public override void ChangeSeverity(E_Severity severity)
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

    public override bool TreatEmergency()
    {
        float salvationProb = UnityEngine.Random.Range(0, 1);
        if (salvationProb <= this.SalvationProb)
        {
            NPeopleInvolved -= 1;
            return true;
        }
        return false;
    }
}
