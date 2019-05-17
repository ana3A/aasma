using System.Collections;
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
    public List<int> ImpossibleEmergencies = new List<int>();
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

    public void SendData(List<float> responseTimesList, List<int> peopleSavedList, List<int> peopleNotSavedList, List<int> allPeopleList, float maxRepTime, int maxPeopleSav, int maxPeopleNotSav, float runningTime, List<float> burned, int ImpEm)
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
        ImpossibleEmergencies.Add(ImpEm);
    }

    public void WriteFile()
    {
        using (System.IO.StreamWriter logFile = new System.IO.StreamWriter(@"C:\Users\kapam\Documents\AASMA\Results.txt", true))
        {
            logFile.WriteLine("responseTimes");
            responseTimes.ForEach(logFile.WriteLine);

            logFile.WriteLine("peopleSaved");
            peopleSaved.ForEach(logFile.WriteLine);

            logFile.WriteLine("peopleNotSaved");
            peopleNotSaved.ForEach(logFile.WriteLine);

            logFile.WriteLine("allPeople");
            allPeople.ForEach(logFile.WriteLine);

            logFile.WriteLine("runTimes");
            runTimes.ForEach(logFile.WriteLine);

            logFile.WriteLine("maxResponseTime");
            logFile.WriteLine(maxResponseTime);

            logFile.WriteLine("maxPeopleSaved");
            logFile.WriteLine(maxPeopleSaved);

            logFile.WriteLine("maxPeopleNotSaved");
            logFile.WriteLine(maxPeopleNotSaved);
            
            logFile.WriteLine("burnedRatios");
            burnedRatios.ForEach(logFile.WriteLine);

            logFile.WriteLine("ImpossibleEmergencies");
            ImpossibleEmergencies.ForEach(logFile.WriteLine);

            logFile.WriteLine("------------------------------------------------");

        }
    }
}
