using EconomyLabTransportation.Models;
using System.Text.Json;

namespace EconomyLabTransportation.Services
{
    public class VehicleDeserializer
    {
        public List<Car> Deserialize(string json)
        {
            List<Car> vehicles = new List<Car>();
            using (JsonDocument jsonDoc = JsonDocument.Parse(json))
            {
                JsonElement root = jsonDoc.RootElement;
                if (root.TryGetProperty("AvailableCars", out JsonElement edgesElement))
                {
                    foreach (JsonElement edgeElement in edgesElement.EnumerateArray())
                    {
                        int id = edgeElement.GetProperty("Id").GetInt32();
                        int cost = edgeElement.GetProperty("Cost").GetInt32();
                        int rentTime = edgeElement.GetProperty("RentTime").GetInt32();
                        int overCost = edgeElement.GetProperty("OverCost").GetInt32();
                        int capacity = edgeElement.GetProperty("Capacity").GetInt32();
                        vehicles.Add(new Car { Id = id, Cost = cost, RentTime = rentTime, OverCost = overCost, Capacity = capacity });
                    }
                }
            }
            return vehicles;
        }
        
    }
}
