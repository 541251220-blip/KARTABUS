using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KARTABUS.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        public int BusId { get; set; }
        public Bus? Bus { get; set; }

        public int RouteId { get; set; }
        public Route? Route { get; set; }

        public DateTime DepartureTime { get; set; }
        public decimal Price { get; set; }
    }
}