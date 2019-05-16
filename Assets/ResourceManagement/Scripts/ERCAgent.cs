using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class ERCAgent : MonoBehaviour
{
    //public GameObject area;
    public UrbanArea myArea;
    public GameObject AmbulanceObject;
    public GameObject FiretruckObject;
    public bool beRandom;
    Rigidbody agentRb;
    private List<Emergency> Calls;
    private List<Emergency> MedicalEmergenciesWaiting;
    private List<Emergency> DisasterEmergenciesWaiting;
    private List<Emergency> EmergenciesBeingTreated;
    //private List<Emergency> DisasterEmergenciesBeingTreated;
    private Dictionary<Vector3, Emergency> EmergencyDic;
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
        EmergencyDic = new Dictionary<Vector3, Emergency>();
        MedicalEmergenciesWaiting = new List<Emergency>();
        DisasterEmergenciesWaiting = new List<Emergency>();
        EmergenciesBeingTreated = new List<Emergency>();
        Calls = new List<Emergency>();
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
        if (Decentralized)
        {
            foreach (Emergency e in Calls)
            {
                if (!EmergencyDic.ContainsKey(e.gameObject.transform.position))
                {
                    EmergencyDic.Add(e.gameObject.transform.position, e);
                    AddEmergency(e);
                }
            }
            Calls.Clear();

            MedicalEmergenciesWaiting.Sort(EmComparor);
            DisasterEmergenciesWaiting.Sort(EmComparor);
        }
        else
        {
            //Perceptions
            //Our perceptions are the calls, which are simulated by the urban area
            //Update World State
            UpdateEmergencies();

            if (!beRandom)
            {
                //Create the intentions: our agent has all emergencies to be resolved as an intension
                //Generates only valid intensions
                //Filter Intensions and Generate Plan:
                //In here the ERC will chosse which emergencies to tackle.
                Emergency medEm;
                Emergency disEm;
                Emergency helpMedEm;
                Emergency helpDisEm;
                ChooseEmergenciesToTackle(out medEm, out disEm, out helpMedEm, out helpDisEm);

                //Execute the plan: by sending the resources
                SendResources(medEm, disEm);
                SendHelp(helpMedEm, helpDisEm);
            }

            else
            {
                Emergency medEm;
                Emergency disEm;
                getRandomEmergencies(out medEm, out disEm);
                sendRandomResources(medEm, disEm);
            }
        }
    }

    public void UpdateEmergencies()
    {
        //Delete or re-open emergencies that were being treated
        List<int> emToDestroy = new List<int>();
        List<int> emToRemove = new List<int>();
        bool ToRemove;
        int i = 0;
        while (i < EmergenciesBeingTreated.Count) {
            ToRemove = false;
            if (EmergenciesBeingTreated[i].GetEmergencyPeopleEnvolved() < 1 && EmergenciesBeingTreated[i].GetEmergencyDisasterLife() <= 0)
            {
                if (EmergenciesBeingTreated[i].NAmbulances <= 0 && EmergenciesBeingTreated[i].NFiretrucks <= 0)
                {
                    Emergency e = EmergenciesBeingTreated[i];
                    //EmergenciesBeingTreated.RemoveAt(i);
                    //Debug.Log("removed");
                    e.SendStatistics();
                    EmergenciesBeingTreated.Remove(e);
                    Destroy(e.gameObject);
                    Destroy(e);
                    myArea.atualEmergencies--;
                    continue;
                }
            }

            if (EmergenciesBeingTreated[i].GetEmergencyPeopleEnvolved() > 0 && EmergenciesBeingTreated[i].NAmbulances <= 0)
            {
                MedicalEmergenciesWaiting.Add(EmergenciesBeingTreated[i]);
                ToRemove = true;
            }

            if (EmergenciesBeingTreated[i].GetEmergencyDisasterLife() > 0 && EmergenciesBeingTreated[i].NFiretrucks <= 0)
            {
                DisasterEmergenciesWaiting.Add(EmergenciesBeingTreated[i]);
                ToRemove = true;
            }

            if (ToRemove)
            {
                EmergenciesBeingTreated.RemoveAt(i);
                continue;
            }

            i++;
        }

        //
        foreach(Emergency e in Calls)
        {
            if (!EmergencyDic.ContainsKey(e.gameObject.transform.position))
            {
                EmergencyDic.Add(e.gameObject.transform.position, e);
                AddEmergency(e);
            }
        }
        Calls.Clear();
    }

    private void SendResources(Emergency medEm, Emergency disEm)
    {
        if ((availableAmbulances > 0 || Multiple) && medEm != null)
        {
            int ambulancesNeeded = (int)Math.Ceiling((float)medEm.GetEmergencyPeopleEnvolved() / Ambulance.maxPeople);

            if (Multiple)
            {
                int ambulancesRemaining = Mathf.Max(ambulancesNeeded - availableAmbulances, 0);
                if (ambulancesRemaining > 0)
                {
                    CommunicationSystem.needMedHelp(medEm, ambulancesRemaining);
                }
            }


            ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);


            for (int i = 0; i < ambulancesNeeded; i++)
            {
                Ambulance am = ambulances[0]; // CreateAmbulance();
                ambulances.RemoveAt(0);
                am.SendAmbulance(medEm);
                availableAmbulances--;
            }

            medEm.SendResources(ambulancesNeeded, 0);
            MedicalEmergenciesWaiting.Remove(medEm);
            if (!EmergenciesBeingTreated.Contains(medEm))
            {
                EmergenciesBeingTreated.Add(medEm);
            }
            
        }

        if ((availableFiretrucks > 0 || Multiple) && disEm != null)
        {
            int firetrucksNeeded = (int)Math.Ceiling((float)disEm.GetEmergencyDisasterLife() / Firetruck.waterDeposit);

            if (Multiple)
            {
                int firetrucksRemaining = Mathf.Max(firetrucksNeeded - availableFiretrucks, 0);
                if (firetrucksRemaining > 0)
                {
                    CommunicationSystem.needFireHelp(disEm, firetrucksRemaining);
                }
            }

            firetrucksNeeded = Mathf.Min(firetrucksNeeded, availableFiretrucks);

            for (int i = 0; i < firetrucksNeeded; i++)
            {
                Firetruck am = firetrucks[0]; // CreateFiretruck();
                firetrucks.RemoveAt(0);
                am.SendFiretruck(disEm);
                availableFiretrucks--;
            }

            disEm.SendResources(0, firetrucksNeeded);
            DisasterEmergenciesWaiting.Remove(disEm);
            if (!EmergenciesBeingTreated.Contains(disEm))
            {
                EmergenciesBeingTreated.Add(disEm);
            }
        }
    }

    private void getRandomEmergencies(out Emergency medEm, out Emergency disEm)
    {
        medEm = disEm = null;
        int index;
        if (MedicalEmergenciesWaiting.Count > 0)
        {
            index = Random.Range(0, MedicalEmergenciesWaiting.Count);
            medEm = MedicalEmergenciesWaiting[index];
        }

        if (DisasterEmergenciesWaiting.Count > 0)
        {
            index = Random.Range(0, DisasterEmergenciesWaiting.Count);
            disEm = DisasterEmergenciesWaiting[0];
        }
    }

    private void sendRandomResources(Emergency medEm, Emergency disEm)
    {
        if ((availableAmbulances > 0 || Multiple) && medEm != null)
        {
            int ambulancesNeeded = Random.Range(0, availableAmbulances + 1);
            ambulancesNeeded = Mathf.Min(ambulancesNeeded, availableAmbulances);

            for (int i = 0; i < ambulancesNeeded; i++)
            {
                Ambulance am = ambulances[0]; // CreateAmbulance();
                ambulances.RemoveAt(0);
                am.SendAmbulance(medEm);
                availableAmbulances--;
            }

            medEm.SendResources(ambulancesNeeded, 0);
            MedicalEmergenciesWaiting.Remove(medEm);
            if (!EmergenciesBeingTreated.Contains(medEm))
            {
                EmergenciesBeingTreated.Add(medEm);
            }

        }

        if ((availableFiretrucks > 0 || Multiple) && disEm != null)
        {
            int firetrucksNeeded = Random.Range(0, availableFiretrucks + 1);
            firetrucksNeeded = Mathf.Min(firetrucksNeeded, availableFiretrucks);

            for (int i = 0; i < firetrucksNeeded; i++)
            {
                Firetruck am = firetrucks[0]; // CreateFiretruck();
                firetrucks.RemoveAt(0);
                am.SendFiretruck(disEm);
                availableFiretrucks--;
            }

            disEm.SendResources(0, firetrucksNeeded);
            DisasterEmergenciesWaiting.Remove(disEm);
            if (!EmergenciesBeingTreated.Contains(disEm))
            {
                EmergenciesBeingTreated.Add(disEm);
            }
        }
    }

    private void ChooseEmergenciesToTackle(out Emergency medEm, out Emergency disEm, out Emergency helpMedEm, out Emergency helpDisEm)
    {
        medEm = disEm = helpMedEm = helpDisEm = null;
        if (MedicalEmergenciesWaiting.Count > 0)
        {
            MedicalEmergenciesWaiting.Sort(EmComparor);
            medEm = MedicalEmergenciesWaiting[0];
        }

        if (DisasterEmergenciesWaiting.Count > 0)
        {
            DisasterEmergenciesWaiting.Sort(EmComparor);
            disEm = DisasterEmergenciesWaiting[0];
        }

        if (Multiple)
        {
            Emergency emh;
            bool helping;
            CommunicationSystem.checkMedHelps(this, out helping, out emh);
            if (helping)
            {
                helpMedEm = emh;
                //Debug.Log(this + "borrowed ambulance to" + emh.MyArea);
                //Ambulance am = ambulances[0];
                //ambulances.RemoveAt(0);
                //am.SendAmbulance(emh);
                //availableAmbulances--;
                //emh.SendResources(1, 0);
            }
            

            CommunicationSystem.checkFireHelps(this, out helping, out emh);
            if (helping)
            {
                helpDisEm = emh;
                //Debug.Log(this + "borrowed firetruck to" + emh.MyArea);
                //Firetruck am = firetrucks[0];
                //firetrucks.RemoveAt(0);
                //am.SendFiretruck(emh);
                //availableFiretrucks--;
                //emh.SendResources(0, 1);
            }
        }
    }

    private void SendHelp(Emergency helpMedEm, Emergency helpDisEm)
    {
        if (Multiple)
        {
            if (availableAmbulances > (nAmbulances / 2))
            {
                Emergency emh;
                bool helping;
                CommunicationSystem.checkMedHelps(this, out helping, out emh);
                if (helping)
                {
                    Debug.Log(this + "borrowed ambulance to" + emh.MyArea);
                    Ambulance am = ambulances[0];
                    ambulances.RemoveAt(0);
                    am.SendAmbulance(emh);
                    availableAmbulances--;
                    emh.SendResources(1, 0);
                }
            }

            if (availableFiretrucks > (nFiretruck / 2))
            {
                Emergency emh;
                bool helping;
                CommunicationSystem.checkFireHelps(this, out helping, out emh);
                if (helping)
                {
                    Debug.Log(this + "borrowed firetruck to" + emh.MyArea);
                    Firetruck am = firetrucks[0];
                    firetrucks.RemoveAt(0);
                    am.SendFiretruck(emh);
                    availableFiretrucks--;
                    emh.SendResources(0, 1);
                }
            }

        }
    }

    public void AddEmergency(Emergency e)
    {
        if (e.GetEmergencyPeopleEnvolved() > 0)
        {
            MedicalEmergenciesWaiting.Add(e);
        }

        if (e.GetEmergencyDisasterLife() > 0)
        {
            DisasterEmergenciesWaiting.Add(e);
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

    public void EmergencyCall(Emergency em)
    {
        Calls.Add(em);
    }

    public void EmergencyCallMed(BothEmergency em)
    {
        Calls.Add(em);
        //MedicalEmergenciesWaiting.Add(em);
        //MedicalEmergenciesWaiting.Sort(EmComparor);
    }

    public void EmergencyCallDis(BothEmergency em)
    {
        Calls.Add(em);
        //DisasterEmergenciesWaiting.Add(em);
        //DisasterEmergenciesWaiting.Sort(EmComparor);
    }
    
    public void EmergencyEnded(Emergency em)
    {
        if (EmergenciesBeingTreated.Contains(em))
        {
            EmergenciesBeingTreated.Remove(em);
            Destroy(em.gameObject);
            Destroy(em);
        }
    }

    public void MedicalEmergencyControlled(Emergency em)
    {
        MedicalEmergenciesWaiting.Remove(em);
        if (!EmergenciesBeingTreated.Contains(em))
        {
            EmergenciesBeingTreated.Add(em);
        }
    }

    public void DisasterEmergencyControlled(Emergency em)
    {
        DisasterEmergenciesWaiting.Remove(em);
        if (!EmergenciesBeingTreated.Contains(em))
        {
            EmergenciesBeingTreated.Add(em);
        }
    }
    
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

    internal void RestartERC()
    {
        MedicalEmergenciesWaiting.Clear();
        DisasterEmergenciesWaiting.Clear();
        spawnInterval = 0;
        availableAmbulances = nAmbulances;
        availableFiretrucks = nFiretruck;
        ambulances.Clear();
        firetrucks.Clear();
        foreach (Ambulance a in allAmbulances)
        {
            a.RestartAmbulance();
            ambulances.Add(a);
        }
        foreach (Firetruck a in allFiretrucks)
        {
            a.RestartFiretruck();
            firetrucks.Add(a);
        }
    }

}