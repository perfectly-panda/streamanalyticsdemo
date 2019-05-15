using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Models;

namespace Event_Generator
{
    public static class CheckMachineCounts
    {
        [FunctionName("CheckMachineCounts")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"CheckMachineCounts function executed at: {DateTime.Now}");

            try
            {
                using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
                {
                    var settingsRepo = new SettingRepository(conn);
                    var machineRepo = new MachineRepository(conn);

                    var machineRate = await settingsRepo.GetSetting(Settings.BaseMachineRate);
                    var activeMachines = await machineRepo.GetActiveMachines();

                    //decomission broken machines
                    var brokenMachines = activeMachines.Where(m => m.Broken);

                    log.LogDebug($"Found {brokenMachines.Count()} broken machines. Removing from circulation.");

                    foreach(var machine in brokenMachines)
                    {
                        machine.Active = false;
                        await machineRepo.UpdateMachine(machine);
                    }

                    if(activeMachines.Count(m => !m.Broken && m.MachineType == "Smasher") < machineRate.SettingValue)
                    {
                        await machineRepo.CreateMachine(new Machine("Smasher", ActionCheck.GenerateInt(10)));
                    }

                    if (activeMachines.Count(m => !m.Broken && m.MachineType == "Slasher") < machineRate.SettingValue)
                    {
                        await machineRepo.CreateMachine(new Machine("Slasher", ActionCheck.GenerateInt(10)));
                    }

                    if (activeMachines.Count(m => !m.Broken && m.MachineType == "Trasher") < machineRate.SettingValue)
                    {
                        await machineRepo.CreateMachine(new Machine("Trasher", ActionCheck.GenerateInt(10)));
                    }

                }
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }
        }
    }
}
