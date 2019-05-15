using Dapper;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class MachineRepository
    {
        private IDbConnection _conn;

        public MachineRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<Machine>> GetMachines()
        {
            var sql = @"
                SELECT
                    Id,
                    MachineType,
                    Broken,
                    Active,
                    Variation,
                    CreateDate
                FROM dbo.Machines";

            return await _conn.QueryAsync<Machine>(sql);
        }

        public async Task<IEnumerable<Machine>> GetActiveMachines()
        {
            var sql = @"
                SELECT
                    Id,
                    MachineType,
                    Broken,
                    Active,
                    Variation,
                    CreateDate
                FROM dbo.Machines
                WHERE Active = 1";

            return await _conn.QueryAsync<Machine>(sql);
        }

        public async Task<Machine> GetMachine(int id)
        {
            var sql = @"
                SELECT
                    Id,
                    MachineType,
                    Broken,
                    Active,
                    Variation,
                    CreateDate
                FROM dbo.Machines
                WHERE Id = @id";

            var result = await _conn.QueryAsync<Machine>(sql, id);

            return result.First();
        }

        public async Task<Machine> UpdateMachine(Machine machine)
        {
            var sql = @"
                UPDATE dbo.Machines SET
                    Broken = @Broken,
                    Active = @Active
                WHERE Id = @Id";

            var result = await _conn.ExecuteAsync(sql, machine);

            return machine;
        }

        public async Task<Machine> CreateMachine(Machine machine)
        {
            var sql = @"
                INSERT INTO dbo.Machines
                    (MachineType,
                    Variation)
                OUTPUT INSERTED.*
                VALUES
                    (@MachineType,
                    @Variation)";

            var inserted = await _conn.QueryAsync<Machine>(sql, machine);
            return inserted.First();
        }

        public async Task<MachineRunResults> CreateRunResults(MachineRunResults results)
        {
            var sql = @"
                INSERT INTO dbo.MachineRunResults
                    (MachineRuns
                    ,RunTime
                    ,CountBrokenMachines
                    ,SmashedCount
                    ,SlashedCount
                    ,TrashedCount
                    ,WidgetsDestroyed)
                VALUES
                    (@MachineRuns
                    ,@RunTime
                    ,@CountBrokenMachines
                    ,@SmashedCount
                    ,@SlashedCount
                    ,@TrashedCount
                    ,@WidgetsDestroyed)";

            await _conn.ExecuteAsync(sql, new
            {
                MachineRuns = JsonConvert.SerializeObject( new { results.MachineRuns, results.OrderStart, results.OrderEnd }),
                results.RunTime,
                results.CountBrokenMachines,
                results.SmashedCount,
                results.SlashedCount,
                results.TrashedCount,
                results.WidgetsDestroyed
            });

            return results;
        }
    }
}
