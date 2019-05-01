using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class ResourcesAcademy : Academy
{
    [HideInInspector]
    public GameObject[] agents;
    [HideInInspector]
    public UrbanArea[] listArea;

    public int totalScore;
    public Text scoreText;
    public override void AcademyReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("emergency"));
        ClearObjects(GameObject.FindGameObjectsWithTag("ambulance"));
        ClearObjects(GameObject.FindGameObjectsWithTag("firetruck"));
        //ClearObjects(GameObject.FindGameObjectsWithTag("erc"));

        //agents = GameObject.FindGameObjectsWithTag("agent");
        listArea = FindObjectsOfType<UrbanArea>();
        foreach (UrbanArea ba in listArea)
        {
            ba.ResetUrbanArea();
        }

        totalScore = 0;
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (GameObject bana in objects)
        {
            Destroy(bana);
        }
    }

    public override void AcademyStep()
    {
        scoreText.text = string.Format(@"Score: {0}", totalScore);
    }
}
