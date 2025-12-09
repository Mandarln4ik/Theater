using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theater.DataBase;

namespace Theater.Services.DataBase
{
    public class Ticket
    {
        public int Id { get; set; }
        public int PerformanceId { get; set; }
        public int SeatId { get; set; }
        public int UserId { get; set; }
        public float Price { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        

        public Ticket(/*int id*/)
        {
            //var conn = DBmanager.GetConnection();
            //var cmd = new MySqlCommand($"SELECT * FROM `Tickets` WHERE id = {id}", conn);
            //using (var reader = cmd.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        Id = reader.GetInt32(0);
            //        PerformanceId = reader.GetInt32(1);
            //        SeatId = reader.GetInt32(2);
            //        UserId = reader.GetInt32(3);
            //        Price = reader.GetFloat(4);
            //        Status = (TicketStatus)Enum.Parse(typeof(TicketStatus), reader.GetString(5));
            //        CreatedAt = reader.GetDateTime(6);
            //    }
            //}
        }
    }

    public enum TicketStatus
    {
        Pending,
        Cancelled,
        Paid
    }
}
