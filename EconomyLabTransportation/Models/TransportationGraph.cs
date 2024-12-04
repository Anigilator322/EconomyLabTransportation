namespace EconomyLabTransportation.Models
{
    public class TransportationGraph
    {
        public List<TransportNode> Nodes { get; set; } = new List<TransportNode>();
        public Dictionary<(int,int),int> Edges { get; set; } = new Dictionary<(int, int), int>();

        public void AddEdge(int from, int to, int weight)
        {
            Edges[(from, to)] = weight;
            Edges[(to, from)] = weight;
        }


        public int GetDistance(int from, int to)
        {
            return Edges.ContainsKey((from, to)) ? Edges[(from, to)] : int.MaxValue;
        }
    }
}
