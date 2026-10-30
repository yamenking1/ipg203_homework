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




  
    class Program
    {
        static void Main(string[] args)
        {

        return 0;
        }
    }
