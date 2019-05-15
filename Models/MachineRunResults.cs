using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class MachineRunResults
    {
        public MachineRunResults()
        {
            InitializeVariables();
        }

        public MachineRunResults(List<MachineRun> runs, List<Order> orderStart)
        {
            MachineRuns = runs;
            OrderStart = orderStart;
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            RunTime = DateTime.UtcNow;
            OrderEnd = new List<Order>();
            SmashedCount = 0;
            SlashedCount = 0;
            TrashedCount = 0;
            WidgetsDestroyed = 0;
        }

        public List<Order> OrderStart { get; set; }
        public List<Order> OrderEnd { get; set; }

        public List<MachineRun> MachineRuns { get; set; }

        public DateTime RunTime { get; set; }
        public int CountBrokenMachines { get; set; }
        public int SmashedCount { get; set; }
        public int SlashedCount { get; set; }
        public int TrashedCount { get; set; }
        public int WidgetsDestroyed { get; set; }
    }
}
