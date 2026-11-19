using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KARTABUS.Models
{
    public class Seat
    {
        public int SeatId { get; set; }

        public int BusId { get; set; }
        public Bus? Bus { get; set; }

        public string SeatNumber { get; set; } = "";
    }
}