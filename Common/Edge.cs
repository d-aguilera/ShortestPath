namespace ShortestPath
{
    public class Edge
    {
        public Vertex To { get; }
        public double Cost { get; }

        public Edge(Vertex to, double cost)
        {
            To = to;
            Cost = cost;
        }
    }
}
