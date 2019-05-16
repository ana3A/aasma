using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class ERCAgent : MonoBehaviour
{
    //public GameObject area;
    //public UrbanArea myArea;
    public GameObject AmbulanceObject;
    public GameObject FiretruckObject;
    public bool beRandom;
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
    private List<Ambulance> allAmbulances;
    private List<Firetruck> allFiretrucks;

    public bool Decentralized;

    public bool Multiple;
    public CommsSystem CommunicationSystem;

    public int wastedAmbulances;
    public int wastedFiretrucks;

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
        wastedAmbulances = 0;
        wastedFiretrucks = 0;
        Physics.IgnoreLayerCollision(0, 9);
        ambulances = new List<Ambulance>();
        firetrucks = new List<Firetruck>();
        allAmbulances = new List<Ambulance>();
        allFiretrucks = new List<Firetruck>();
        for (int i = 0; i < nAmbulances; i++)
        {
            var a = CreateAmbulance();
            ambulances.Add(a);
            allAmbulances.Add(a);
        }
        for (int i = 0; i < nFiretruck; i++)
        {
            var f = CreateFiretruck();
            firetrucks.Add(f);
            allFiretrucks.Add(f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beRandom) {
            if (spawnInterval >= maxSpawnInterval)
            {
                if (MedicalEmergenciesWaiting.Count > 0)
                {
                    int index = Random.Range(0, MedicalEmergenciesWaiting.Count);
                    Emergency em = MedicalEmergenciesWaiting[index];

                    if (availableAmbulances > 0)
                    {
                        int ambulancesNeeded = Random.Range(0, availableAmbulances + 1);
                        //int ambulancesNeeded = (int)Math.Ceiling((float)em.GetEmergencyPeopleEnvolved() / Ambulance.maxPeople);
                        //ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);


                        for (int i = 0; i < ambulancesNeeded; i++)
                        {
                            Ambulance am = ambulances[0]; // CreateAmbulance();
                            ambulances.RemoveAt(0);
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
                    int index = Random.Range(0, DisasterEmergenciesWaiting.Count);
                    Emergency em = DisasterEmergenciesWaiting[index];

                    if (availableFiretrucks > 0)
                    {
                        int firetrucksNeeded = Random.Range(0, availableFiretrucks + 1); //(int)Math.Ceiling((float)em.GetEmergencyDisasterLife() / Firetruck.waterDeposit);
                        //firetrucksNeeded = Mathf.Min(firetrucksNeeded, availableFiretrucks);

                        for (int i = 0; i < firetrucksNeeded; i++)
                        {
                            Firetruck am = firetrucks[0]; // CreateFiretruck();
                            firetrucks.RemoveAt(0);
                            am.SendFiretruck(em);
                            availableFiretrucks--;
                        }

                        em.SendResources(0, firetrucksNeeded);
                        DisasterEmergenciesWaiting.Remove(em);
                        //EmergenciesBeingTreated.Add(em);
                    }

                }

                spawnInterval = 0;
            }

            else
            {
                spawnInterval += Time.deltaTime;
            }
        }
        else
        {
            if (!Decentralized)
            {
                if (spawnInterval >= maxSpawnInterval)
                {
                    if (MedicalEmergenciesWaiting.Count > 0)
                    {
                        Emergency em = MedicalEmergenciesWaiting[0];

                        if (availableAmbulances > 0 || Multiple)
                        {
                            int ambulancesNeeded = (int)Math.Ceiling((float)em.GetEmergencyPeopleEnvolved() / Ambulance.maxPeople);
                            
                            if (Multiple)
                            {
                                int ambulancesRemaining = Mathf.Max(ambulancesNeeded-availableAmbulances, 0);
                                if (ambulancesRemaining > 0 )
                                {
                                    CommunicationSystem.needMedHelp(em, ambulancesRemaining);
                                }
                            }
                            
                            ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);


                            for (int i = 0; i < ambulancesNeeded; i++)
                            {
                                Ambulance am = ambulances[0]; // CreateAmbulance();
                                ambulances.RemoveAt(0);
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
                        Emergency em = DisasterEmergenciesWaiting[0];

                        if (availableFiretrucks > 0 || Multiple)
                        {
                            int firetrucksNeeded = (int)Math.Ceiling((float)em.GetEmergencyDisasterLife() / Firetruck.waterDeposit);
                            
                            if (Multiple)
                            {
                                int firetrucksRemaining = Mathf.Max(firetrucksNeeded-availableFiretrucks, 0);
                                if (firetrucksRemaining > 0 )
                                {
                                    CommunicationSystem.needFireHelp(em, firetrucksRemaining);
                                }
                            }

                            firetrucksNeeded = Mathf.Min(firetrucksNeeded, availableFiretrucks);

                            for (int i = 0; i < firetrucksNeeded; i++)
                            {
                                Firetruck am = firetrucks[0]; // CreateFiretruck();
                                firetrucks.RemoveAt(0);
                                am.SendFiretruck(em);
                                availableFiretrucks--;
                            }

                            em.SendResources(0, firetrucksNeeded);
                            DisasterEmergenciesWaiting.Remove(em);
                            //EmergenciesBeingTreated.Add(em);
                        }

                    }

                    spawnInterval = 0;
                }

                else
                {
                    spawnInterval += Time.deltaTime;
                }

                if (Multiple)
                {
                    if(availableAmbulances > (nAmbulances/2))
                    {
                        Emergency emh;
                        bool helping;
                        CommunicationSystem.checkMedHelps(this, out helping, out emh);
                        if(helping)
                        {
                            Debug.Log(this+ "borrowed ambulance to" + emh.MyArea);
                            Ambulance am = ambulances[0];
                            ambulances.RemoveAt(0);
                            am.SendAmbulance(emh);
                            availableAmbulances--;
                            emh.SendResources(1, 0);
                        }
                    }
                
                    if(availableFiretrucks > (nFiretruck/2))
                    {
                        Emergency emh;
                        bool helping;
                        CommunicationSystem.checkFireHelps(this, out helping, out emh);
                        if(helping)
                        {
                            Debug.Log(this+ "borrowed firetruck to" + emh.MyArea);
                            Firetruck am = firetrucks[0];
                            firetrucks.RemoveAt(0);
                            am.SendFiretruck(emh);
                            availableFiretrucks--;
                            emh.SendResources(0, 1);
                        }
                    }
                }
            }

            else
            {

            }
        }
    }

    internal void RestartERC()
    {
        MedicalEmergenciesWaiting.Clear();
        DisasterEmergenciesWaiting.Clear();
        spawnInterval = 0;
        availableAmbulances = nAmbulances;
        availableFiretrucks = nFiretruck;
        ambulances.Clear();
        firetrucks.Clear();
        foreach(Ambulance a in allAmbulances) {
            a.RestartAmbulance();
            ambulances.Add(a);
        }
        foreach (Firetruck a in allFiretrucks)
        {
            a.RestartFiretruck();
            firetrucks.Add(a);
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

    public void SendAmbulance(Ambulance a)
    {
        this.availableAmbulances--;
        ambulances.Remove(a);

    }

    public void SendFiretruck(Firetruck f)
    {
        this.availableFiretrucks--;
        firetrucks.Remove(f); ;
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

    public void MedicalEmergencyControlled(Emergency em)
    {
        MedicalEmergenciesWaiting.Remove(em);
    }

    public void DisasterEmergencyControlled(Emergency em)
    {
        DisasterEmergenciesWaiting.Remove(em);
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
        a.Decentralized = Decentralized;
        return a;
    }

    private Firetruck CreateFiretruck()
    {
        Vector3 pos = this.transform.position;
        pos.y = 1;
        GameObject ft = Instantiate(FiretruckObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        Firetruck f = ft.GetComponent<Firetruck>();
        f.myERC = this;
        f.Decentralized = Decentralized;
        return f;
    }

    public bool getGameOver()
    {
        return gameOver;
    }

    public void SimulationFailed()
    {
        gameOver = true;
    }

    public bool TheresMedicalEmergency()
    {
        return MedicalEmergenciesWaiting.Count > 0;
    }

    public bool TheresDisasterEmergency()
    {
        return DisasterEmergenciesWaiting.Count > 0;
    }

    public Emergency WorstMedicalEmergency()
    {
        return MedicalEmergenciesWaiting[0];
    }

    public Emergency WorstDisasterEmergency()
    {
        return DisasterEmergenciesWaiting[0];
    }

}
