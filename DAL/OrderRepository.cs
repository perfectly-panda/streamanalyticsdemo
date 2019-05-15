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
    public class OrderRepository
    {
        private IDbConnection _conn;

        public OrderRepository(IDbConnection conn)
        {
            _conn = conn;
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            var sql = @"
                SELECT
                    o.Id,
                    WidgetCount,
                    CreateDate,
                    CompleteDate,
                    Completed,
                    PendingCount,
                    SmashedCount,
                    SlashedCount,
                    CompletedCount
                FROM dbo.Orders o
                INNER JOIN dbo.OrderStatus s
                    on o.Id = s.OrderId";

            return await _conn.QueryAsync<Order>(sql);
        }

        public async Task<IEnumerable<Order>> GetOpenOrders()
        {
            var sql = @"
                SELECT
                     o.Id,
                    WidgetCount,
                    CreateDate,
                    CompleteDate,
                    Completed,
                    PendingCount,
                    SmashedCount,
                    SlashedCount,
                    CompletedCount
                FROM dbo.Orders o
                WHERE o.Completed = 0";

            return await _conn.QueryAsync<Order>(sql);
        }

        public async Task<int> GetOpenOrderCount()
        {
            var sql = @"
                SELECT COUNT(*)
                FROM dbo.Orders o
                WHERE o.Completed = 0";

            var count = await _conn.QueryAsync<int>(sql);

            return count.FirstOrDefault();
        }

        public async Task<Order> GetOrder(int id)
        {
            var sql = @"
                SELECT
                    o.Id,
                    WidgetCount,
                    CreateDate,
                    CompleteDate,
                    Completed,
                    PendingCount,
                    SmashedCount,
                    SlashedCount,
                    CompletedCount
                FROM dbo.Orders o
                WHERE o.id = @id";

            var order = await _conn.QueryAsync<Order>(sql, id);

            return order.FirstOrDefault();
        }

        public async Task<Order> CreateOrder(Order order)
        {
            var orderSql = @"
                INSERT INTO dbo.Orders
                    (WidgetCount,
                    PendingCount)
                OUTPUT INSERTED.*
                VALUES
                    (@WidgetCount,
                    @WidgetCount)";

            var inserted = await _conn.QueryAsync<Order>(orderSql, order);
            return inserted.First();
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            if (order.CompletedCount == order.WidgetCount)
            {
                order.Completed = true;
                order.CompleteDate = DateTime.UtcNow;
            }
            var orderSql = @"
            UPDATE dbo.Orders
            SET
                CompleteDate = @CompleteDate,
                Completed = @Completed,
                PendingCount = @PendingCount,
                SmashedCount = @SmashedCount,
                SlashedCount = @SlashedCount,
                CompletedCount = @CompletedCount
            OUTPUT inserted.*
            WHERE id = @id";

            var result = await _conn.QueryAsync<Order>(orderSql, order);

            return order;
        }
    }
}
