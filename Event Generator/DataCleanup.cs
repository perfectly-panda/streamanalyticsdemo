using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DAL;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Event_Generator
{
    public static class DataCleanup
    {
        [FunctionName("DataCleanup")]
        public static async Task Run([TimerTrigger("0 0 */1 * * *")]TimerInfo myTimer, ILogger log)
        {
            using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
            {
                var machineRepo = new MachineRepository(conn);
                var orderRepo = new OrderRepository(conn);

                await machineRepo.CleanOldData();
                await orderRepo.CleanOldData();

            }
        }
    }
}
