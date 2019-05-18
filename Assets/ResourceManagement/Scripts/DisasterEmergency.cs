using System;
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

    public float ratio;

    private float AffectedArea;
    private float InitialAffectedArea;
    private float Regain;
    private int MaxRegain = 1;

    public void InitEmergency(E_Severity severity, int peopleInvolved, UrbanArea area)
    {
        base.InitEmergency(area, severity, this.GetComponent<Renderer>().material);
        this.creationTime = Time.time;
        NewSeverity(severity);
        InitialAffectedArea = AffectedArea;
    }

    public override void NewSeverity(E_Severity severity)
    {
        if (severity == E_Severity.Light)
        {
            SetEmergencySeverity(severity);
            DevastationLife = LightDevastationLife;
            regainEnergyPercentage = LightRegainEnergyPercentage;
            MyMaterial.color = new Color(255f / 255f, 220f / 255f, 46f / 255f);
            AffectedArea = LightDevastationLife;

        }

        else if (severity == E_Severity.Medium)
        {
            SetEmergencySeverity(severity);
            DevastationLife = MediumDevastationLife;
            regainEnergyPercentage = MediumRegainEnergyPercentage;
            MyMaterial.color = new Color(219f / 255f, 69f / 255f, 0f);
            AffectedArea = MediumDevastationLife;
        }
        else
        {
            SetEmergencySeverity(severity);
            DevastationLife = SevereDevastationLife;
            regainEnergyPercentage = SevereRegainEnergyPercentage;
            MyMaterial.color = new Color(1f, 0f, 0f);
            AffectedArea = SevereDevastationLife;
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
            AffectedArea += AffectedArea * 0.25f;
        }
        else
        {
            SetEmergencySeverity(severity);
            regainEnergyPercentage = SevereRegainEnergyPercentage;
            DevastationLife += DevastationLife * 0.5f;
            MyMaterial.color = new Color(1f, 0f, 0f);
            AffectedArea += AffectedArea * 0.5f;
        }
    }

    public override void SendResources(int ambulances, int firetrucks)
    {
        sendTime(Time.time - creationTime);

        if (NFiretrucks == -1)
            this.NFiretrucks = firetrucks;
        else NFiretrucks += firetrucks;
    }

    public override bool TreatEmergency(Firetruck f)
    {
        DevastationLife -= Firetruck.damage;
        return true;
    }

    public override float GetEmergencyDisasterLife()
    {
        return DevastationLife;
    }
    // Start is called before the first frame update
    new void Start()
    {
        
    }

    public override  void SendStatistics()
    {
        ratio = InitialAffectedArea / AffectedArea;
        //Debug.Log("hi");
        //Debug.Log(InitialAffectedArea);
        //Debug.Log(AffectedArea);
        MyArea.Ratio(AffectedArea/InitialAffectedArea);
        if (ratio < 0.5)
        {
            Debug.Log("Dead");
        }
        MyArea.AddEmergencyStatistics(this, ratio);
    }

    // Update is called once per frame
    new void Update()
    {
        Duration += Time.deltaTime;
        if (this.DevastationLife <= 0 && NFiretrucks == 0)
        {
            
            //Destroy(this.gameObject);
        }
        else
        {
            Regain = Math.Min(regainEnergyPercentage * DevastationLife, MaxRegain);
            DevastationLife += Regain;

            IncreaseSeverity();

            var p = UnityEngine.Random.Range(0f, 1f);
            if (p < 0.5)
            {
                AffectedArea += regainEnergyPercentage * AffectedArea;
            }

            if (this.NFiretrucks == 0)
            {
                this.NFiretrucks = -1;
                MyArea.ReOpenEmergency(this);
            }

        }
    }

    public override int NeededFiretrucks()
    {
        return (int) Math.Ceiling((DevastationLife - Math.Max(0, NFiretrucks) * Firetruck.waterDeposit) / Firetruck.waterDeposit);
    }
}
