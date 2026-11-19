using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KARTABUS.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public string PaymentMethod { get; set; } = "";
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "Menunggu";
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}