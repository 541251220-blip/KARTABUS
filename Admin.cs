using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KARTABUS.Models
{
    public class Admin : User
    {
        public string Position { get; set; } = "Operator";
    }
}