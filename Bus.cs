using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KARTABUS.Models
{
    public class Bus
    {
        public int BusId { get; set; }
        public string BusName { get; set; } = "";
        public string Operator { get; set; } = "";
        public string PlateNumber { get; set; } = "";
        public string BusClass { get; set; } = "";
        public int Capacity { get; set; }
    }
}