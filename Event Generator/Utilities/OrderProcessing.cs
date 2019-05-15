using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Event_Generator.Utilities
{
    public static class OrderProcessing
    {
        public static OrderProcessingResults RunMachines(Order order, List<MachineRun> machines)
        {

            var smashers = machines.Where(m => m.MachineType == "Smasher").ToList();
            var slashers = machines.Where(m => m.MachineType == "Slasher").ToList();
            var trashers = machines.Where(m => m.MachineType == "Trasher").ToList();

            var smasherResults = order.PendingCount > 0?  RunType(order.PendingCount, smashers): new Tuple<int, int, List<MachineRun>>(0, 0, smashers);
            var slasherResults = order.SmashedCount > 0? RunType(order.SmashedCount, slashers) : new Tuple<int, int, List<MachineRun>>(0, 0, slashers);
            var trasherResults = order.SlashedCount > 0? RunType(order.SlashedCount, trashers) : new Tuple<int, int, List<MachineRun>>(0, 0, trashers);

            //update values
            order.PendingCount = order.PendingCount - smasherResults.Item1 + slasherResults.Item2 + trasherResults.Item2;
            order.SmashedCount = order.SmashedCount + smasherResults.Item1 - (slasherResults.Item1 + slasherResults.Item2);
            order.SlashedCount = order.SlashedCount + slasherResults.Item1 - (trasherResults.Item1 + trasherResults.Item2);
            order.CompletedCount += trasherResults.Item1;

            var updatedMachines = new List<MachineRun>();

            //return the remaining capacity
            updatedMachines.AddRange(smasherResults.Item3.Where(m => m.CompletedOutput > 0 && m.FailedOutput > 0).ToList());
            updatedMachines.AddRange(slasherResults.Item3.Where(m => m.CompletedOutput > 0 && m.FailedOutput > 0).ToList());
            updatedMachines.AddRange(trasherResults.Item3.Where(m => m.CompletedOutput > 0 && m.FailedOutput > 0).ToList());

            var results = new OrderProcessingResults()
            {
                Order = order,
                Machines = updatedMachines,
                SmashedCount = smasherResults.Item1,
                SlashedCount = slasherResults.Item1,
                TrashedCount = trasherResults.Item1,
                WidgetsDestroyed = smasherResults.Item2 + slasherResults.Item2 + trasherResults.Item2
            };

            return results;
        }
        public static Tuple<int,int, List<MachineRun>> RunType(int request, List<MachineRun> machines)
        {
            if (machines.Count() > 0)
            {
                var machineRequest = (int)Math.Floor((decimal)request / machines.Count());
                var mod = request % machines.Count();

                var suceeded = 0;
                var failed = 0;

                foreach(var machine in machines)
                {
                    var currentMachineRequest = machineRequest;
                    if(mod > 0)
                    {
                        currentMachineRequest++;
                        mod--;
                    }

                    var result = MachineContribution(currentMachineRequest, machine);

                    suceeded += result.Item1;
                    failed += result.Item2;
                    machine.CompletedOutput -= result.Item1;
                    machine.FailedOutput -= result.Item2;
                }

                return new Tuple<int, int, List<MachineRun>>(suceeded, failed, machines);
            }

            return new Tuple<int, int, List<MachineRun>>(0,0, machines);
        }

        public static Tuple<int, int> MachineContribution(int request, MachineRun machine)
        {
            var totalProcessed = machine.CompletedOutput + machine.FailedOutput;

            if (totalProcessed <= request)
            {
                return new Tuple<int, int>(machine.CompletedOutput, machine.FailedOutput);
            }
            else
            {
                var ratioUsed = (decimal)request / totalProcessed;

                return new Tuple<int, int>((int)Math.Ceiling(machine.CompletedOutput * ratioUsed), (int)Math.Floor(machine.FailedOutput * ratioUsed));
            }
        }
    }
}
