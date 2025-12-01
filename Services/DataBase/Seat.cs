using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theater.DataBase
{
    public class Seat : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RowName { get; set; }
        public SeatStatus SeatStatus
        {
            get;
            set { field = value; OnPropertyChanged(nameof(SeatStatus)); }
        }

        public bool Status
        {
            get;
            set { field = value; OnPropertyChanged(nameof(Status)); }
        }


        public Seat()
        {
            if (Status)
            {
                SeatStatus = SeatStatus.Occupied;
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
