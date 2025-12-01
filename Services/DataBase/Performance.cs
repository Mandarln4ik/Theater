using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Theater.Services;

namespace Theater.DataBase
{
    public class Performance : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public int DurationMinutes { get; set; }
        public float Rating { get; set; }
        public float Price { get; set; }
        public string PriceStr { get; set; }
        public string ImageUrl { get; set; }
        public BitmapImage Image
        {
            get;
            set { field = value; OnPropertyChanged(nameof(Image)); }
        }

        public ObservableCollection<AvailableSession> avaibleSession
        {
            get;
            set { field = value; OnPropertyChanged(nameof(avaibleSession)); }
        } = new();

        public Performance(int PerformanceId)
        {
            Id = PerformanceId;
            int playId = 0;
            int hallId = 0;
            var conn = DBmanager.GetConnection();
            MySqlCommand cmd = new MySqlCommand($"SELECT `play_id`, `hall_id`FROM Performances WHERE `id` = {PerformanceId}", conn);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    playId = reader.GetInt32(0);
                    hallId = reader.GetInt32(1);
                }
            }
            int genreId = 0;
            int directorId = 0;
            cmd = new MySqlCommand($"SELECT p.title, p.genre_id, p.director_id, p.duration, p.description, p.image_path, COALESCE(AVG(pr.Rating), 0.0) AS avg_rating FROM Plays p LEFT JOIN Performances perf ON perf.play_id = p.id LEFT JOIN PlayReviews pr ON pr.play_id = perf.id WHERE p.id = {playId} GROUP BY p.id, p.title, p.genre_id, p.director_id, p.duration, p.description, p.image_path", conn);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Title = reader.GetString(0);
                    genreId = reader.GetInt32(1);
                    directorId = reader.GetInt32(2);
                    DurationMinutes = reader.GetInt32(3);
                    Description = reader.GetString(4);
                    ImageUrl = reader.GetString(5);
                    Rating = reader.GetFloat(6);
                }
            }
            cmd = new MySqlCommand($"SELECT `name` FROM Genres WHERE `id` = {genreId}", conn);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Genre = reader.GetString(0);
                }
            }
            cmd = new MySqlCommand($"SELECT `first_name`, `last_name` FROM Directors WHERE `id` = {directorId}", conn);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Director = $"{reader.GetString(1)} {reader.GetString(0)}";
                }
            }
            cmd = new MySqlCommand($"SELECT \r\n    perf.id AS id,\r\n    perf.hall_id AS hall_id,\r\n    perf.performance_date AS Performance_date, perf.price \r\nFROM \r\n    Performances perf\r\n    INNER JOIN Halls h ON perf.hall_id = h.id\r\nWHERE \r\n    perf.play_id = {playId};", conn);
            List<(int, int, string, string, float)> data = new();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    data.Add(new (reader.GetInt32(0), reader.GetInt32(1), reader.GetMySqlDateTime(2).GetDateTime().ToString("dd MMMM - ddd (yyyy)"), reader.GetMySqlDateTime(2).GetDateTime().ToString("HH:mm"), reader.GetFloat(3)));
                }
            }

            foreach (var i in data)
            {
                avaibleSession.Add(
                        new AvailableSession(i.Item2)
                        {
                            id = i.Item1,
                            Date = i.Item3,
                            Time = i.Item4,
                            Price = i.Item5
                        });
            }
            float min = data.Min(item => item.Item5);
            float max = data.Max(item => item.Item5);
            Price = min;
            if (min == max)
            {
                PriceStr = $"{min:F1}";
            }
            else
            {
                PriceStr = $"{min:F1} - {max:F1}";
            }

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
