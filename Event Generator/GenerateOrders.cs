using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Models;
using Microsoft.Extensions.Configuration;
using DAL;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Event_Generator
{
    public static class GenerateOrders
    {
        [FunctionName("GenerateOrders")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [ServiceBus("orders", Connection = "ServiceBusConnection")] ICollector<string> orderOutput,
            [ServiceBus("logs", Connection = "ServiceBusConnection")] ICollector<string> logOutput,
            ILogger log)
        {
            log.LogInformation($"GenerateOrders function executed at: {DateTime.Now}");

            var settingsList = new List<Settings>
            {
                Settings.OrderRate,
                Settings.MaxActiveOrders,
                Settings.MaxOrderSize
            };
            try
            {
                using (IDbConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnection")))
                {
                    var settingsRepo = new SettingRepository(conn);
                    var orderRepo = new OrderRepository(conn);

                    var settings = await settingsRepo.GetSettings(settingsList);
                    var orderCount = await orderRepo.GetOpenOrderCount();

                    if (orderCount < Setting.GetSetting(settings.ToList(), Settings.MaxActiveOrders))
                    {
                        Order order = new Order(ActionCheck.GenerateInt((int)settings.First(s => s.Id == Settings.MaxOrderSize).SettingValue));

                        var result = await orderRepo.CreateOrder(order);

                        orderOutput.Add(JsonConvert.SerializeObject(result,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver()
                            }));
                        logOutput.Add($"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} New order created. Id: {result.Id}, Widget Count: {result.WidgetCount}");

                    }
                }
            }
            catch(Exception e)
            {
                log.LogError(e.Message);
            }

            log.LogInformation($"GenerateOrders function completed at: {DateTime.Now}");
        }
    }
}
