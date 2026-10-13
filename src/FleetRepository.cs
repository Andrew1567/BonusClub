using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class FleetRepository
    {
        public List<Vehicle> Items = new List<Vehicle>();

        public void Add(Vehicle v)
        {
            Items.Add(v);
        }

        public Vehicle FindByPlate(string plate)
        {
            foreach (var v in Items)
            {
                if (v.Plate == plate)
                {
                    return v;
                }
            }
            return null;
        }

        public bool UpdateOdometer(string plate, int km)
        {
            try
            {
                var v = FindByPlate(plate);
                v.Odometer = km;
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public int CountActive()
        {
            int n = 0;
            foreach (var v in Items)
            {
                if (v.Active)
                {
                    n++;
                }
            }
            return n;
        }
    }
}
