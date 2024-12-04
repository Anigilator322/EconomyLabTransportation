using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyLabTransportation.Models
{
    public class Route
    {
        public List<int> NodeIds { get; set; } = new List<int>();
        public int Cost { get; set; }
        public int Time { get; set; }
        public int VehicleId { get; set; }

        public Route(int warehouseId)
        {
            NodeIds.Add(warehouseId);
        }

        public Route(List<int> nodeIds, int vehicleId)
        {
            NodeIds = nodeIds;
            VehicleId = vehicleId;
        }
    }
}
