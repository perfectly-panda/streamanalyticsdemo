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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Event_Generator
{
    public static class CheckMachineCounts
    {
        [FunctionName("CheckMachineCounts")]
        public static async Task Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer,
            [ServiceBus("machines", Connection = "ServiceBusConnection")] ICollector<string> machineOutput,
            [ServiceBus("logs", Connection = "ServiceBusConnection")] ICollector<string> logOutput,
            ILogger log)
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
                        machineOutput.Add(JsonConvert.SerializeObject(machine,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
                        logOutput.Add($"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} Machine {machine.Id} deactivated.");
                    }

                    if(activeMachines.Count(m => !m.Broken && m.MachineType == "Smasher") < machineRate.SettingValue)
                    {
                        var smasher = await machineRepo.CreateMachine(new Machine("Smasher", ActionCheck.GenerateInt(10)));
                        machineOutput.Add(JsonConvert.SerializeObject(smasher,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
                        logOutput.Add($"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} Machine {smasher.Id} created as {smasher.MachineType}.");
                    }

                    if (activeMachines.Count(m => !m.Broken && m.MachineType == "Slasher") < machineRate.SettingValue)
                    {
                        var slasher = await machineRepo.CreateMachine(new Machine("Slasher", ActionCheck.GenerateInt(10)));
                        machineOutput.Add(JsonConvert.SerializeObject(slasher,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
                        logOutput.Add($"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} Machine {slasher.Id} created as {slasher.MachineType}.");
                    }

                    if (activeMachines.Count(m => !m.Broken && m.MachineType == "Trasher") < machineRate.SettingValue)
                    {
                        var trasher = await machineRepo.CreateMachine(new Machine("Trasher", ActionCheck.GenerateInt(10)));

                        machineOutput.Add(JsonConvert.SerializeObject(trasher,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
                        logOutput.Add($"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} Machine {trasher.Id} created as {trasher.MachineType}.");
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
