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
        // ===== 3. Encapsulation (التغليف) =====

// (Private Fields)  
private readonly string _serialNumber;
private string _name;
private int _repairTime;
private string _status;

//  (Properties)
public string SerialNumber => _serialNumber; //(Read-Only)

public string Name
{
    get => _name;
    set => _name = !string.IsNullOrEmpty(value) ? value : "جهاز غير معروف";
}

public int RepairTime
{
    get => _repairTime;
    set => _repairTime = value >= 0 ? value : 0; 

public string Status
{
    get => _status;
    // جعل الـ set خاصاً لحماية الحالة من التعديل العشوائي الخارجي
    private set => _status = value; 
}
    }
    
    
}
    . تعريف الـ Delegate لتحديد شكل دالة الاستجابة للحدث
public delegate void MaintenanceStatusChangedHandler(string deviceSerial, string statusMessage);

public class MaintenanceManager
{
    // 2. تعريف الـ Event باستخدام الـ Delegate المعرف أعلاه
    public event MaintenanceStatusChangedHandler OnStatusChanged;

    // خاصية لرقم الجهاز التسلسلي (لأغراض التوضيح)
    public string SerialNumber { get; }
    public string CurrentStatus { get; private set; }

    public MaintenanceManager(string serialNumber)
    {
        SerialNumber = serialNumber;
        CurrentStatus = "قيد التشخيص";
    }

    // 3. دالة لتحديث حالة الصيانة وإطلاق الحدث عند تغير الحالة
    public void UpdateStatus(string newStatus)
    {
        CurrentStatus = newStatus;

        // التحقق من أن هناك من يستمع للأحداث قبل إطلاقها
        if (OnStatusChanged != null)
        {
            // إطلاق الحدث وإرسال التفاصيل (رقم الجهاز وحالة الصيانة الجديدة)
            OnStatusChanged(SerialNumber, $"تحديث حالة الجهاز ذو الرقم التسلسلي [{SerialNumber}]: أصبحت الحالة -> {CurrentStatus}");
        }
    }
}

// فئة لاختبار وتشغيل الحدث (مثال استهلاكي)
public class ProgramTest
{
    public static void Main()
    {
        // إنشاء كائن لإدارة صيانة جهاز معين
        MaintenanceManager deviceMaintenance = new MaintenanceManager("REF-90812");

        // الاشتراك في الحدث (Registering to the Event)
        deviceMaintenance.OnStatusChanged += (serial, message) =>
        {
            Console.WriteLine("[إشعار نظام الصيانة]: " + message);
        };

        // تغيير حالة الجهاز لتفعيل الحدث تلقائياً
        deviceMaintenance.UpdateStatus("اكتمل الإصلاح بنجاح وجاهز للتسليم");
    }
}

