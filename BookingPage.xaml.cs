using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Theater.DataBase;
using Nav = Theater.Services.NavigationService;
namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Page
    {

        public BookingPage()
        {
            InitializeComponent();
            
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            Nav.Instance.NavigateTo(0);
            var app = App.GetApp();
            app.selectedPerformance = null;
            app.selectedSeats.Clear();
            app.selectedSession = null;
            FullCost.Text = "0";
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            DataContext = App.GetApp();
            BlurGrid.Visibility = Visibility.Hidden;
            BuyWindow.Visibility = Visibility.Hidden;
        }

        private void SelectSession_Click(object sender, RoutedEventArgs e)
        {
            var app = App.GetApp();
            var _avaibleSession = (sender as Button).DataContext as AvailableSession;
            AvailableSession s = app.selectedPerformance.avaibleSession.First(avaibleSession => avaibleSession == _avaibleSession);
            foreach (var item in app.selectedPerformance.avaibleSession)
            {
                item.IsSelected = false;
            }
            s.IsSelected = true;
            app.selectedSession = s;
            foreach (var item in app.selectedSeats)
            {
                item.SeatStatus = SeatStatus.Available;
            }
            app.selectedSeats.Clear();
        }

        private void SeatBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (sender as ToggleButton);
            Seat Seat = btn.DataContext as Seat;
            Seat.SeatStatus = btn.IsChecked.Value ? SeatStatus.Selected : SeatStatus.Available;
            if (Seat.SeatStatus == SeatStatus.Selected)
            {
                App.GetApp().selectedSeats.Add(Seat);
            }
            else
            {
                if (App.GetApp().selectedSeats.Contains(Seat))
                {
                    App.GetApp().selectedSeats.Remove(Seat);
                }
            }
            FullCost.Text = $"{(App.GetApp().selectedSession.Price * App.GetApp().selectedSeats.Count):F2}";
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            App.GetApp().Buy();
        }

        private void GoToBuy_Click(object sender, RoutedEventArgs e)
        {
            BlurGrid.Visibility = Visibility.Visible;
            BuyWindow.Visibility = Visibility.Visible;
        }
    }
}
