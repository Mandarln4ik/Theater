using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Theater.DataBase;
using Nav = Theater.Services.NavigationService;

namespace Theater
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public ObservableCollection<string> genres { get; set; } = new ObservableCollection<string>();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoad(object sender, RoutedEventArgs e)
        {
            DataContext = App.GetApp();
            await CollectGenres();
        }

        private async Task CollectGenres()
        {
            genres.Add("Все жанры");
            var conn = DBmanager.GetConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT name FROM `Genres` GROUP BY name", conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    genres.Add(reader.GetString(0));
                }
            }
            FilterByGenre.ItemsSource = genres;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
            {
                scrollviewer.LineLeft();
                scrollviewer.LineLeft();
            }
            else
            {
                scrollviewer.LineRight();
                scrollviewer.LineRight();
            }
            e.Handled = true;
        }

        private void UpcomingSessionButton_Click(object sender, RoutedEventArgs e)
        {
            Nav.Instance.NavigateTo(2);
            var session = (sender as Button).DataContext as UpcomingSession;
            if (session == null)
            {
                var session1 = (sender as Button).DataContext as Performance;
                if (session1 != null)
                {
                    var app = App.GetApp();
                    app.LoadPerformance(session1.Title);
                    Nav.Instance.NavigateTo(2); // Navigate to BookingFrame
                }
            }
            if (session != null)
            {
                var app = App.GetApp();
                app.LoadPerformance(session.PlayTitle);
                Nav.Instance.NavigateTo(2); // Navigate to BookingFrame
            }
        }

        #region Sort, Filter, Search

        public string searchText = "";
        public string selectedGenre = "";
        public int selectedSort = 0;

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = (sender as TextBox).Text;
            ApplyFilters();
        }

        private void FilterByGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedGenre = FilterByGenre.SelectedItem.ToString();
            ApplyFilters();
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSort = SortBy.SelectedIndex;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var app = App.GetApp();
            app.Search(searchText, selectedGenre, selectedSort);
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            FilterByGenre.SelectedIndex = 0;
            SortBy.SelectedIndex = 0;
        }

        #endregion
    }
}
