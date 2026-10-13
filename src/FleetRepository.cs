using System;
using System.Collections.Generic;

namespace FleetMaintenance
{
    public class FleetRepository
    {
        // Робимо поле приватним
        private List<Vehicle> _items = new List<Vehicle>();

        // Публічна властивість для читання
        public IEnumerable<Vehicle> All
        {
            get { return _items; }
        }

        public void Add(Vehicle v)
        {
            _items.Add(v);
        }

        // Додаємо метод для завантаження списку з імпортера
        public void Load(IEnumerable<Vehicle> vehicles)
        {
            _items = new List<Vehicle>(vehicles);
        }

        public Vehicle FindByPlate(string plate)
        {
            foreach (var v in _items) // замінили Items на _items
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
                if (v != null)
                {
                    v.Odometer = km;
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public int CountActive()
        {
            int n = 0;
            foreach (var v in _items) // замінили Items на _items
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