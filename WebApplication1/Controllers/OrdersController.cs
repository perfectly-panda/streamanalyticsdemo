using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using WebApplication.Hubs;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IConfiguration config;
        private IHubContext<LogHub> _hubContext;

        public OrdersController(IConfiguration configuration, IHubContext<LogHub> hubContext)
        {
            config = configuration;
            _hubContext = hubContext;
        }



        // GET: api/Orders
        [HttpGet]
        public async Task<IEnumerable<Order>> Get()
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new OrderRepository(conn);

                return await repo.GetOrders();
            }
        }

        // GET: api/Orders/open
        [HttpGet("open")]
        public async Task<IEnumerable<Order>> GetOpen()
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new OrderRepository(conn);

                return await repo.GetOpenOrders();
            }
        }

        // GET api/Orders/5
        [HttpGet("{id}")]
        public async Task<Order> Get(int id)
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new OrderRepository(conn);

                return await repo.GetOrder(id);
            }
        }

        // POST api/Orders
        [HttpPost]
        public async void Post([FromBody] Order value)
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new OrderRepository(conn);


                var result = await repo.CreateOrder(value);

                await _hubContext.Clients.All.SendAsync("orderUpdate", JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                await _hubContext.Clients.All.SendAsync("newLog", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} New order created. Id: {result.Id}, Widget Count: {result.WidgetCount} -- via UI");
            }
        }

        // DELETE api/Orders/5
        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new OrderRepository(conn);

                var order = await repo.GetOrder(id);

                order.CompletedCount = order.WidgetCount;
                order.PendingCount = order.SlashedCount = order.SmashedCount = 0;

                var result = await repo.UpdateOrder(order);

                await _hubContext.Clients.All.SendAsync("orderUpdate", JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
            }
        }
    }
}
