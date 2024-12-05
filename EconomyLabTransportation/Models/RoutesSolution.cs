
namespace EconomyLabTransportation.Models
{
    public class RoutesSolution
    {
        public List<Route> Routes { get; set; } = new List<Route>();
        public Dictionary<Car, int> VehicleInUsageTime { get; set; } = new Dictionary<Car, int>();
    }
}
