using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Theater.Services;

namespace Theater.DataBase
{
    public class Truppa : INotifyPropertyChanged
    {
        public int id {  get; set; }
        public string fullName { get; set; }
        public string birthDate { get; set; }
        public string description { get; set; }
        public string ImageUrl { get; set; }
        public BitmapImage Image
        {
            get;
            set { field = value; OnPropertyChanged(nameof(Image)); }
        }

        public Truppa() 
        {
            if (!String.IsNullOrEmpty(ImageUrl))
            {
                Image = Task.Run(() => NetHelper.GetBitmapAsync(ImageUrl)).Result;
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
