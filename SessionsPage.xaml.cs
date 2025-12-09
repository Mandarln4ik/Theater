using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Theater.DataBase;
using Theater.View;

namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для SessionsPage.xaml
    /// </summary>
    public partial class SessionsPage : Page
    {
        public ObservableCollection<PerformanceView> sessionsView { get; set; } = new ();

        public Dictionary<int, string> Plays = new();
        public Dictionary<int, string> Halls = new();

        public int selectedId = 0;
        public bool isEdit = false;
        public SessionsPage()
        {
            InitializeComponent();
        }

        public async Task CollectSessions(CancellationToken token = default)
        { 
            sessionsView.Clear();
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"SELECT perf.id, perf.play_id AS PlayId, pl.title AS PlayTitle, perf.hall_id, h.name AS HallName, perf.performance_date AS PerformanceDate, perf.price AS Price FROM Performances perf JOIN Plays pl ON perf.play_id = pl.id JOIN Halls h ON perf.hall_id = h.id WHERE ('{ByName.Text}' IS NULL OR '{ByName.Text}' = '' OR pl.title LIKE CONCAT('%', '{ByName.Text}', '%')) AND ('{ByHall.Text}' IS NULL OR'{ByHall.Text}' = '' OR h.name LIKE CONCAT('%', '{ByHall.Text}', '%')) ORDER BY perf.id ASC", conn);

            var reader = await cmd.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    sessionsView.Add(new PerformanceView
                    {
                        Id = reader.GetInt32(0),
                        PlayId = reader.GetInt32(1),
                        PlayTitle = reader.GetString(2),
                        HallId = reader.GetInt32(3),
                        HallName = reader.GetString(4),
                        PerformanceDate = reader.GetDateTime(5),
                        PerformanceDateStr = reader.GetDateTime(5).ToString("yyyy-MM-dd HH:mm:ss"),
                        Price = reader.GetFloat(6)
                    });
                }, System.Windows.Threading.DispatcherPriority.Normal, token);
            }
        }

        public async void CollectDictionary()
        {
            Plays.Clear();
            Halls.Clear();

            var cmd = new MySqlCommand($"SELECT id, title FROM Plays", DBmanager.GetConnection());
            var reader1 = await cmd.ExecuteReaderAsync();

            while (await reader1.ReadAsync())
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Plays.Add(reader1.GetInt32(0), reader1.GetString(1));
                });
            }

            cmd = new MySqlCommand($"SELECT id, name FROM Halls", DBmanager.GetConnection());
            var reader2 = await cmd.ExecuteReaderAsync();

            while (await reader2.ReadAsync())
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Halls.Add(reader2.GetInt32(0), reader2.GetString(1));
                });
            }

            HallCB.Items.Clear();
            PlayCB.Items.Clear();

            foreach (var item in Plays)
            {
                PlayCB.Items.Add(item.Value);
            }
            foreach (var item in Halls)
            {
                HallCB.Items.Add(item.Value);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectSessions();
            CollectDictionary();
            DataContext = this;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            BlurGrid.Visibility = Visibility.Hidden;
            SessionWindow.Visibility = Visibility.Hidden;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            int Hours;
            int Minutes;
            int Seconds;
            string[] strings = new string[3];
            strings = TimeTB.Text.Split(new char[] { ':', '.'});
            if (!int.TryParse(strings[0], out Hours)) { return; }
            if (!int.TryParse(strings[1], out Minutes)) { return; }
            if (!int.TryParse(strings[2], out Seconds)) { return; }

            float price = float.Parse(PriceTB.Text);
            var date = $"{DateP.SelectedDate.Value.Date.Year}-{DateP.SelectedDate.Value.Date.Month}-{DateP.SelectedDate.Value.Date.Day} {Hours}:{Minutes}:{Seconds}";
            var conn = DBmanager.GetConnection();
            MySqlCommand cmd;
            if (isEdit)
            {
                cmd = new MySqlCommand($"UPDATE `Performances` SET `play_id` = '{Plays.First(p => p.Value == PlayCB.SelectedItem).Key}', `hall_id` = '{Halls.First(h => h.Value == HallCB.SelectedItem).Key}', `performance_date` = '{date}',`price` = '{price}' WHERE `Performances`.`id` = {selectedId};", conn);
            }
            else
            {
                cmd = new MySqlCommand($"INSERT INTO `Performances` (`play_id`, `hall_id`, `performance_date`, `price`) VALUES ('{Plays.First(p => p.Value == PlayCB.SelectedItem).Key}', '{Halls.First(h => h.Value == HallCB.SelectedItem).Key}', {date}, {price});", conn);
            }
            cmd.ExecuteNonQuery();
            if (isEdit)
            {
                CollectSessions();
                CollectDictionary();
            }
            BlurGrid.Visibility = Visibility.Hidden;
            SessionWindow.Visibility = Visibility.Hidden;
            isEdit = false;
        }

        private void AddSession_Click(object sender, RoutedEventArgs e)
        {
            BlurGrid.Visibility = Visibility.Visible;
            SessionWindow.Visibility = Visibility.Visible;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            selectedId = ((sender as Button).DataContext as PerformanceView).Id;
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"DELETE FROM Performances WHERE id = {selectedId}", conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sessionsView.Remove(sessionsView.First(s => s.Id == selectedId));
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            isEdit = true;
            BlurGrid.Visibility = Visibility.Visible;
            SessionWindow.Visibility = Visibility.Visible;

            selectedId = ((sender as Button).DataContext as PerformanceView).Id;
            var session = sessionsView.First(s => s.Id == selectedId);
            PlayCB.SelectedItem = Plays[session.PlayId];
            HallCB.SelectedItem = Halls[session.HallId];
            DateP.SelectedDate = session.PerformanceDate;
            TimeTB.Text = session.PerformanceDate.ToString("HH:mm:ss");
            PriceTB.Text = session.Price.ToString();
        }

        CancellationToken CancellationToken = new CancellationToken();
        private CancellationTokenSource _searchCts;
        private async void TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            var token = _searchCts.Token;

            try
            {
                await Task.Delay(500, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (token.IsCancellationRequested) return;

            await Task.Run(async () =>
            {
                await Task.Run(async () =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        CollectSessions(token);
                    }, System.Windows.Threading.DispatcherPriority.Normal, token);
                }, token);

                await CollectSessions(token);
            }, token);
        }
    }
}
