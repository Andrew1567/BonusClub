using System;

namespace FleetMaintenance
{
    public class Vehicle
    {
        public int Id;
        public string Plate;
        public string Type;
        public int Odometer;
        public int LastOilKm;
        public DateTime LastOilDate;
        public int LastBrakeKm;
        public DateTime LastBrakeDate;
        public int LastInspKm;
        public DateTime LastInspDate;
        public bool Active = true;
    }
}
