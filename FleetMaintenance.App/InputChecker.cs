using System;

namespace FleetMaintenance.App;

public class InputChecker
{
    public int CountValidQuantities(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            if (int.TryParse(row, out var q) && q > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
