using System;
using System.Collections.Generic;

namespace homework
{
    // ===== 4. Delegate: shape of the method that reacts to a status change =====
    public delegate void StatusChangedHandler(string serialNumber, string message);

    // ===== 5. Static class: checks the data before it is used =====
    public static class Validator
    {
        public static bool IsValidSerial(string serial)
        {
            // Example of a valid serial: REF-90812
            if (serial == null || serial.Length < 5)
            {
                return false;
            }

            // The serial must have a dash in it
            for (int i = 0; i < serial.Length; i++)
            {
                if (serial[i] == '-')
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsValidName(string name)
        {
            return name != null && name != "";
        }

        public static bool IsValidRepairTime(int hours)
        {
            return hours > 0;
        }
    }

    // ===== 1. Abstraction: interface =====
    public interface IDevice
    {
        void PerformMaintenance();
        void RepairCompleted();
        int CalculateRepairCost();
    }

    // ===== 1. Abstraction: abstract class =====
    public abstract class Device : IDevice
    {
        // 5. Static fields behind the static properties
        private static int totalDevices = 0;
        private static int inRepairCount = 0;
        private static int completedCount = 0;

        // 3. Encapsulation: all fields are private
        private readonly string serialNumber;
        private string name;
        private int repairTime;
        private string status;

        // 4. Event: raised when the status changes
        public event StatusChangedHandler OnStatusChanged;

        protected Device(string serialNumber, string name, int repairTime)
        {
            if (Validator.IsValidSerial(serialNumber))
            {
                this.serialNumber = serialNumber;
            }
            else
            {
                Console.WriteLine("Invalid serial number, using UNKNOWN-0");
                this.serialNumber = "UNKNOWN-0";
            }

            Name = name;
            RepairTime = repairTime;
            status = "Received";
            totalDevices++;
        }

        // 5. Static properties
        public static int TotalDevices
        {
            get { return totalDevices; }
        }

        public static int InRepairCount
        {
            get { return inRepairCount; }
        }

        public static int CompletedCount
        {
            get { return completedCount; }
        }

        // Read-only: can only be set in the constructor
        public string SerialNumber
        {
            get { return serialNumber; }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (Validator.IsValidName(value))
                {
                    name = value;
                }
                else
                {
                    name = "Unknown device";
                }
            }
        }

        public int RepairTime
        {
            get { return repairTime; }
            set
            {
                if (Validator.IsValidRepairTime(value))
                {
                    repairTime = value;
                }
                else
                {
                    repairTime = 1;
                }
            }
        }

        // Only the device itself can change its status
        public string Status
        {
            get { return status; }
            private set { status = value; }
        }

        // Every device type calculates its cost differently
        public abstract int CalculateRepairCost();
        public void PerformMaintenance()
        {
            if (Status != "Received")
            {
                Console.WriteLine(Name + " is already in the repair process.");
                return;
            }

            Status = "In Repair";
            inRepairCount++;
            RaiseStatusChanged(Name + " is now in repair.");
        }

        public void RepairCompleted()
        {
            if (Status != "In Repair")
            {
                Console.WriteLine(Name + " is not being repaired right now.");
                return;
            }

            Status = "Completed";
            inRepairCount--;
            completedCount++;
            RaiseStatusChanged(Name + " repair completed.");
        }

        private void RaiseStatusChanged(string message)
        {
            if (OnStatusChanged != null)
            {
                OnStatusChanged(SerialNumber, message);
            }
        }
    }

    // ===== 2. Inheritance + polymorphism: 3 subclasses =====
    public class Refrigerator : Device
    {
        public Refrigerator(string serialNumber, string name, int repairTime)
            : base(serialNumber, name, repairTime) { }

        public override int CalculateRepairCost()
        {
            return 50 + (RepairTime * 30);
        }
    }

    public class WashingMachine : Device
    {
        public WashingMachine(string serialNumber, string name, int repairTime)
            : base(serialNumber, name, repairTime) { }

        public override int CalculateRepairCost()
        {
            return 40 + (RepairTime * 25);
        }
    }

    public class AirConditioner : Device
    {
        public AirConditioner(string serialNumber, string name, int repairTime)
            : base(serialNumber, name, repairTime) { }

        public override int CalculateRepairCost()
        {
            return 30 + (RepairTime * 20);
        }
    }

    // ===== Main class: list of the base type =====
    public class RepairCenter
    {
        private List<Device> devices = new List<Device>();

        public void AddDevice(Device device)
        {
            devices.Add(device);
        }

        public int TotalRepairCost()
        {
            int total = 0;
            foreach (Device d in devices)
            {
                total += d.CalculateRepairCost();   // each device runs its own version
            }
            return total;
        }

        public void PrintAll()
        {
            foreach (Device d in devices)
            {
                Console.WriteLine(d.Name + " [" + d.SerialNumber + "] - " + d.Status + " - cost: " + d.CalculateRepairCost());
            }
        }

        public void PrintByStatus(string status)
        {
            Console.WriteLine("Devices with status " + status + ":");
            foreach (Device d in devices)
            {
                if (d.Status == status)
                {
                    Console.WriteLine("  " + d.Name);
                }
            }
        }
    }

    class Program
    {
        // Method that matches the delegate and reacts to the event
        static void ShowNotification(string serialNumber, string message)
        {
            Console.WriteLine("[Notification " + serialNumber + "]: " + message);
        }

        static void Main(string[] args)
        {
            RepairCenter center = new RepairCenter();

            Refrigerator fridge = new Refrigerator("REF-90812", "Fridge", 3);
            WashingMachine washer = new WashingMachine("WSH-10455", "Washer", 2);
            AirConditioner ac = new AirConditioner("AIR-20731", "AC", 4);

            // Subscribe to the event
            fridge.OnStatusChanged += ShowNotification;
            washer.OnStatusChanged += ShowNotification;
            ac.OnStatusChanged += ShowNotification;

            center.AddDevice(fridge);
            center.AddDevice(washer);
            center.AddDevice(ac);

            fridge.PerformMaintenance();
            washer.PerformMaintenance();
            washer.RepairCompleted();
            Console.WriteLine();
            center.PrintAll();

            Console.WriteLine();
            center.PrintByStatus("In Repair");
            center.PrintByStatus("Completed");

            Console.WriteLine();
            Console.WriteLine("Total devices: " + Device.TotalDevices);
            Console.WriteLine("In repair: " + Device.InRepairCount);
            Console.WriteLine("Completed: " + Device.CompletedCount);
            Console.WriteLine("Total cost: " + center.TotalRepairCost());
        }
    }
}

    
