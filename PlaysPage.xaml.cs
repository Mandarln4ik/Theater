using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
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
    /// Логика взаимодействия для PlaysPage.xaml
    /// </summary>
    public partial class PlaysPage : Page
    {
        public ObservableCollection<PlayView> playsView { get; set; } = new();

        public Dictionary<int, string> Genres = new();
        public Dictionary<int, string> Directors = new();

        public int selectedId = 0;
        public bool isEdit = false;

        public PlaysPage()
        {
            InitializeComponent();
        }

        public async Task CollectPlays(CancellationToken token = default)
        {
            playsView.Clear();
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"SELECT p.id, p.title, p.genre_id, g.name AS genreName, p.director_id, CONCAT(d.first_name,' ',d.last_name) AS directorFullname,p.age, p.duration, p.description, p.image_path FROM Plays p LEFT JOIN Genres g ON p.genre_id = g.id LEFT JOIN Directors d ON p.director_id = d.id WHERE ('{ByName.Text}' IS NULL OR '{ByName.Text}' = '' OR p.title LIKE CONCAT('%', '{ByName.Text}', '%')) AND ('{ByGenre.Text}' IS NULL OR '{ByGenre.Text}' = '' OR g.name LIKE CONCAT('%', '{ByGenre.Text}', '%')) ORDER BY p.id ASC", conn);
            var reader = await cmd.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    playsView.Add(new PlayView()
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        GenreId = reader.GetInt32(2),
                        Genre = reader.GetString(3),
                        DirectorId = reader.GetInt32(4),
                        Director = reader.GetString(5),
                        Age = reader.GetString(6),
                        Duration = reader.GetInt32(7),
                        Description = reader.GetString(8),
                        ImageUrl = reader.GetString(9),
                    });
                }, System.Windows.Threading.DispatcherPriority.Normal, token);
            }
        }

        public async void CollectDictionary()
        {
            var conn = DBmanager.GetConnection();
            Genres.Clear();
            Directors.Clear();
            var cmd = new MySqlCommand($"SELECT id, name FROM Genres ORDER BY id;", DBmanager.GetConnection());
            var reader1 = await cmd.ExecuteReaderAsync();

            while (await reader1.ReadAsync())
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Genres.Add(reader1.GetInt32(0), reader1.GetString(1));
                });
            }

            cmd = new MySqlCommand($"SELECT id, CONCAT(first_name, ' ', last_name) AS name FROM Directors ORDER BY id;", DBmanager.GetConnection());
            var reader2 = await cmd.ExecuteReaderAsync();

            while (await reader2.ReadAsync())
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    Directors.Add(reader2.GetInt32(0), reader2.GetString(1));
                });
            }

            GenreCB.Items.Clear();
            DirectorCB.Items.Clear();

            foreach (var item in Genres)
            {
                GenreCB.Items.Add(item.Value);
            }
            foreach (var item in Directors)
            {
                DirectorCB.Items.Add(item.Value);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CollectPlays();
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
            int Duration;
            if (!int.TryParse(DurationTB.Text, out Duration)) { return; }
            var conn = DBmanager.GetConnection();
            MySqlCommand cmd;
            if (isEdit)
            {
                cmd = new MySqlCommand($"UPDATE `Plays` SET `title` = '{TitleTB.Text}', `genre_id` = '{Genres.First(g => g.Value == GenreCB.SelectedItem).Key}', `director_id` = '{Directors.First(g => g.Value == DirectorCB.SelectedItem).Key}', `age` = '{AgeTB.Text}', `Duration` = '{Duration}', `Description` = '{DescriptionTB.Text}', `image_path` = '{UrlTB.Text}' WHERE `Plays`.`id` = {selectedId};", conn);
            }
            else
            {
                cmd = new MySqlCommand($"INSERT INTO `Plays` (`title`, `genre_id`, `director_id`, `age`, `duration`, `description`, `image_path`) VALUES ('{TitleTB.Text}', '{Genres.First(g => g.Value == GenreCB.SelectedItem).Key}', '{Directors.First(g => g.Value == DirectorCB.SelectedItem).Key}','{AgeTB.Text}', '{Duration}', '{DescriptionTB.Text}', '{UrlTB.Text}');", conn);
            }
            cmd.ExecuteNonQuery();
            if (isEdit)
            {
                CollectPlays();
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
            selectedId = ((sender as Button).DataContext as PlayView).Id;
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"DELETE FROM Plays WHERE id = {selectedId}", conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                playsView.Remove(playsView.First(s => s.Id == selectedId));
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            BlurGrid.Visibility = Visibility.Visible;
            SessionWindow.Visibility = Visibility.Visible;

            selectedId = ((sender as Button).DataContext as PlayView).Id;
            var session = playsView.First(s => s.Id == selectedId);
            TitleTB.Text = session.Title;
            GenreCB.SelectedItem = Genres[session.GenreId];
            DirectorCB.SelectedItem = Directors[session.DirectorId];
            AgeTB.Text = session.Age;
            DurationTB.Text = session.Duration.ToString();
            DescriptionTB.Text = session.Description;
            UrlTB.Text = session.ImageUrl;
            isEdit = true;
        }

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
                Dispatcher.InvokeAsync(() =>
                {
                    CollectPlays(token);
                }, System.Windows.Threading.DispatcherPriority.Normal ,token);
            }, token);
        }

    }
}
