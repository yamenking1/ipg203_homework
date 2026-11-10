using System;
using System.Collections.Generic;

namespace homework
{
    
    public delegate void StatusChangedHandler(string serialNumber, string message);

 
    public static class Validator
    {
        public static bool IsValidSerial(string serial)
        {
           
            if (serial == null || serial.Length < 5)
            {
                return false;
            }

          
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

    
    public interface IDevice
    {
        void RepairCompleted();
        int CalculateRepairCost();
    }

   
    public abstract class Device : IDevice
    {
        
        private static int totalDevices = 0;
        private static int inRepairCount = 0;
        private static int completedCount = 0;

        
        private readonly string serialNumber;
        private string name;
        private int repairTime;
        private string status;

       
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
         
            status = "In Repair";
            totalDevices++;
            inRepairCount++;
        }

     
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

        
        public string Status
        {
            get { return status; }
            private set { status = value; }
        }

     
        public abstract int CalculateRepairCost();

        public void RepairCompleted()
        {
            if (Status != "In Repair")
            {
                Console.WriteLine(Name + " is already completed.");
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

    
    public class RepairCenter
    {
        private List<Device> devices = new List<Device>();

        public void AddDevice(Device device)
        {
            devices.Add(device);
        }

        
        public Device FindDevice(string serial)
        {
            foreach (Device d in devices)
            {
                if (d.SerialNumber == serial)
                {
                    return d;
                }
            }
            return null;
        }

        public int TotalRepairCost()
        {
            int total = 0;
            foreach (Device d in devices)
            {
                total += d.CalculateRepairCost();   
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
       
        static void ShowNotification(string serialNumber, string message)
        {
            Console.WriteLine("[Notification " + serialNumber + "]: " + message);
        }

        static void Main(string[] args)
        {
            RepairCenter center = new RepairCenter();
            bool running = true;

            while (running)
            {
                Console.WriteLine();
                Console.WriteLine("===== Repair Center =====");
                Console.WriteLine("1. Add a device");
                Console.WriteLine("2. Calculate repair cost");
                Console.WriteLine("3. Show if a device is prepared");
                Console.WriteLine("4. Mark a device as fully prepared");
                Console.WriteLine("5. Show devices still in repair");
                Console.WriteLine("6. Show devices that are done");
                Console.WriteLine("0. Quit");
                Console.Write("Choose: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Type: 1 = Refrigerator, 2 = Washing machine, 3 = Air conditioner");
                        Console.Write("Type: ");
                        string type = Console.ReadLine();

                        Console.Write("Serial number (example REF-90812): ");
                        string serial = Console.ReadLine();
                        Console.Write("Name: ");
                        string name = Console.ReadLine();
                        Console.Write("Repair time in hours: ");
                        int hours = int.Parse(Console.ReadLine());

                        if (!Validator.IsValidSerial(serial))
                        {
                            Console.WriteLine("Invalid serial number, device not added.");
                        }
                        else if (center.FindDevice(serial) != null)
                        {
                            Console.WriteLine("A device with this serial number already exists.");
                        }
                        else if (type == "1")
                        {
                            Refrigerator fridge = new Refrigerator(serial, name, hours);
                            fridge.OnStatusChanged += ShowNotification;
                            center.AddDevice(fridge);
                            Console.WriteLine("Refrigerator added.");
                        }
                        else if (type == "2")
                        {
                            WashingMachine washer = new WashingMachine(serial, name, hours);
                            washer.OnStatusChanged += ShowNotification;
                            center.AddDevice(washer);
                            Console.WriteLine("Washing machine added.");
                        }
                        else if (type == "3")
                        {
                            AirConditioner ac = new AirConditioner(serial, name, hours);
                            ac.OnStatusChanged += ShowNotification;
                            center.AddDevice(ac);
                            Console.WriteLine("Air conditioner added.");
                        }
                        else
                        {
                            Console.WriteLine("Unknown type, device not added.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("serial number have to be at least 5 charcter long with  - in it");
                        Console.Write("Serial number: ");
                        Device costDevice = center.FindDevice(Console.ReadLine());
                        if (costDevice == null)
                        {
                            Console.WriteLine("Device not found.");
                        }
                        else
                        {
                            Console.WriteLine(costDevice.Name + " repair cost: " + costDevice.CalculateRepairCost());
                        }
                        Console.WriteLine("Total cost of all devices: " + center.TotalRepairCost());
                        break;

                    case "3":
                        Console.Write("Serial number: ");
                        Device statusDevice = center.FindDevice(Console.ReadLine());
                        if (statusDevice == null)
                        {
                            Console.WriteLine("Device not found.");
                        }
                        else if (statusDevice.Status == "Completed")
                        {
                            Console.WriteLine(statusDevice.Name + " is prepared.");
                        }
                        else
                        {
                            Console.WriteLine(statusDevice.Name + " is not prepared yet (" + statusDevice.Status + ").");
                        }
                        break;

                    case "4":
                        Console.Write("Serial number: ");
                        Device doneDevice = center.FindDevice(Console.ReadLine());
                        if (doneDevice == null)
                        {
                            Console.WriteLine("Device not found.");
                        }
                        else
                        {
                            doneDevice.RepairCompleted();
                        }
                        break;

                    case "5":
                        Console.WriteLine("Devices in repair: " + Device.InRepairCount);
                        center.PrintByStatus("In Repair");
                        break;

                    case "6":
                        Console.WriteLine("Devices completed: " + Device.CompletedCount);
                        center.PrintByStatus("Completed");
                        break;

                    case "0":
                        running = false;
                        Console.WriteLine("Goodbye.");
                        break;

                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }
    }
}
