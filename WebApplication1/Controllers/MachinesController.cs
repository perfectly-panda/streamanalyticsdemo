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
using Microsoft.AspNetCore.SignalR;
using WebApplication.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        private IConfiguration config;
        private IHubContext<LogHub> _hubContext;

        public MachinesController(IConfiguration configuration, IHubContext<LogHub> hubContext)
        {
            config = configuration;
            _hubContext = hubContext;
        }

        // GET api/machines
        [HttpGet]
        public async Task<IEnumerable<Machine>> Get()
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new MachineRepository(conn);

                return await repo.GetMachines();
            }
        }

        // GET api/machines/5
        [HttpGet("{id}")]
        public async Task<Machine> Get(int id)
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new MachineRepository(conn);

                return await repo.GetMachine(id);
            }
        }

        // POST api/machines
        [HttpPost]
        public async void Post([FromBody] Machine value)
        {
            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new MachineRepository(conn);

                var result = await repo.CreateMachine(value);

                await _hubContext.Clients.All.SendAsync("machineUpdate", JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                await _hubContext.Clients.All.SendAsync("newLog", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} Machine {result.Id} created as {result.MachineType}. -- via UI");

            }
        }

        // GET api/machines/break/5
        [HttpPost("break/{id}")]
        public async void Break(int id)
        {
            var machine = new Machine()
            {
                Id = id,
                Broken = true,
                Active = true
            };

            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new MachineRepository(conn);

                var result = await repo.UpdateMachine(machine);

                await _hubContext.Clients.All.SendAsync("machineUpdate", JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                await _hubContext.Clients.All.SendAsync("newLog", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} Machine {id} broken. -- via UI");

            }
        }

        // DELETE api/machines/5
        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            var machine = new Machine()
            {
                Id = id,
                Active = false
            };

            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new MachineRepository(conn);

                var result = await repo.UpdateMachine(machine);

                await _hubContext.Clients.All.SendAsync("machineUpdate", JsonConvert.SerializeObject(result,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }));
                await _hubContext.Clients.All.SendAsync("newLog", $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time")).ToLongTimeString()} Machine {id} deactivated. -- via UI");

            }
        }
    }
}
