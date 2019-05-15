using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class OrderProcessingResults
    {
        public Order Order { get; set; }
        public List<MachineRun> Machines { get; set; }
        public int SmashedCount { get; set; }
        public int SlashedCount { get; set; }
        public int TrashedCount { get; set; }
        public int WidgetsDestroyed { get; set; }
    }
}
