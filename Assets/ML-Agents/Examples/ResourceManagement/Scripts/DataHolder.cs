﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public int nRestarts = 3;
    public List<float> responseTimes = new List<float>();
    public List<int> peopleSaved = new List<int>();
    public List<int> peopleNotSaved = new List<int>();
    public List<int> allPeople = new List<int>();
    public List<float> runTimes = new List<float>();
    public List<float> burnedRatios = new List<float>();
    public float maxResponseTime = -1f;
    public int maxPeopleSaved = 0;
    public int maxPeopleNotSaved = 0;

    public static DataHolder instance = null;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(instance);
    }

    public void SendData(List<float> responseTimesList, List<int> peopleSavedList, List<int> peopleNotSavedList, List<int> allPeopleList, float maxRepTime, int maxPeopleSav, int maxPeopleNotSav, float runningTime, List<float> burned)
    {
        responseTimes.AddRange(responseTimesList);
        peopleSaved.AddRange(peopleSavedList);
        peopleNotSaved.AddRange(peopleNotSavedList);
        allPeople.AddRange(allPeopleList);
        runTimes.Add(runningTime);

        maxResponseTime = Mathf.Max(maxResponseTime, maxRepTime);
        maxPeopleSaved = Mathf.Max(maxPeopleSaved, maxPeopleSav);
        maxPeopleNotSaved = Mathf.Max(maxPeopleNotSaved, maxPeopleNotSav);

        burnedRatios.AddRange(burned);
    }
}
