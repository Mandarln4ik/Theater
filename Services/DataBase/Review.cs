using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theater.DataBase;

namespace Theater.Services.DataBase
{
    public class Review : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int PlayId { get; set; }
        public int UserId { get; set; }
        public User _User
        {
            get;
            set { field = value; OnPropertyChanged(nameof(_User)); }
        }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public MySqlDateTime ReviewDate { get; set; }
        public string DateStr { get; set; }

        public Review(int reviewId)
        {
            var conn = DBmanager.GetConnection();
            var cmd = new MySqlCommand($"SELECT * FROM `PlayReviews` WHERE id = {reviewId}",conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Id = reviewId;
                    PlayId = reader.GetInt32(1);
                    UserId = reader.GetInt32(2);
                    Rating = reader.GetInt32(3);
                    Comment = reader.GetString(4);
                    ReviewDate = reader.GetMySqlDateTime(5);
                }
            }
            DateStr = ReviewDate.GetDateTime().ToString("dd MMMM yyyyг");
            _User = new User(UserId);
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
