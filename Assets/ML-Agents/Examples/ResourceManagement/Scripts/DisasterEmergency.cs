﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterEmergency : Emergency
{
    public float DevastationLife;
    public float LightDevastationLife;
    public float MediumDevastationLife;
    public float SevereDevastationLife;

    private float regainEnergyPercentage;
    public float LightRegainEnergyPercentage;
    public float MediumRegainEnergyPercentage;
    public float SevereRegainEnergyPercentage;

    public void InitEmergency(E_Severity severity, int peopleInvolved, UrbanArea area)
    {
        base.InitEmergency(area, severity, this.GetComponent<Renderer>().material);

        NewSeverity(severity);
    }

    public override void NewSeverity(E_Severity severity)
    {
        if (severity == E_Severity.Light)
        {
            SetEmergencySeverity(severity);
            DevastationLife = LightDevastationLife;
            regainEnergyPercentage = LightRegainEnergyPercentage;
            MyMaterial.color = new Color(255f/255f, 220f/255f, 46f/255f);
        }

        else if (severity == E_Severity.Medium)
        {
            SetEmergencySeverity(severity);
            DevastationLife = MediumDevastationLife;
            regainEnergyPercentage = MediumRegainEnergyPercentage;
            MyMaterial.color = new Color(219f/255f, 69f/255f, 0f);
        }
        else
        {
            SetEmergencySeverity(severity);
            DevastationLife = SevereDevastationLife;
            regainEnergyPercentage = SevereRegainEnergyPercentage;
            MyMaterial.color = new Color(1f, 0f, 0f);
        }
    }

    public override void ChangeSeverity(E_Severity severity)
    {

        if (severity == E_Severity.Medium)
        {
            SetEmergencySeverity(severity);
            regainEnergyPercentage = MediumRegainEnergyPercentage;
            DevastationLife += DevastationLife * 0.25f;
            MyMaterial.color = new Color(219f / 255f, 69f / 255f, 0f);
        }
        else
        {
            SetEmergencySeverity(severity);
            regainEnergyPercentage = SevereRegainEnergyPercentage;
            DevastationLife += DevastationLife * 0.5f;
            MyMaterial.color = new Color(1f, 0f, 0f);
        }
    }

    public override void SendResources(int ambulances, int firetrucks)
    {
        this.NFiretrucks = firetrucks;
    }

    public override bool TreatEmergency()
    {
        DevastationLife -= Firetruck.damage;
        return true;
    }

    public override float GetEmergencyDisasterLife()
    {
        return DevastationLife;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Duration += Time.deltaTime;
        if (this.DevastationLife <= 0)
        {
            MyArea.RemoveEmergency(this);
            Destroy(this.gameObject);
        }
        else
        {

            DevastationLife += regainEnergyPercentage * DevastationLife;

            IncreaseSeverity();

            if (this.NFiretrucks == 0)
            {
                this.NFiretrucks = -1;
                MyArea.ReOpenEmergency(this);
            }

        }
    }
}
