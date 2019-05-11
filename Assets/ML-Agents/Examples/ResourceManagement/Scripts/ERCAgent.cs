using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ERCAgent : MonoBehaviour
{
    //public GameObject area;
    //public UrbanArea myArea;
    public GameObject AmbulanceObject;
    public GameObject FiretruckObject;
    Rigidbody agentRb;
    private List<Emergency> MedicalEmergenciesWaiting;
    private List<Emergency> DisasterEmergenciesWaiting;
    private List<Emergency> EmergenciesBeingTreated;
    private Dictionary<Vector3, int> EmergencyIndex;
    [SerializeField]
    private int nAmbulances;
    [SerializeField]
    private int nFiretruck;
    private EmergencyComparor EmComparor;
    [SerializeField]
    private int availableAmbulances;
    [SerializeField]
    private int availableFiretrucks;
    private float spawnInterval = 0;
    private float maxSpawnInterval = 1;

    private bool gameOver;

    private List<Ambulance> ambulances;
    private List<Firetruck> firetrucks;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        EmComparor = new EmergencyComparor();
        EmergencyIndex = new Dictionary<Vector3, int>();
        MedicalEmergenciesWaiting = new List<Emergency>();
        DisasterEmergenciesWaiting = new List<Emergency>();
        EmergenciesBeingTreated = new List<Emergency>();
        availableAmbulances = nAmbulances;
        availableFiretrucks = nFiretruck;
        Physics.IgnoreLayerCollision(0, 9);
        ambulances = new List<Ambulance>();
        firetrucks = new List<Firetruck>();
        for (int i = 0; i < nAmbulances; i++)
        {
            ambulances.Add(CreateAmbulance());
        }
        for (int i = 0; i < nFiretruck; i++)
        {
            firetrucks.Add(CreateFiretruck());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MedicalEmergenciesWaiting.Count > 0)
        {
            Emergency em = MedicalEmergenciesWaiting[0];

            if (availableAmbulances > 0)
            {
                int ambulancesNeeded = (int)Math.Ceiling((float)em.GetEmergencyPeopleEnvolved() / Ambulance.maxPeople);
                ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);


                for (int i = 0; i < ambulancesNeeded; i++)
                {
                    Ambulance am = ambulances[0]; //CreateAmbulance();
                    am.SendAmbulance(em);
                    availableAmbulances--;
                    ambulances.RemoveAt(0);
                }

                em.SendResources(ambulancesNeeded, 0);
                MedicalEmergenciesWaiting.Remove(em);
                //EmergenciesBeingTreated.Add(em);
            }
        }

        if (DisasterEmergenciesWaiting.Count > 0)
        {
            Emergency em = DisasterEmergenciesWaiting[0];

            if (availableFiretrucks > 0)
            {
                int firetrucksNeeded = (int)Math.Ceiling((float)em.GetEmergencyDisasterLife() / Firetruck.waterDeposit);
                firetrucksNeeded = Mathf.Min(firetrucksNeeded, availableFiretrucks);

                for (int i = 0; i < firetrucksNeeded; i++)
                {
                    Firetruck am = firetrucks[0]; //CreateFiretruck();
                    am.SendFiretruck(em);
                    firetrucks.RemoveAt(0);
                    availableFiretrucks--;
                }

                em.SendResources(0, firetrucksNeeded);
                DisasterEmergenciesWaiting.Remove(em);
                //EmergenciesBeingTreated.Add(em);
            }

        }
    }

    public void ReturnAmbulance(Ambulance a)
    {
        this.availableAmbulances++;
        ambulances.Add(a);
    }

    public void ReturnFiretruck(Firetruck f)
    {
        this.availableFiretrucks++;
        firetrucks.Add(f);
    }

    public void EmergencyCall(DisasterEmergency em)
    {
        DisasterEmergenciesWaiting.Add(em);
        DisasterEmergenciesWaiting.Sort(EmComparor);
    }

    public void EmergencyCall(MedicalEmergency em)
    {
        MedicalEmergenciesWaiting.Add(em);
        MedicalEmergenciesWaiting.Sort(EmComparor);
    }

    public void EmergencyCall(BothEmergency em)
    {
        if (em.GetEmergencyPeopleEnvolved() > 0)
        {
            MedicalEmergenciesWaiting.Add(em);
            MedicalEmergenciesWaiting.Sort(EmComparor);
        }
        if (em.GetEmergencyDisasterLife() > 0)
        {
            DisasterEmergenciesWaiting.Add(em);
            DisasterEmergenciesWaiting.Sort(EmComparor);
        }
    }

    public void EmergencyCallMed(BothEmergency em)
    {
        MedicalEmergenciesWaiting.Add(em);
        MedicalEmergenciesWaiting.Sort(EmComparor);
    }

    public void EmergencyCallDis(BothEmergency em)
    {
        DisasterEmergenciesWaiting.Add(em);
        DisasterEmergenciesWaiting.Sort(EmComparor);
    }

    //public void EmergencyCall(BothEmergency em)
    //{
    //    if (em.GetEmergencyType() == Emergency.E_Type.Disaster)
    //    {
    //        DisasterEmergenciesWaiting.Add(em);
    //        DisasterEmergenciesWaiting.Sort(EmComparor);
    //    }
    //    else if (em.GetEmergencyType() == Emergency.E_Type.Medical)
    //    {
    //        MedicalEmergenciesWaiting.Add(em);
    //        MedicalEmergenciesWaiting.Sort(EmComparor);
    //    }
    //    else
    //    {
    //        MedicalEmergenciesWaiting.Add(em);
    //        MedicalEmergenciesWaiting.Sort(EmComparor);

    //        DisasterEmergenciesWaiting.Add(em);
    //        DisasterEmergenciesWaiting.Sort(EmComparor);
    //    }
    //}

    public void EmergencyEnded(Emergency em)
    {
        EmergenciesBeingTreated.Remove(em);
    }

    //public void EmergencyReOpen(Emergency em)
    //{
    //    EmergenciesBeingTreated.Remove(em);
    //    EmergencyCall(em);
    //}

    public void EmergencyReOpen(MedicalEmergency em)
    {
        //EmergenciesBeingTreated.Remove(em);
        EmergencyCall(em);
    }

    public void EmergencyReOpen(DisasterEmergency em)
    {
        //EmergenciesBeingTreated.Remove(em);
        EmergencyCall(em);
    }

    public void ReOpenDisEmergency(BothEmergency em)
    {
        //EmergenciesBeingTreated.Remove(em);
        EmergencyCallDis(em);
    }

    public void ReOpenMedEmergency(BothEmergency em)
    {
        //EmergenciesBeingTreated.Remove(em);
        EmergencyCallMed(em);
    }

    public void EmergencyReOpen(BothEmergency em)
    {
        //EmergenciesBeingTreated.Remove(em);
        EmergencyCall(em);
    }

    private Ambulance CreateAmbulance()
    {
        Vector3 pos = this.transform.position;
        pos.y = 1;
        GameObject am = Instantiate(AmbulanceObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        Ambulance a = am.GetComponent<Ambulance>();
        a.myERC = this;
        return a;
    }

    private Firetruck CreateFiretruck()
    {
        Vector3 pos = this.transform.position;
        pos.y = 1;
        GameObject ft = Instantiate(FiretruckObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        Firetruck f = ft.GetComponent<Firetruck>();
        f.myERC = this;
        return f;
    }

    public bool getGameOver()
    {
        return gameOver;
    }

    public void SimulationFailed ()
    {
        gameOver = true;
    }



}
