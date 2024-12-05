
namespace EconomyLabTransportation.Models
{
    public enum TransportNodeType
    {
        Storage,
        Consumer
    }
    public class TransportNode
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
