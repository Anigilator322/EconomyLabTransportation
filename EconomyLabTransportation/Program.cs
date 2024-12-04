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
            
            /*var routes = planner.PlanRoutes();
            int totalTime = 0;
            int totalCost = 0;
            foreach (var route in routes)
            {
                totalTime += route.Time;
                totalCost += route.Cost;
                Console.Write("Vehicle: " + route.VehicleId +" Route time in mins: "+route.Time+" Route cost: "+route.Cost +" Route: ");
                Console.WriteLine(string.Join(" -> ", route.NodeIds));
            }
            Console.WriteLine("Total time: " + totalTime);
            Console.WriteLine("Total cost: " + totalCost);*/

            Console.WriteLine("VRP Solver");
            
            DataModel dataModel = new DataModel(graph, vehicles);
            var solver = new VRPSolver();
            solver.SolveVRP(dataModel);


        }
    }
}
