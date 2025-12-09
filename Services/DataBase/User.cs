using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Theater.DataBase;
using Theater.View;

namespace Theater.Services.DataBase
{
    public class User : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ImageUrl
        {
            get;
            set { field = value; OnPropertyChanged(nameof(ImageUrl)); }
        }
        public BitmapImage Image
        {
            get;
            set { field = value; OnPropertyChanged(nameof(Image)); }
        }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtStr { get; set; }
        public string MoneySpend { get; set; }
        public List<Ticket> Tickets
        {
            get;
            set { field = value; OnPropertyChanged(nameof(Tickets)); }
        } = new();
        public ObservableCollection<OrderView> OrderViews { get; set; } = new();

        public User(int userId = -1) 
        {
            if (userId == -1) { return; }
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"SELECT * FROM `Users` WHERE id = {userId}",conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Id = userId;
                    Name = $"{reader.GetString(2)} {reader.GetString(1)}";
                    Email = reader.GetString(3);
                    ImageUrl = reader.GetString(5);
                    Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString(6));
                    CreatedAt = reader.GetDateTime(7);
                }
            }

            if (!String.IsNullOrEmpty(ImageUrl))
            {
                Image = Task.Run(() => NetHelper.GetBitmapAsync(ImageUrl)).Result;
            }

            CreatedAtStr = CreatedAt.ToString("В клубе с dd MMMM yyyy года");

            //cmd = new MySqlCommand($"SELECT id FROM `Tickets` WHERE user_id = {Id}", conn);
            //List<int> ids = new();
            //using (var reader = cmd.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        ids.Add(reader.GetInt32(0));
            //    }
            //}
            //foreach (int id in ids)
            //{
            //    Tickets.Add(new Ticket(id));
            //}
            cmd = new MySqlCommand($"SELECT * FROM `Tickets` WHERE user_id = {Id}", conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Tickets.Add(new Ticket()
                    {
                        Id = reader.GetInt32(0),
                        PerformanceId = reader.GetInt32(1),
                        SeatId = reader.GetInt32(2),
                        UserId = reader.GetInt32(3),
                        Price = reader.GetFloat(4),
                        Status = (TicketStatus)Enum.Parse(typeof(TicketStatus), reader.GetString(5)),
                        CreatedAt = reader.GetDateTime(6)
                    });
                }
            }


            Dictionary<int, List<int>> orders = new Dictionary<int, List<int>>();
            foreach (var item in Tickets)
            {
                if (!orders.ContainsKey(item.PerformanceId))
                {
                    orders.Add(item.PerformanceId, new List<int>());
                }
                orders[item.PerformanceId].Add(item.SeatId);
            }

            foreach(var performanceId in orders)
            {
                var order = new OrderView();
                cmd = new MySqlCommand($"SELECT p.title, perf.price, perf.performance_date, h.name FROM Performances perf INNER JOIN Plays p ON perf.play_id = p.id INNER JOIN Halls h ON perf.hall_id = h.id WHERE perf.id = {performanceId.Key};",conn);
                order.PerformanceId = performanceId.Key;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        order.Title = reader.GetString(0);
                        order.Price = $"{reader.GetFloat(1) * Tickets.Where(t => t.PerformanceId == order.PerformanceId).ToList().Count:F0}";
                        order.Date = reader.GetMySqlDateTime(2).GetDateTime().ToString("dd MMMM yyyy г. в HH:mm");
                        order.Hall = reader.GetString(3);
                    }
                }
                List<string> strs = new List<string>();
                foreach (var seatId in performanceId.Value)
                {
                    cmd = new MySqlCommand($"SELECT `row_number`, `seat_number` FROM `Seats` WHERE id = {seatId}", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            strs.Add($"{reader.GetInt32(0)}.{reader.GetInt32(1)}");
                        }
                    }
                }
                order.Seats = String.Join(",", strs);
                OrderViews.Add(order);
            }

            using (var reader = new MySqlCommand($"SELECT COALESCE(SUM(price), 0) AS TotalSpent FROM Tickets WHERE user_id = {Id};",conn).ExecuteReader())
            {
                while (reader.Read())
                {
                    MoneySpend = $"{reader.GetFloat(0):F0}";
                }
            }

        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            if (propertyName == nameof(ImageUrl))
            {
                if (!String.IsNullOrEmpty(ImageUrl))
                {
                    Image = Task.Run(() => NetHelper.GetBitmapAsync(ImageUrl)).Result;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum UserRole
    {
        user,
        admin
    }
}
