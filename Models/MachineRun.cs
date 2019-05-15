using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class MachineRun
    {
        public MachineRun() { }
        public MachineRun(Machine machine, List<Setting> settings)
        {
            Machine = machine;
            SettingsList = settings;
            MachineType = machine.MachineType;
        }
        public Machine Machine { get; }
        public List<Setting> SettingsList { get;}

        public string MachineType { get; set; }

        public int CompletedOutput { get; set; }
        public int FailedOutput { get; set; }
        public bool MachineBroken { get; set; }
    }
}
