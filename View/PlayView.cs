using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theater.View
{
    public class PlayView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int GenreId { get; set; }
        public string Genre { get; set; }
        public int DirectorId { get; set; }
        public string Director { get; set; }
        public string Age { get; set; } 
        public int Duration { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
