using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theater.DataBase
{
    public class Hall : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<HallRow> Rows { get; set; } = new ObservableCollection<HallRow>();

        public Hall(int id)
        {
            int rowsCount = 0;
            int seatsInRow = 0;
            var conn = DBmanager.GetConnection();
            MySqlCommand cmd = new MySqlCommand($"SELECT `name`, `rows`, `seats_in_row` FROM Halls WHERE id = {id}", conn);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Id = id;
                    Name = reader.GetString(0);
                    rowsCount = reader.GetInt32(1);
                    seatsInRow = reader.GetInt32(2);
                }
            }

            cmd = new MySqlCommand($"SELECT `id`, `seat_number`, `status` FROM Seats WHERE hall_id = {id} ORDER BY `row_number` DESC, `seat_number` ASC;", conn);
            using (var reader = cmd.ExecuteReader())
            {
                for (int r = 0; r < rowsCount; r++)
                {
                    HallRow hallRow = new HallRow();
                    hallRow.RowNumber = r + 1;
                    for (int s = 0; s < seatsInRow; s++)
                    {
                        if (reader.Read())
                        {
                            hallRow.Seats.Add(new Seat
                            {
                                Id = reader.GetInt32(0),
                                Name = $"{reader.GetInt32(1)}", RowName = $"{r + 1}",
                                Status = reader.GetBoolean(2)
                            });
                        }
                    }
                    Rows.Add(hallRow);
                }
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
