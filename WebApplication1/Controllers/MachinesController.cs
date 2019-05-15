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

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MachinesController : ControllerBase
    {
        private IConfiguration config;

        public MachinesController(IConfiguration configuration)
        {
            config = configuration;
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

                await repo.CreateMachine(value);
            }
        }

        // POST api/machines/break/5
        [HttpPost("{id}")]
        public async void Break(int id)
        {
            var machine = new Machine()
            {
                Id = id,
                Broken = true
            };

            using (IDbConnection conn = new SqlConnection(config["Data:SqlConnection"]))
            {
                var repo = new MachineRepository(conn);

                await repo.UpdateMachine(machine);
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

                await repo.UpdateMachine(machine);
            }
        }
    }
}
