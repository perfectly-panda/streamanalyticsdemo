 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DAL;
using Event_Generator.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Models;

namespace Event_Generator
{
    public static class ProcessOrders
    {

        [FunctionName("ProcessOrders")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {

            var settingsList = new List<Settings>
            {
                    Settings.AverageProductionTime,
                    Settings.ProductionTimeVariability,
                    Settings.AverageFailureRate,
                    Settings.FailureRateVariability,
                    Settings.MachineFailureRate
            };

            try
            {
                using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
                {
                    var settingsRepo = new SettingRepository(conn);
                    var machineRepo = new MachineRepository(conn);

                    var settings = settingsRepo.GetSettings(settingsList).Result;
                    var machines = machineRepo.GetActiveMachines().Result;

                    var results = new List<MachineRun>();

                    foreach (var machine in machines)
                    {
                        var run = new MachineRun(machine, settings.ToList());

                        var result = RunMachine(run);

                        results.Add(result);

                        if (result.MachineBroken)
                        {
                            await context.CallActivityAsync("BreakMachine", result.Machine);
                        }
                    }

                    var runLog = await context.CallActivityAsync<MachineRunResults>("ProcessResults", results);

                    if (runLog != null)
                    {
                        runLog.MachineRuns = results;
                        var logResults = machineRepo.CreateRunResults(runLog).Result;
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }
        }

        public static MachineRun RunMachine(MachineRun machine)
        {
            var averageFailureRate = Setting.GetSetting(machine.SettingsList, Settings.AverageFailureRate);
            var failureRateVariability = Setting.GetSetting(machine.SettingsList, Settings.FailureRateVariability);
            var averageProductionTime = Setting.GetSetting(machine.SettingsList, Settings.AverageProductionTime);
            var productionTimeVariability = Setting.GetSetting(machine.SettingsList, Settings.ProductionTimeVariability);
            var machineFailureRate = Setting.GetSetting(machine.SettingsList, Settings.MachineFailureRate);

            var adjFailureRate = averageFailureRate;

            if (machine.Machine.Broken)
            {
                adjFailureRate = averageFailureRate * 10;
            }

            //produce widgets
            var baseProduction = ActionCheck.GenerateInt((int)(averageProductionTime * 2), (int)productionTimeVariability);

            var baseFailure = ActionCheck.GenerateFloat(adjFailureRate * 2, failureRateVariability)/4;

            baseFailure = baseFailure > 1 ? 1 : baseFailure < 0 ? 0 : baseFailure;

            machine.CompletedOutput = (int)Math.Ceiling(baseProduction * (1 - baseFailure));

            machine.FailedOutput = baseProduction - machine.CompletedOutput;

            //check to see if machine broke
            var machineAge = (DateTime.UtcNow - machine.Machine.CreateDate).TotalHours / 100;

            var failureChance1 = ActionCheck.GenerateFloat(machineFailureRate, (float)machineAge);
            var failureChance2 = ActionCheck.GenerateFloat(machineFailureRate, (float)machineAge);
            var failureChance3 = ActionCheck.GenerateFloat(machineFailureRate, (float)machineAge);

            if (!machine.MachineBroken && !ActionCheck.Check(failureChance1) && !ActionCheck.Check(failureChance2) && !ActionCheck.Check(failureChance3))
            {
                machine.MachineBroken = true;
            }

            return machine;
        }

        [FunctionName("BreakMachine")]
        public static async Task BreakMachine([ActivityTrigger] Machine machine, ILogger log)
        {
            using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
            {
                var repo = new MachineRepository(conn);

                machine.Broken = true;

                repo.UpdateMachine(machine);
            }
        }

        [FunctionName("ProcessResults")]
        public static async Task<MachineRunResults> ProcessResults([ActivityTrigger] List<MachineRun> machines, ILogger log)
        {
            log.LogDebug("starting run log");


            try
            {
                using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
                {
                    var orderRepo = new OrderRepository(conn);
                    var openOrders = orderRepo.GetOpenOrders().Result;

                    //store initial results
                    var results = new MachineRunResults(machines, openOrders.ToList());
                    results.OrderEnd = new List<Order>();

                    foreach (var order in openOrders.OrderBy(o => o.CreateDate))
                    {
                        var result = OrderProcessing.RunMachines(order, machines);

                        results.OrderEnd.Add(result.Order);
                        results.SmashedCount += result.SmashedCount;
                        results.SlashedCount += result.SlashedCount;
                        results.TrashedCount += result.TrashedCount;
                        results.WidgetsDestroyed += result.WidgetsDestroyed;

                        machines = results.MachineRuns;

                        if (order.PendingCount < 0)
                        {
                            if (order.SmashedCount > -(order.PendingCount))
                            {
                                order.SmashedCount += order.PendingCount;
                                order.PendingCount = 0;
                            }
                            else if(order.SlashedCount > -(order.PendingCount))
                            {
                                order.SlashedCount += order.PendingCount;
                                order.PendingCount = 0;
                            }
                            else
                            {
                                order.CompletedCount += order.PendingCount;
                                order.PendingCount = 0;
                            }
                        }

                        if (order.SmashedCount < 0)
                        {
                            order.PendingCount += order.SmashedCount;
                            order.SmashedCount = 0;
                        }

                        if (order.SlashedCount < 0)
                        {
                            order.PendingCount += order.SlashedCount;
                            order.SlashedCount = 0;
                        }

                        var orderUpdate = orderRepo.UpdateOrder(result.Order).Result;
                    }

                    log.LogWarning($"Processing Complete {results.SmashedCount}, {results.SlashedCount}, {results.TrashedCount}, {results.WidgetsDestroyed}");

                    return results;
                }
            }
            catch(Exception e)
            {
                log.LogError(e.Message);
            }

            return null;
        }

        [FunctionName("ProcessOrders_TimerStart")]
        public static async Task ProcessOrdersStart(
            [TimerTrigger("0 * * * * *")]TimerInfo myTimer,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            //don't try to play catchup
            if (!myTimer.IsPastDue)
            {
                // Function input comes from the request content.
                string instanceId = await starter.StartNewAsync("ProcessOrders", null);

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            }
            else
            {
                log.LogWarning("Processing Orders skipped: ignore IsPastDue invocations");
            }
        }

        private static Tuple<int, int> OrderProcessResult(int request, MachineRun machine)
        {
            var totalProcessed = machine.CompletedOutput + machine.FailedOutput;

            if(totalProcessed < request)
            {
                return new Tuple<int, int>(machine.CompletedOutput, machine.FailedOutput);
            }
            else
            {
                var ratioUsed = (decimal)request / totalProcessed;

                return new Tuple<int, int>((int)Math.Ceiling(machine.CompletedOutput * ratioUsed), (int)Math.Floor(machine.FailedOutput * ratioUsed));
            }
        }

        private static void ValueCheck(int count, bool error, string section)
        {
            if (count < 0)
            {
                throw new Exception($"Count should not be less than 0. Section failed: {section}");
            }

            if (error)
            {
                throw new Exception($"Order count error. Section failed: {section}");
            }
        }
        
    }
}