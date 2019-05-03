using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ERCAgent : MonoBehaviour
{
    //public GameObject area;
    //public UrbanArea myArea;
    public GameObject AmbulanceObject;
    public GameObject FiretruckObject;
    Rigidbody agentRb;
    private List<MedicalEmergency> MedicalEmergenciesWaiting;
    private List<DisasterEmergency> DisasterEmergenciesWaiting;
    private List<Emergency> EmergenciesBeingTreated;
    private Dictionary<Vector3, int> EmergencyIndex;
    [SerializeField]
    private int nAmbulances;
    [SerializeField]
    private int nFiretruck;
    private EmergencyComparor EmComparor;
    private int availableAmbulances;
    private int availableFiretrucks;

    // Start is called before the first frame update
    void Start()
    {
        EmComparor = new EmergencyComparor();
        EmergencyIndex = new Dictionary<Vector3, int>();
        MedicalEmergenciesWaiting = new List<MedicalEmergency>();
        DisasterEmergenciesWaiting = new List<DisasterEmergency>();
        EmergenciesBeingTreated = new List<Emergency>();
        availableAmbulances = nAmbulances;
        availableFiretrucks = nFiretruck;
        Physics.IgnoreLayerCollision(0, 9);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (EmergenciesBeingTreated.Count > 0)
        {
            Emergency em = EmergenciesBeingTreated[0];

            if (em.GetEmergencyType() == Emergency.E_Type.Medical)
            {
                if (availableAmbulances > 0)
                {
                    int ambulancesNeeded = em.GetEmergencyPeopleEnvolved() / (Ambulance.maxPeople);
                    ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);

                    for (int i = 0; i < ambulancesNeeded; i++)
                    {
                        Ambulance am = CreateAmbulance();
                        am.SendAmbulance(em);
                        availableAmbulances--;
                    }

                    EmergenciesWaiting.Remove(em);
                    EmergenciesBeingTreated.Add(em);
                }
            }
        }*/
        //Se há emergencias por tratar
        if (MedicalEmergenciesWaiting.Count > 0)
        {
            MedicalEmergency em = MedicalEmergenciesWaiting[0];

            if (availableAmbulances > 0)
            {
                int ambulancesNeeded = (int)Math.Ceiling((float)em.GetEmergencyPeopleEnvolved() / Ambulance.maxPeople);
                Debug.Log(ambulancesNeeded);
                ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);


                for (int i = 0; i < ambulancesNeeded; i++)
                {
                    Ambulance am = CreateAmbulance();
                    am.SendAmbulance(em);
                    availableAmbulances--;
                }

                em.SendResources(ambulancesNeeded, 0);
                MedicalEmergenciesWaiting.Remove(em);
                //EmergenciesBeingTreated.Add(em);
            }
        }

        if (DisasterEmergenciesWaiting.Count > 0)
        {
            DisasterEmergency em = DisasterEmergenciesWaiting[0];

            if (availableFiretrucks > 0)
            {
                int firetrucksNeeded = (int)Math.Ceiling((float)em.GetEmergencyDisasterLife() / Firetruck.waterDeposit);
                firetrucksNeeded = Mathf.Min(firetrucksNeeded, availableFiretrucks);

                for (int i = 0; i < firetrucksNeeded; i++)
                {
                    Firetruck am = CreateFiretruck();
                    am.SendFiretruck(em);
                    availableFiretrucks--;
                }

                em.SendResources(0, firetrucksNeeded);
                DisasterEmergenciesWaiting.Remove(em);
                //EmergenciesBeingTreated.Add(em);
            }

        }
    }

    public void ReturnAmbulance()
    {
        this.availableAmbulances++;
    }

    public void ReturnFiretruck()
    {
        this.availableFiretrucks++;
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

    private Ambulance CreateAmbulance()
    {
        Vector3 pos = this.transform.localPosition;
        pos.y = 1;
        GameObject am = Instantiate(AmbulanceObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        Ambulance a = am.GetComponent<Ambulance>();
        a.myERC = this;
        return a;
    }

    private Firetruck CreateFiretruck()
    {
        Vector3 pos = this.transform.localPosition;
        pos.y = 1;
        GameObject ft = Instantiate(FiretruckObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        Firetruck f = ft.GetComponent<Firetruck>();
        f.myERC = this;
        return f;
    }





}
