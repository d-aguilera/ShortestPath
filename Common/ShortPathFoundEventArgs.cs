namespace ShortestPath
{
    public class ShortPathFoundEventArgs
    {
        public ShortPathFoundEventArgs(Vertex neighbor, Vertex current)
        {
            Neighbor = neighbor;
            Current = current;
        }

        public Vertex Neighbor { get; }
        public Vertex Current { get; }
    }
}
