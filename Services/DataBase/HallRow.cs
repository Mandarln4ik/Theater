using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theater.DataBase
{
    public class HallRow
    {
        public int RowNumber { get; set; }
        public ObservableCollection<Seat> Seats { get; set; } = new ObservableCollection<Seat>();
    }
}
