using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Org.BouncyCastle.Tls.Crypto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Theater.DataBase;
using Theater.Properties;
using Theater.Services.DataBase;

namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application, INotifyPropertyChanged
    {
        public static App instance;
        public User currentUser
        {
            get;
            set { field = value; OnPropertyChanged(nameof(currentUser)); }
        }
        public static App GetApp()
        {
            return instance;
        }

        public ObservableCollection<UpcomingSession> upcomingSessions { get; set; } = new();

        public ObservableCollection<Performance> Performances { get; set; } = new();
        public ObservableCollection<Performance> FiltedPerformances
        {
            get;
            set { field = value; OnPropertyChanged(nameof(FiltedPerformances)); }
        } = new();
        public ObservableCollection<Truppa> truppa { get; set; } = new();

        public Performance selectedPerformance
        {
            get;
            set { field = value; OnPropertyChanged(nameof(selectedPerformance)); }
        }

        public AvailableSession selectedSession
        {
            get;
            set { field = value; OnPropertyChanged(nameof(selectedSession)); }
        }

        public ObservableCollection<Seat> selectedSeats { get; set; } = new();

        public ObservableCollection<Review> reviews { get; set; } = new();

        #region Data

        private async Task CollectAllPerformances()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT id FROM `Performances` GROUP BY play_id;", DBmanager.GetConnection());
            List<int> i = new();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    i.Add(reader.GetInt32(0));
                }
            }

            foreach (var item in i)
            {
                Performances.Add(new Performance(item));
            }
        }

        private async Task CollectUpcomingSessions()
        {
            Debug.WriteLine("Sessions");
            var con = DBmanager.GetConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT \r\n    perf.id,\r\n   h.name AS НомерЗала,\r\n    p.title AS НазваниеИгры,\r\n    perf.performance_date AS ДатаНачала \r\nFROM \r\n    Performances perf\r\n    INNER JOIN Halls h ON perf.hall_id = h.id\r\n    INNER JOIN Plays p ON perf.play_id = p.id\r\nWHERE \r\n    perf.performance_date > NOW()\r\n GROUP BY p.title ORDER BY \r\n    perf.performance_date ASC;", con);

            Dispatcher.Invoke(() =>
            {
                upcomingSessions = new ObservableCollection<UpcomingSession>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        upcomingSessions.Add(new UpcomingSession(reader.GetDateTime(3))
                        {
                            Id = reader.GetInt32(0),
                            HallName = reader.GetString(1),
                            PlayTitle = reader.GetString(2)
                        });
                    }
                }
            });
            return;
        }

        private async Task CollectTruppa()
        {
            Debug.WriteLine("Trupa");
            var con = DBmanager.GetConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT first_name, last_name, birth_date, description, image_path FROM `Actors`", con);
            Dispatcher.Invoke(() =>
            {
                truppa = new ObservableCollection<Truppa>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        truppa.Add(new Truppa
                        {
                            fullName = reader.GetString(1) + " " + reader.GetString(0),
                            birthDate = reader.GetString(3),
                            description = reader.GetDateTime(2).ToString("dd.MM.yyyy"),
                            ImageUrl = reader.GetString(4),
                        });
                    }
                }
            });
            return;
        }

        private async Task CollectReviewsForSelectedPlay(int playId)
        {
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"SELECT id FROM `PlayReviews` WHERE play_id = {playId}", conn);
            List<int> ids = new();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add(reader.GetInt32(0));
                }
            }

            foreach (var id in ids)
            {
                Dispatcher.Invoke(() =>
                {
                    reviews.Add(new Review(id));
                });
            }
        }

        public void LoadPerformance(string playTitle, int playId = -1)
        {
            selectedPerformance = Performances.First(p => p.Title == playTitle);
            selectedSession = selectedPerformance.avaibleSession.First();
            selectedSession.IsSelected = true;
            if (playId == -1)
            {
                var conn = DBmanager.GetConnection();
                var cmd = new MySqlCommand($"SELECT id FROM `Plays` WHERE title = '{playTitle}'",conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        playId = reader.GetInt32(0);
                    }
                }
            }
            reviews.Clear();
            Task.Run(() => CollectReviewsForSelectedPlay(playId));
        }

        #endregion

        #region Auth

        public void TryAuth()
        {
            if (currentUser == null)
            {
                //проверяем когда мы были в аккаунте в последний раз
                if (Settings.Default.LastLogIn == null || Settings.Default.PasswordHash == null || Settings.Default.Email == null) return;
                Debug.WriteLine($"{DateTime.Now} - {Settings.Default.LastLogIn} = {(DateTime.Now - Settings.Default.LastLogIn).Days}");
                if (DateTime.Now - Settings.Default.LastLogIn > TimeSpan.FromDays(7))
                {
                    //Выходим из аккаунта
                    Settings.Default.PasswordHash = null;
                }
                else
                {
                    var conn = DBmanager.GetConnection();
                    var cmd = new MySqlCommand($"SELECT id FROM `Users` WHERE email = '{Settings.Default.Email}' AND password_hash = '{Settings.Default.PasswordHash}'", conn);
                    int id = -1;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            id = reader.GetInt32(0);
                        }
                    }
                    if (id != -1)
                    {
                        currentUser = new User(id);
                        Settings.Default.LastLogIn = DateTime.Now;
                        Debug.WriteLine("Успешный вход");
                    }
                }
                Settings.Default.Save();
            }
        }

        private SignUp SignUpWindow;
        public void SignUp()
        {
            if (SignUpWindow != null) SignUpWindow.Close();
            SignUpWindow = new SignUp();
            SignUpWindow.ShowDialog();
            if (SignUpWindow.DialogResult == true)
            {
                currentUser = SignUpWindow.User;
            }
        }

        private SignIn SignInWindow;
        public void SignIn()
        {
            if (SignInWindow != null) SignInWindow.Close();
            SignInWindow = new SignIn();
            SignInWindow.ShowDialog();
            if (SignInWindow.DialogResult == true)
            {
                currentUser = SignInWindow.User;
            }
        }

        public void SignOut()
        {
            currentUser = null;
            Settings.Default.Email = null;
            Settings.Default.PasswordHash = null;
            Settings.Default.LastLogIn = DateTime.MinValue;
            Settings.Default.Save();
        }

        #endregion

        public void Buy()
        {
            var conn = DBmanager.GetConnection();
            foreach (var item in selectedSeats)
            {
                var cmd = new MySqlCommand($"INSERT INTO `Tickets` (`performance_id`, `seat_id`, `user_id`, `price`) VALUES ('{selectedPerformance.Id}','{item.Id}','{currentUser.Id}','{selectedSession.Price}');", conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void Search(string search, string genre, int sortBy) // 0 - От а до я, 1 - От я до а, 2 - Цена по возрастанию, 3 - Цена по убыванию
        {
            List<Performance> filted = new List<Performance>();
            if (!String.IsNullOrEmpty(genre) && genre != "Все жанры")
            {
                filted = Performances.Where(perf => perf.Genre == genre && ( perf.Title.ToLower().Contains(search.ToLower()) || perf.Description.ToLower().Contains(search.ToLower())) ).ToList();
            }
            else
            {
                filted = Performances.Where(perf => perf.Title.ToLower().Contains(search.ToLower()) || perf.Description.ToLower().Contains(search.ToLower())).ToList();
            }
            switch (sortBy)
            {
                case 0:
                    filted = filted.OrderBy(p => p.Title).ToList();
                    break;
                case 1:
                    filted = filted.OrderByDescending(p => p.Title).ToList();
                    break;
                case 2:
                    filted = filted.OrderBy(p => p.Price).ToList();
                    break;
                case 3:
                    filted = filted.OrderByDescending(p => p.Price).ToList();
                    break;
                default:
                    break;
            }
            FiltedPerformances = new ObservableCollection<Performance>(filted);
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            instance = this;
            await CollectAllPerformances();
            await CollectUpcomingSessions();
            await CollectTruppa();
            Search("", "", 0);
            TryAuth();
        }
    }
}
