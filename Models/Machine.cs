using System;

namespace Models
{
    public class Machine
    {
        public Machine()
        {

        }

        public Machine(string machineType, int variation)
        {
            MachineType = machineType;
            Variation = variation;
            CreateDate = DateTime.UtcNow;
            Active = true;
            Broken = false;
        }

        public int Id { get; set; }
        public string MachineType { get; set; }
        public bool Broken { get; set; }
        public bool Active { get; set; }
        public int Variation { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
