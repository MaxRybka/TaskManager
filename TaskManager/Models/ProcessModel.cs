using System;
using System.ComponentModel;
using System.Diagnostics;
using TaskManager.Resources;

namespace TaskManager.Models
{
    public class ProcessModel
    {
        private static readonly int Processor_Counter = Environment.ProcessorCount;
        private static readonly long Memory_Counter = PerformanceInfo.GetTotalMemory();

        public string Name { get; private set; }
        public int Id { get; private set; }
        public bool Active { get; private set; }
        public double CPU { get; private set; }
        public double RAMinPercents { get; private set; }
        public long RAMinKB { get; private set; }
        public int Streams { get; private set; }
        public int Handles { get; private set; }
        public string Folder { get; private set; }
        public string StartTime { get; private set; }
        public Process Process { get; }

        private PerformanceCounter CPUcounter { get; }
        private PerformanceCounter RAMcounter { get; }

        internal ProcessModel(Process process)
        {
            Process = process;
            Init();

            CPUcounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName, true);
            RAMcounter = new PerformanceCounter("Process", "Working Set", process.ProcessName, true);

            Update();
        }

        private void Init()
        {
            Name = Process.ProcessName;
            Id = Process.Id;
            Active = Process.Responding;
            Streams = Process.Threads.Count;
            Handles = Process.HandleCount;
            try
            {
                Folder = Process.MainModule.FileName;
            }
            catch (Win32Exception)
            {
                Folder = "unavailable"; // Access denied.
            }
            catch (InvalidOperationException) { }
            try
            {
                StartTime = Process.StartTime.ToString();
            }
            catch (Win32Exception)
            {
                StartTime = "unavailable"; // Access denied.
            }
            catch (InvalidOperationException) { }
        }

        // Recount metadata of the current process.
        internal void Update()
        {
            try
            {
                CPU = CPUcounter.NextValue() / Processor_Counter;
            }
            catch (InvalidOperationException) { }
            try
            {
                RAMinKB = (long)RAMcounter.NextValue() / 1024;
                RAMinPercents = 100 * RAMcounter.NextValue() / Memory_Counter;
            }
            catch (InvalidOperationException) { }
            Streams = Process.Threads.Count;
            Handles = Process.HandleCount;
        }

        public override int GetHashCode()
        {
            return Id;
        }
        public override bool Equals(object obj)
        {
            return !(obj is ProcessModel other) ? false : Id == other.Id;
        }
    }
}
