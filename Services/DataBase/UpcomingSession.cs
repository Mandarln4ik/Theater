using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Theater.DataBase
{
    public class UpcomingSession : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string HallName { get; set; }
        public string PlayTitle { get; set; }
        public string PerformanceDateTime { get; set; }
        public string TimeRemaining
        {
            get;
            set { field = value; OnPropertyChanged(nameof(TimeRemaining)); }
        }

        public UpcomingSession(DateTime dateTime)
        {
            PerformanceDateTime = dateTime.ToString("dd MMMM HH:mm (yyyy)");
            Task.Run(async () =>
            {
                while (true)
                {
                    TimeSpan remainig = dateTime - DateTime.Now;
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        TimeRemaining = $"До начала: {remainig.Days} дн. {remainig.Hours} ч. {remainig.Minutes} мин.";
                    });
                    await Task.Delay(15000);
                }
            });
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
