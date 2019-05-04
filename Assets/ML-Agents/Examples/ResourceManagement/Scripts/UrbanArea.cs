using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UrbanArea : MonoBehaviour
{
    public GameObject EmergencyObject;
    public GameObject DisasterEmergencyObject;
    public GameObject MedicalEmergencyObject;
    //public int numBananas;
    public float range;
    public float TimeBetween;
    public ERCAgent MyERC;
    public int atualEmergencies = 0;
    public int maxEmergencies = 10;

    public int allPeople = 0;
    public int peopleSaved = 0;
    public int peopleNotSaved = 0;

    enum E_Type { Medical, Disaster, Both}

    //Key: position Value: emergency
    private Dictionary<Vector3, GameObject> MyEmergencies;
    private IEnumerator MakeEmergenciesOccur(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            if (atualEmergencies < maxEmergencies)
            {
                var em_prob = Random.Range(0f, 1f);
                if (em_prob <= 0.05)
                {
                    atualEmergencies += 1;
                    CreateEmergency();
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        MyEmergencies = new Dictionary<Vector3, GameObject>();
        var coroutine = MakeEmergenciesOccur(TimeBetween);
        StartCoroutine(coroutine);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateEmergency()
    {
        float type_probability = Random.Range(0f, 1f);
        Emergency.E_Severity Eserv;
        int people_involved = -1;
        E_Type Etype;

        if (type_probability <= 0.6f)
        {
            people_involved = Random.Range((int)1, (int)21);
            Etype = E_Type.Medical;
        }
        else //if (type_probability <= 0.8f)
        {
            Etype = E_Type.Disaster;
        }
        //else
        //{
        //    people_involved = Random.Range((int)1, (int)21);
        //    Etype = Emergency.E_Type.Both;
        //}

        float creation_probability = Random.Range(0.0f, 1.0f);
        if (creation_probability <= 0.6f)
        {
            Eserv = Emergency.E_Severity.Light;
        }
        else if (creation_probability <= 0.9f)
        {
            Eserv = Emergency.E_Severity.Medium;
        }
        else
        {
            Eserv = Emergency.E_Severity.Severe;
        }

        Vector3 pos = new Vector3(Random.Range(-range, range), 1f, Random.Range(-range, range)) + transform.position;
        while (this.MyEmergencies.ContainsKey(pos) || TooCloseToCentral(pos))
        {
            pos = new Vector3(Random.Range(-range, range), 1f, Random.Range(-range, range)) + transform.position;
        }
        

        if (Etype == E_Type.Medical)
        {
            GameObject em = Instantiate(MedicalEmergencyObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            MedicalEmergency e = em.GetComponent<MedicalEmergency>();
            e.InitEmergency(Eserv, people_involved, this);
            MyERC.EmergencyCall(e);
            //Notify Area that emergency exists
            MyEmergencies.Add(em.gameObject.transform.localPosition, em);
        }

        else if (Etype == E_Type.Disaster)
        {
            GameObject em = Instantiate(DisasterEmergencyObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
            DisasterEmergency e = em.GetComponent<DisasterEmergency>();
            e.InitEmergency(Eserv, people_involved, this);
            MyERC.EmergencyCall(e);
            //Notify Area that emergency exists
            MyEmergencies.Add(em.gameObject.transform.localPosition, em);
        }

        
        //Notify ERC that an emergency occured -> simulating the "calls"
        
    }

    private bool TooCloseToCentral(Vector3 pos) {
        if (Vector3.Distance(pos, MyERC.transform.localPosition) < 5)
            return true;
        return false;
    }
    public void RemoveEmergency(Emergency em)
    {
        atualEmergencies -= 1;
        MyEmergencies.Remove(em.transform.localPosition);
        MyERC.EmergencyEnded(em);
    }

    public void Saved()
    {
        peopleSaved += 1;
        allPeople += 1;
    }

    public void NotSaved()
    {
        peopleNotSaved += 1;
        allPeople += 1;
    }

    public void ReOpenEmergency(DisasterEmergency em)
    {
        MyERC.EmergencyReOpen(em);
    }

    public void ReOpenEmergency(MedicalEmergency em)
    {
        MyERC.EmergencyReOpen(em);
    }

    public void ResetUrbanArea()
    {
        MyEmergencies.Clear();
    }

}
