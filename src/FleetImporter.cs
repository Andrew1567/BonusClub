using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class FleetImporter
    {
        public Vehicle Parse(string line)
        {
            // Охранное условие 1
            if (line == null)
            {
                return null;
            }

            string[] p = line.Split(';');
            
            // Охранное условие 2
            if (p.Length != 6)
            {
                return null;
            }

            int id, odo, last;
            // Охранное условие 3 (если хотя бы одно число не спарсилось, выходим)
            if (!int.TryParse(p[0], out id) ||
                !int.TryParse(p[3], out odo) ||
                !int.TryParse(p[4], out last))
            {
                return null;
            }

            DateTime d;
            // Охранное условие 4
            if (!DateTime.TryParse(p[5], out d))
            {
                return null;
            }

            // Если все проверки пройдены, создаем объект
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