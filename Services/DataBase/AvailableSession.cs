using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theater.DataBase
{
    public class AvailableSession : INotifyPropertyChanged
    {
        public int id {  get; set; }
        public string Date {  get; set; }
        public string Time { get; set; }
        public Hall _Hall { get; set; }
        public float Price { get; set; }
        public bool IsSelected
        {
            get;
            set { field = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public AvailableSession(int hallId)
        {
            _Hall = new Hall(hallId);
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
