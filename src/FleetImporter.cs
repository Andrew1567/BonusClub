using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class FleetImporter
    {
        public Vehicle Parse(string line)
        {
            Vehicle v = null;
            if (line != null)
            {
                string[] p = line.Split(';');
                if (p.Length == 6)
                {
                    int id, odo, last;
                    if (int.TryParse(p[0], out id)
                        && int.TryParse(p[3], out odo)
                        && int.TryParse(p[4], out last))
                    {
                        DateTime d;
                        if (DateTime.TryParse(p[5], out d))
                        {
                            v = new Vehicle();
                            v.Id = id;
                            v.Plate = p[1];
                            v.Type = p[2];
                            v.Odometer = odo;
                            v.LastOilKm = last;
                            v.LastOilDate = d;
                            v.LastBrakeKm = last;
                            v.LastBrakeDate = d;
                            v.LastInspKm = last;
                            v.LastInspDate = d;
                        }
                    }
                }
            }
            return v;
        }

        public List<Vehicle> LoadAll(string[] lines)
        {
            var list = new List<Vehicle>();
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    var v = Parse(lines[i]);
                    if (v != null)
                    {
                        list.Add(v);
                    }
                }
                catch (Exception)
                {
                }
            }
            return list;
        }
    }
}
