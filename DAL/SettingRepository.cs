using Dapper;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SettingRepository
    {
        private IDbConnection _conn;
        public SettingRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<Setting>> GetSettings()
        {
            var sql = @"
                SELECT
                    Id
                    ,SettingName
                    ,SettingValue
                FROM dbo.Settings";

            return await _conn.QueryAsync<Setting>(sql);
        }

        public async Task<IEnumerable<Setting>> GetSettings(List<Settings> settings)
        {
            var sql = @"
                SELECT
                    Id
                    ,SettingName
                    ,SettingValue
                FROM dbo.Settings
                WHERE Id IN @settings";
            return await _conn.QueryAsync<Setting>(sql, new { settings });
        }

        public async Task<Setting> GetSetting(Settings setting)
        {
            var sql = @"
                SELECT
                    Id
                    ,SettingName
                    ,SettingValue
                FROM dbo.Settings
                WHERE Id = @setting";

            var result = await _conn.QueryAsync<Setting>(sql, new { setting });

            return result.FirstOrDefault();
        }
    }
}
