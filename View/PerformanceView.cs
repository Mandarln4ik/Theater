using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theater.View
{
    public class PerformanceView
    {
        public int Id { get; set; }
        public int PlayId { get; set; }
        public string PlayTitle { get; set; }
        public int HallId { get; set; }
        public string HallName { get; set; }
        public DateTime PerformanceDate { get; set; }
        public string PerformanceDateStr { get; set; }
        public float Price { get; set; }
    }
}
