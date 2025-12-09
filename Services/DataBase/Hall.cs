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

        public Hall(int id, int PerformanceId)
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

            cmd = new MySqlCommand($"SELECT s.id, s.row_number, s.seat_number, IF(t.id IS NOT NULL, TRUE, FALSE) AS IsOccupied FROM Seats s LEFT JOIN Tickets t ON t.seat_id = s.id AND t.performance_id = {PerformanceId} WHERE s.hall_id = {id} ORDER BY s.row_number ASC, s.seat_number ASC", conn);
            Rows.Clear(); // очищаем перед загрузкой

            using (var reader = cmd.ExecuteReader())
            {
                HallRow currentRow = null;
                int lastRowNumber = -1;

                while (reader.Read())
                {
                    int rowNumber = reader.GetInt32(1);

                    if (rowNumber != lastRowNumber)
                    {
                        currentRow = new HallRow { RowNumber = rowNumber };
                        Rows.Add(currentRow);
                        lastRowNumber = rowNumber;
                    }

                    currentRow.Seats.Add(new Seat
                    {
                        Id = reader.GetInt32(0),
                        Name = $"{reader.GetInt32(2)}",
                        RowName = $"{reader.GetInt32(1)}",
                        Status = reader.GetBoolean(3)
                    });
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
