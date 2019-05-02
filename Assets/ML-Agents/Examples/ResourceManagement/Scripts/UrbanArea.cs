using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UrbanArea : MonoBehaviour
{
    public GameObject EmergencyObject;
    //public int numBananas;
    public float range;
    public float TimeBetween;
    public ERCAgent MyERC;

    //Key: position Value: emergency
    private Dictionary<Vector3, GameObject> MyEmergencies;
    private IEnumerator MakeEmergenciesOccur(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            var em_prob = Random.Range(0f, 1f);
            if (em_prob <= 0.05) {
                CreateEmergency();
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
        Emergency.E_Type Etype;
        Emergency.E_Severity Eserv;
        int people_involved = 0;

        if (type_probability <= 0.6f)
        {
            people_involved = Random.Range((int)1, (int)21);
            Etype = Emergency.E_Type.Medical;
        }
        else //if (type_probability <= 0.8f)
        {
            Etype = Emergency.E_Type.Disaster;
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
        GameObject em = Instantiate(EmergencyObject, pos, Quaternion.Euler(new Vector3(0f, 0f, 0f)));
        Emergency e = em.GetComponent<Emergency>();
        e.InitEmergency(Eserv, Etype, people_involved, this);

        //Notify Area that emergency exists
        MyEmergencies.Add(em.gameObject.transform.localPosition, em);
        //Notify ERC that an emergency occured -> simulating the "calls"
        MyERC.EmergencyCall(e);
    }

    private bool TooCloseToCentral(Vector3 pos) {
        if (Vector3.Distance(pos, MyERC.transform.localPosition) < 5)
            return true;
        return false;
    }
    public void RemoveEmergency(Emergency em)
    {
        MyEmergencies.Remove(em.transform.localPosition);
        MyERC.EmergencyEnded(em);
    }

    public void ReOpenEmergency(Emergency em)
    {
        MyERC.EmergencyReOpen(em);
    }

    public void ResetUrbanArea()
    {
        MyEmergencies.Clear();
    }

}
