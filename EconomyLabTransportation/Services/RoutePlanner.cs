using EconomyLabTransportation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EconomyLabTransportation.Services
{
    public class RoutePlanner
    {
        private TransportationGraph _graph;
        private List<Car> _vehicles;
        private int _warehouseId;
        private Dictionary<Car,int> VehicleInUsageTime = new Dictionary<Car, int>();
        private int _maxVehicleInUsage = 9;
        private int _nodeCoef = 0;

        public RoutePlanner(TransportationGraph graph, List<Car> vehicles, int warehouseId)
        {
            _graph = graph;
            _vehicles = vehicles;
            _warehouseId = warehouseId;
            foreach(var edge in _graph.Edges)
            {
                _nodeCoef += edge.Value;
            }
            _nodeCoef = _nodeCoef / 2;
            _nodeCoef -= 1;
            foreach (Car vehicle in _vehicles)
            {
                VehicleInUsageTime.Add(vehicle, 0);
            }
        }

        private TransportNode GetNearestNode(TransportNode origin, List<TransportNode> remainingNodes)
        {
            int distance = int.MaxValue;
            TransportNode nearestNode = new TransportNode();
            foreach (var node in remainingNodes)
            {
                if (_graph.GetDistance(origin.Id, node.Id) < distance)
                {
                    distance = _graph.GetDistance(origin.Id, node.Id);
                    nearestNode = node;
                }
            }
            return nearestNode;
        }
        //Жадный алгоритм. Строит маршрут для одной машины по самым ближайшим точкам
        private (Route,int) BuildRoute(List<TransportNode> nodes,Car vehicle,int vehicleRemCapacity)
        {
            var route = new Route(_warehouseId);
            int remainingCapacity = vehicleRemCapacity;
            var remainingNodes = new List<TransportNode>(nodes);

            while (remainingCapacity > 0 || remainingNodes.Count <= 1)
            {
                TransportNode nearestNode = GetNearestNode(_graph.Nodes[route.NodeIds[^1]], remainingNodes);
                if (nearestNode.Id == 0)
                    break; // Нет доступных точек
                if (nearestNode.Value <= remainingCapacity) //Влезет груз
                {
                    route.NodeIds.Add(nearestNode.Id);
                    remainingNodes.Remove(nearestNode);
                    remainingCapacity -= nearestNode.Value;
                }
                else
                {
                    bool flag = true;
                    foreach (var node in remainingNodes)
                    {
                        if(_graph.GetDistance(route.NodeIds[^1], node.Id) < 100000) //Существует путь
                        {
                            if (node.Value <= remainingCapacity)
                            {
                                flag = false;
                                route.NodeIds.Add(node.Id);
                                remainingNodes.Remove(node);
                                remainingCapacity -= node.Value;
                                break;
                            }
                        }
                    }
                    if(flag)
                        break;
                }
                if (VehicleInUsageTime[vehicle] + CalculateTime(route) > _maxVehicleInUsage * 60)
                {
                    route.NodeIds.RemoveAt(route.NodeIds.Count - 1); // Если превышено время использования машины, то просто возвращаемся
                    break;
                }

            }
            route.NodeIds.Add(_warehouseId); // Вернуться на склад
            foreach(var node in route.NodeIds)
            {
                if (node != 0)
                    route.DeliveredStaff += _graph.Nodes[node].Value;
            }
            return (route, remainingCapacity);
        }


        public RoutesSolution PlanRoutes()
        {
            RoutesSolution routes = new RoutesSolution();
            var remainingNodes = new List<TransportNode>(_graph.Nodes);
            remainingNodes.RemoveAll(node => node.Id == _warehouseId); // Удалить склад из списка
            var allowedVehicles = new List<Car>(_vehicles);
            foreach (Car vehicle in _vehicles)
            {
                VehicleInUsageTime[vehicle] = 0;
            }

            while (remainingNodes.Count > 0)
            {
                // Построить маршрут для текущего набора оставшихся точек
                var bestRoute = new Route(_warehouseId);
                Car bestVehicle = null;
                int minCost = int.MaxValue;
                foreach (var vehicle in allowedVehicles)
                {
                    var currentRoute = BuildRoute(remainingNodes, vehicle, vehicle.Capacity);
                    if (currentRoute.Item1.NodeIds.Count > 2)
                    {
                        int cost = CalculateCosts(currentRoute.Item1, vehicle) - currentRoute.Item1.DeliveredStaff * _nodeCoef;

                        if (cost < minCost)
                        {
                            minCost = cost;
                            bestRoute = currentRoute.Item1;
                            bestVehicle = vehicle;
                        }
                    }
                }

                // Удалить обслуженные точки
                foreach (var nodeId in bestRoute.NodeIds)
                {
                    remainingNodes.RemoveAll(node => node.Id == nodeId);
                }
                bestRoute.VehicleId = bestVehicle.Id;
                bestRoute.Cost = CalculateCosts(bestRoute, bestVehicle);
                bestRoute.Time = CalculateTime(bestRoute);
                VehicleInUsageTime[bestVehicle] += bestRoute.Time;
                if (VehicleInUsageTime[bestVehicle] > _maxVehicleInUsage * 60)
                {
                    allowedVehicles.Remove(bestVehicle);
                }
                routes.Routes.Add(bestRoute);
            }
            return routes;
        }

        public int CalculateCosts(Route route, Car car)
        {
            int cost = car.Cost;
            if((((int)Math.Ceiling((double)CalculateTime(route) / 60)) - car.RentTime) > 0)
                cost += (((int)Math.Ceiling((double)CalculateTime(route) / 60)) - car.RentTime) * car.OverCost;
            return cost;
        }

        public int CalculateTime(Route route)
        {
            int val = 0;
            int s = 0;
            foreach (var node in route.NodeIds)
            {
                val += _graph.Nodes[node].Value;
            }
            for(int i =0;i<route.NodeIds.Count-1;i++)
            {
                s += _graph.GetDistance(route.NodeIds[i], route.NodeIds[i + 1]);
            }
            int time = 10 * (route.NodeIds.Count - 1) + 15 * (route.NodeIds.Count - 2) + 10 * (val) + 2*(s);
            return time;
        }
    }

}
