using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theater.View
{
    public class OrderView
    {
        public int PerformanceId { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Hall { get; set; }
        public string Seats { get; set; }
        public string Price { get; set; }
    }
}
