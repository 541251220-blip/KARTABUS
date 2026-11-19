using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KARTABUS.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = "";

        public int PassengerId { get; set; }
        public Passenger? Passenger { get; set; }

        public int ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

        public int SeatId { get; set; }
        public Seat? Seat { get; set; }

        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Menunggu Pembayaran";
        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}
