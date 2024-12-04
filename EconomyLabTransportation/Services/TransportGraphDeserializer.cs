using EconomyLabTransportation.Models;
using System.Text.Json;
using System.Xml.Linq;


namespace EconomyLabTransportation.Services
{
    public class TransportGraphDeserializer
    {
        public TransportationGraph Deserialize(string json)
        {
            TransportationGraph graph = new TransportationGraph();
            using (JsonDocument jsonDoc = JsonDocument.Parse(json))
            {
                JsonElement root = jsonDoc.RootElement;
                if (root.TryGetProperty("Edges", out JsonElement edgesElement))
                {
                    foreach (JsonElement edgeElement in edgesElement.EnumerateArray())
                    {
                        int from = edgeElement.GetProperty("From").GetInt32();
                        int to = edgeElement.GetProperty("To").GetInt32();
                        int weight = edgeElement.GetProperty("Weight").GetInt32();
                        graph.AddEdge(from, to, weight);
                    }
                }
                if(root.TryGetProperty("Nodes", out JsonElement nodesElement))
                {
                    List<TransportNode> nodes = JsonSerializer.Deserialize<List<TransportNode>>(nodesElement.GetRawText());
                    graph.Nodes.AddRange(nodes);
                }
            }
            return graph;
        }
    }
}
