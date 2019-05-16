using System.Collections;
using System.Collections.Generic;

public class EmergencyComparor : IComparer<Emergency>
{
    public int Compare(Emergency x, Emergency y)
    {
        if ((int)x.GetEmergencySeverity() > (int)y.GetEmergencySeverity())
        {
            return -1;
        }

        if ((int)x.GetEmergencySeverity() < (int)y.GetEmergencySeverity())
        {
            return 1;
        }

        else {
            if (x.GetEmergencyDuration() > y.GetEmergencyDuration())
            {
                return -1;
            }

            if (x.GetEmergencyDuration() < y.GetEmergencyDuration())
            {
                return 1;
            }

            else
            {
                return 0;
            }
        }

    }
}
