using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyLabTransportation.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int Cost { get; set; }
        public int Capacity { get; set; }
        public int RentTime { get; set; }
        public int OverCost { get; set; }

    }
}
