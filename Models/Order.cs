using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Order
    {
        public Order()
        {
        }

        public Order(int widgetCount)
        {
            WidgetCount = widgetCount;
        }

        public int Id { get; set; }
        public int WidgetCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public bool Completed { get; set; }
        public int PendingCount { get; set; }
        public int SmashedCount { get; set; }
        public int SlashedCount { get; set; }
        public int CompletedCount { get; set; }

    }
}
