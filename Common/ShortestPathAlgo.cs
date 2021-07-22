namespace ShortestPath
{
    public abstract class ShortestPathAlgo
    {
        public delegate void ShortPathFoundEventHandler(object sender, ShortPathFoundEventArgs e);

        public abstract event ShortPathFoundEventHandler ShortPathFound;

        public abstract void Process(Graph graph, Vertex target);
    }
}
