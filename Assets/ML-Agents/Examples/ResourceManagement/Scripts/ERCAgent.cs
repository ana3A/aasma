using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ERCAgent : MonoBehaviour
{
    //public GameObject area;
    //public UrbanArea myArea;
    public GameObject AmbulanceObject;
    Rigidbody agentRb;
    private List<Emergency> EmergenciesWaiting;
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
        EmergenciesWaiting = new List<Emergency>();
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
        if (EmergenciesWaiting.Count > 0)
        {
            Emergency em = EmergenciesWaiting[0];

            if(em.GetEmergencyType() == Emergency.E_Type.Medical)
            {
                if (availableAmbulances > 0)
                {
                    int ambulancesNeeded = (int)Math.Ceiling((float)em.GetEmergencyPeopleEnvolved() / Ambulance.maxPeople);
                    ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);


                    for (int i = 0; i < ambulancesNeeded; i++)
                    {
                        Ambulance am = CreateAmbulance();
                        am.SendAmbulance(em);
                        availableAmbulances--;
                    }

                    em.SendAmbulance(ambulancesNeeded);
                    EmergenciesWaiting.Remove(em);
                    EmergenciesBeingTreated.Add(em);
                }
            }
            //else if (em.GetEmergencyType() == Emergency.E_Type.Disaster)
            //{
            //    if (availableFiretrucks > 0)
            //    {
            //        firetrucksNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);

            //        for (int i = 0; i < ambulancesNeeded; i++)
            //        {
            //            Ambulance am = CreateAmbulance();
            //            am.SendAmbulance(em);
            //            availableAmbulances--;
            //        }
            //    }
            //}
            //else
            //{

            //}
        }
    }

    public void ReturnAmbulance()
    {
        this.availableAmbulances++;
    }

    public void EmergencyCall(Emergency em)
    {
        EmergenciesWaiting.Add(em);
        EmergenciesWaiting.Sort(EmComparor);
    }

    public void EmergencyEnded(Emergency em)
    {
        EmergenciesBeingTreated.Remove(em);
    }

    public void EmergencyReOpen(Emergency em)
    {
        EmergenciesBeingTreated.Remove(em);
        EmergencyCall(em);
    }

    private Ambulance CreateAmbulance()
    {
        Vector3 pos = this.transform.localPosition;
        GameObject am = Instantiate(AmbulanceObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        Ambulance a = am.GetComponent<Ambulance>();
        a.myERC = this;
        return a;
    }





}
