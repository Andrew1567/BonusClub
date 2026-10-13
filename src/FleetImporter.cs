using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class FleetImporter
    {
        public Vehicle Parse(string line)
        {
            if (line == null)
            {
                return null;
            }

            string[] p = line.Split(';');
            
            if (p.Length != 6)
            {
                return null;
            }

            int id, odo, last;
            if (!int.TryParse(p[0], out id) ||
                !int.TryParse(p[3], out odo) ||
                !int.TryParse(p[4], out last))
            {
                return null;
            }

            DateTime d;
            if (!DateTime.TryParse(p[5], out d))
            {
                return null;
            }

            var v = new Vehicle();
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

            return v;
        }

        public List<Vehicle> LoadAll(string[] lines)
        {
            var list = new List<Vehicle>();
            foreach (var line in lines)
            {
                if (TryParse(line, out Vehicle v))
                {
                    list.Add(v);
                }
            }
            return list;
        }

        private bool TryParse(string line, out Vehicle vehicle)
        {
            vehicle = null;
            try
            {
                vehicle = Parse(line);
                return vehicle != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}