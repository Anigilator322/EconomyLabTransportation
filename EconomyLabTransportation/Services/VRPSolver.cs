using EconomyLabTransportation.Models;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EconomyLabTransportation.Services
{
    public class VRPSolver
    {
        public void SolveVRP(DataModel data)
        {
            RoutingIndexManager manager = new RoutingIndexManager(data.DistanceMatrix.GetLength(0), data.VehicleNumber, data.Depot);
            RoutingModel routing = new RoutingModel(manager);
            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                // Convert from routing variable Index to
                // distance matrix NodeIndex.
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return data.DistanceMatrix[fromNode, toNode];
            });
            // Define cost of each arc.
            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);
            // Add Capacity constraint.
            int demandCallbackIndex = routing.RegisterUnaryTransitCallback((long fromIndex) =>
            {
                // Convert from routing variable Index to
                // demand NodeIndex.
                var fromNode =
                    manager.IndexToNode(fromIndex);
                return data.Demands[fromNode];
            });
            routing.AddDimensionWithVehicleCapacity(demandCallbackIndex, 0, // null capacity slack
                                                        data.VehicleCapacities, // vehicle maximum capacities
                                                        true,                   // start cumul to zero
                                                        "Capacity");


            for(int i=0;i<data.VehicleBaseCosts.Length;i++)
            {
                routing.SetFixedCostOfVehicle(data.VehicleBaseCosts[i], i);
            }
            

            // Setting first solution heuristic.
            RoutingSearchParameters searchParameters =
                    operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration { Seconds = 1 };

            // Solve the problem.
            Assignment solution = routing.SolveWithParameters(searchParameters);

            // Print solution on console.
            PrintSolution(data, routing, manager, solution);

        }

        /// <summary>
        ///   Print the solution.
        /// </summary>
        public void PrintSolution(DataModel data, RoutingModel routing, RoutingIndexManager manager,
                                  Assignment solution)
        {
            Console.WriteLine($"Objective {solution.ObjectiveValue()}:");

            // Inspect solution.
            long totalDistance = 0;
            long totalLoad = 0;
            for (int i = 0; i < data.VehicleNumber; ++i)
            {
                Console.WriteLine("Route for Vehicle {0}:", i);
                long routeDistance = 0;
                long routeLoad = 0;
                var index = routing.Start(i);
                while (routing.IsEnd(index) == false)
                {
                    long nodeIndex = manager.IndexToNode(index);
                    routeLoad += data.Demands[nodeIndex];
                    Console.Write("{0} Load({1}) -> ", nodeIndex, routeLoad);
                    var previousIndex = index;
                    index = solution.Value(routing.NextVar(index));
                    routeDistance += routing.GetArcCostForVehicle(previousIndex, index, i);
                }
                Console.WriteLine("{0}", manager.IndexToNode((int)index));
                Console.WriteLine("Distance of the route: {0}m", routeDistance);
                totalDistance += routeDistance;
                totalLoad += routeLoad;
            }
            Console.WriteLine("Total distance of all routes: {0}m", totalDistance);
            Console.WriteLine("Total load of all routes: {0}m", totalLoad);
        }
    }
}
