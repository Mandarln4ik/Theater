using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    public partial class BookingPage : Page  , INotifyPropertyChanged
    {

        public BookingPage()
        {
            InitializeComponent();
            btns.Add(Star1);
            btns.Add(Star2);
            btns.Add(Star3);
            btns.Add(Star4);
            btns.Add(Star5);
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
            WarnWindow.Visibility = Visibility.Hidden;
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

            if (Seat.Status)
            {
                btn.IsChecked = false;
                return;
            }

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
            var s = App.GetApp().Buy();
            if (s)
            {
                BlurGrid.Visibility = Visibility.Hidden;
                BuyWindow.Visibility = Visibility.Hidden;
            }
        }

        private void GoToBuy_Click(object sender, RoutedEventArgs e)
        {
            if (App.GetApp().currentUser == null) 
            {
                WarnWindow.Visibility = Visibility.Visible;
                BlurGrid.Visibility = Visibility.Visible;
                return;
            }
            if (App.GetApp().selectedSeats.Count > 0)
            {
                BlurGrid.Visibility = Visibility.Visible;
                BuyWindow.Visibility = Visibility.Visible;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            BlurGrid.Visibility = Visibility.Hidden;
            WarnWindow.Visibility = Visibility.Hidden;
        }

        private void SendReview_Click(object sender, RoutedEventArgs e)
        {
            if (App.GetApp().currentUser == null)
            {
                WarnWindow.Visibility = Visibility.Visible;
                BlurGrid.Visibility = Visibility.Visible;
                return;
            }
            if (!String.IsNullOrEmpty(ReviewText.Text))
            {
                var app = App.GetApp();
                var conn = DBmanager.GetConnection();
                var cmd = new MySqlCommand($"INSERT INTO `PlayReviews` (`play_id`, `user_id`, `rating`, `comment`, `review_date`) VALUES ('{app.selectedPerformance.PlayId}', '{app.currentUser.Id}', '{rating}', '{ReviewText.Text}', current_timestamp());", conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return;
                }
                ReviewText.Text = String.Empty;
                rating = 0;
                foreach (var item in btns)
                {
                    (item.Content as Image).Source = starE;
                }
                app.ReloadPerformance();
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


        public int rating = 0;
        public List<Button> btns = new();
        private ImageSource starF = new BitmapImage(new Uri("pack://Application:,,,/icons/star_f.png"));
        private ImageSource starE = new BitmapImage(new Uri("pack://Application:,,,/icons/star_e.png"));
        private void Star_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag.ToString());
            rating = id;
            foreach (var item in btns)
            {
                (item.Content as Image).Source = starE;
            }

            for (int i = 0; i < id; i++)
            {
                (btns[i].Content as Image).Source = starF;
            }

        }
    }
}
