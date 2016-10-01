using System;
using Microsoft.VisualBasic.Devices;

namespace ExternalSorting
{
    internal static class MemoryInfo
    {
        static MemoryInfo()
        {
            Reset();
        }

        public static ulong MaximumMemoryAmount { get; private set; }

        public static void Reset()
        {
            var ci = new ComputerInfo();
            MaximumMemoryAmount = ci.AvailablePhysicalMemory;
        }

        public static float GetOccupiedMemoryPercent()
        {
            return (float) GC.GetTotalMemory(false)/MaximumMemoryAmount;
        }

        public static ulong GetFreeMemoryLeft()
        {
            return MaximumMemoryAmount - (ulong)GC.GetTotalMemory(false);
        }
    }
}