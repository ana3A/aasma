using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommsSystem : MonoBehaviour
{
    private static List<Tuple<Emergency, int>> MedicalEmergenciesNeedingHelp;
    private static List<Tuple<Emergency, int>> DisasterEmergenciesNeedingHelp;

    // Start is called before the first frame update
    void Start()
    {
        MedicalEmergenciesNeedingHelp = new List<Tuple<Emergency, int>>();
        DisasterEmergenciesNeedingHelp = new List<Tuple<Emergency, int>>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkMedHelps(ERCAgent helper, out bool ReturnB, out Emergency ReturnEm)
    {
        if(MedicalEmergenciesNeedingHelp.Count > 0){
            Tuple<Emergency, int> help = MedicalEmergenciesNeedingHelp[0];
            while (help.Item1 == null)    //deletes already completed emergencies
            {
                Debug.Log("Emergency already dealt with previously. Please try to help another");
                MedicalEmergenciesNeedingHelp.RemoveAt(0);
                if(MedicalEmergenciesNeedingHelp.Count == 0)
                {
                    Debug.Log("No more help needed");
                    ReturnB = false;
                    ReturnEm = null;
                    return;
                }
                else
                {
                    help = MedicalEmergenciesNeedingHelp[0];
                }
            }
            ReturnEm = help.Item1;
            if (ReturnEm.MyArea.MyERC != helper)
            {
                ReturnB = true;
                int ambulancesNeeded = help.Item2 - 1;
                MedicalEmergenciesNeedingHelp.RemoveAt(0);
                if (ambulancesNeeded > 0)
                {
                    Tuple<Emergency, int> helptuple = new Tuple<Emergency, int>(ReturnEm, ambulancesNeeded);
                    MedicalEmergenciesNeedingHelp.Insert(0, helptuple);
                }
                return;
            }
            else
            {
                Debug.Log(helper + " cant help itself: " + ReturnEm.MyArea.MyERC);
            }
            
        }

        ReturnB = false;
        ReturnEm = null;
    }

    public void checkFireHelps(ERCAgent helper, out bool ReturnB, out Emergency ReturnEm)
    {
        if(DisasterEmergenciesNeedingHelp.Count > 0){
            Tuple<Emergency, int> help = DisasterEmergenciesNeedingHelp[0];
            while (help.Item1 == null)    //deletes already completed emergencies
            {
                Debug.Log("Emergency already dealt with previously. Please try to help another");
                DisasterEmergenciesNeedingHelp.RemoveAt(0);
                if(DisasterEmergenciesNeedingHelp.Count == 0)
                {
                    Debug.Log("No more help needed");
                    ReturnB = false;
                    ReturnEm = null;
                    return;
                }
                else
                {
                    help = DisasterEmergenciesNeedingHelp[0];
                }
            }
            ReturnEm = help.Item1;
            if (ReturnEm.MyArea.MyERC != helper)
            {
                ReturnB = true;
                int firetrucksNeeded = help.Item2 - 1;
                DisasterEmergenciesNeedingHelp.RemoveAt(0);
                if (firetrucksNeeded > 0)
                {
                    Tuple<Emergency, int> helptuple = new Tuple<Emergency, int>(ReturnEm, firetrucksNeeded);
                    DisasterEmergenciesNeedingHelp.Insert(0, helptuple);
                }
                return;
            }
            else
            {
                Debug.Log(helper + " cant help itself: " + ReturnEm.MyArea.MyERC);
            }
            
        }

        ReturnB = false;
        ReturnEm = null;
    }

    public void needMedHelp(Emergency em, int ambulancesNeeded)
    {
        Debug.Log(em.MyArea.MyERC + " needs help");
        Tuple<Emergency, int> helptuple = new Tuple<Emergency, int>(em, ambulancesNeeded);
        MedicalEmergenciesNeedingHelp.Add(helptuple);
    }

    public void needFireHelp(Emergency em, int firetrucksNeeded)
    {
        Debug.Log(em.MyArea.MyERC + " needs help");
        Tuple<Emergency, int> helptuple = new Tuple<Emergency, int>(em, firetrucksNeeded);
        DisasterEmergenciesNeedingHelp.Add(helptuple);
    }
}
