using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.DTOModels
{
    public class ReservationSummary
    {

        public int Id { get; set; }
        public DateTime ReservationTime { get; set; }
        public string CustomerName { get; set; }
        public string MovieName { get; set; }
        public string Email { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public string Phone { get; set; }
        public DateTime PlayingDate { get; set; }
        public DateTime PlayingTime { get; set; }
    }
}
