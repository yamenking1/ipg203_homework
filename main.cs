using System;
using System.Collections.Generic;

namespace homework
{
    // ===== 1. Abstraction: interface =====
    interface IDeviceSharedValues
    {
        void preformMaintainces();
        void repaireCompleted();
    }

    // ===== 1. Abstraction: abstract class =====
    abstract class DeviceShared : IDeviceSharedValues
    {
        public string Name { get; set; }
        public int RepaireTime { get; set; }

        // Each device stores its own name and repair time
        protected DeviceShared(string name, int repaireTime)
        {
            Name = name;
            RepaireTime = repaireTime;
        }

        public void preformMaintainces()
        {
            Console.WriteLine(Name + " In Repaire");
        }

        public void repaireCompleted()
        {
            Console.WriteLine(Name + " repaire completed");
        }

        // Abstract: every device calculates its cost differently
        public abstract int calclulateRepaireCost();
    }

    // ===== 2. Inheritance + polymorphism: 3 subclasses =====
    class Refregaritor : DeviceShared
    {
        public Refregaritor(string name, int repaireTime) : base(name, repaireTime) { }

        public override int calclulateRepaireCost()
        {
            return 50 + (RepaireTime * 30);
        }
    }

    class WashingMaching : DeviceShared
    {
        public WashingMaching(string name, int repaireTime) : base(name, repaireTime) { }

        public override int calclulateRepaireCost()
        {
            return 40 + (RepaireTime * 25);
        }
    }

    class Aircondetioner : DeviceShared
    {
        public Aircondetioner(string name, int repaireTime) : base(name, repaireTime) { }

        public override int calclulateRepaireCost()
        {
            return 30 + (RepaireTime * 20);
        }
    }

    // ===== Main class: list of the base type =====
    class RepairCenter
    {
        private List<DeviceShared> _devices = new List<DeviceShared>();

        public void AddAppliance(DeviceShared device)
        {
            _devices.Add(device);
        }

        public int TotalRepaireCost()
        {
            int total = 0;
            foreach (DeviceShared d in _devices)
            {
                total += d.calclulateRepaireCost();   // each device runs its own version
            }
            return total;                              // return AFTER the loop
        }

        public void PrintAll()
        {
            foreach (DeviceShared d in _devices)
            {
                Console.WriteLine(d.Name + " cost: " + d.calclulateRepaireCost());
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            RepairCenter center = new RepairCenter();

            Refregaritor fridge = new Refregaritor("Fridge", 3);
            WashingMaching washer = new WashingMaching("Washer", 2);
            Aircondetioner ac = new Aircondetioner("AC", 4);

            center.AddAppliance(fridge);
            center.AddAppliance(washer);
            center.AddAppliance(ac);

            fridge.preformMaintainces();
            fridge.repaireCompleted();

            center.PrintAll();
            Console.WriteLine("Total cost: " + center.TotalRepaireCost());
        }
    }
}
