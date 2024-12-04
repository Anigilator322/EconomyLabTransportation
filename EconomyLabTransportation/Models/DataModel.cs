using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomyLabTransportation.Models
{
    public class DataModel
    {
        public long[,] DistanceMatrix;
        public long[] Demands;
        public long[] VehicleCapacities;
        public long[] VehicleBaseCosts;
        public int VehicleNumber;
        public int Depot = 0;

        public DataModel(TransportationGraph graph, List<Car> vehicles)
        {
            DistanceMatrix = new long[graph.Nodes.Count, graph.Nodes.Count];
            Demands = new long[graph.Nodes.Count];
            for (int i = 0; i < graph.Nodes.Count; ++i)
            {
                for (int j = 0; j < graph.Nodes.Count; ++j)
                {
                    DistanceMatrix[i, j] = graph.GetDistance(graph.Nodes[i].Id, graph.Nodes[j].Id);
                }
                Demands[i] = graph.Nodes[i].Value;
            }

            VehicleCapacities = new long[vehicles.Count];
            VehicleBaseCosts = new long[vehicles.Count];
            VehicleNumber = vehicles.Count;
            for (int i = 0; i < vehicles.Count; ++i)
            {
                VehicleCapacities[i] = vehicles[i].Capacity;
                VehicleBaseCosts[i] = vehicles[i].Cost;
            }

            for(int i = 0; i< DistanceMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < DistanceMatrix.GetLength(1); j++)
                {
                    Console.Write(DistanceMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
