namespace FleetMaintenance
{
    public class ServiceTask
    {
        public int VehicleId;
        public string Kind;
        public string State;
        public decimal Cost;
        public string Reason;
        public decimal ReportedCost()
        {
            if (State == "OVERDUE")
            {
                return Cost * 1.15m;
            }
            return Cost;
        }
    }

}
