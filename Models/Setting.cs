using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Setting
    {
        public Settings Id { get; set; }
        public string SettingName { get; set; }
        public float SettingValue { get; set; }

        public static float GetSetting(List<Setting> settings, Settings setting)
        {
            return settings.First(s => s.Id == setting).SettingValue;
        }
    }
}
