using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Theater.DataBase;

namespace Theater.Services
{
    public class NavigationService
    {
        public static NavigationService Instance;
        private MainWindow window;
        private List<Frame> _frames;

        public NavigationService(MainWindow mainWindow)
        {
            Instance = this;
            window = mainWindow;
            _frames = new List<Frame>();
        }

        public void Add(Frame frame)
        {
            _frames.Add(frame);
        }

        public void Add(List<Frame> frames)
        {
            foreach (var item in frames)
            {
                _frames.Add(item);
            }
        }

        public void Remove(Frame frame)
        {
            _frames.Remove(frame);
        }

        public void NavigateTo(Frame frame)
        {
            foreach (var item in _frames)
            {
                if (item == frame)
                {
                    item.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    item.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        public void NavigateTo(int frameId)
        {
            foreach (var item in _frames.Select((value, index) => new { value, index }))
            {
                if (item.index == frameId)
                {
                    item.value.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    item.value.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
