using EconomyLabTransportation.Models;
using EconomyLabTransportation.Services;
using System.Text.Json;

namespace EconomyLabTransportation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VehicleDeserializer vehicleDeserializer = new VehicleDeserializer();
            TransportGraphDeserializer TransportGraphDeserializer = new TransportGraphDeserializer();
            List<Car> vehicles = vehicleDeserializer.Deserialize(File.ReadAllText("CarConfig.json"));
            TransportationGraph graph = TransportGraphDeserializer.Deserialize(File.ReadAllText("TransportConfig.json"));

            RoutePlanner planner = new RoutePlanner(graph, vehicles, 0);
            
            var routes = planner.PlanRoutes();
            foreach (var route in routes)
            {
                Console.Write("Vehicle: " + route.VehicleId +" Total time in mins: "+route.Time+" Total cost: "+route.Cost +" Route: ");
                Console.WriteLine(string.Join(" -> ", route.NodeIds));
            }
        }
    }
}
