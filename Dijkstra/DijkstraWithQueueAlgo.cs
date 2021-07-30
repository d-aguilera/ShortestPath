namespace ShortestPath
{
    public class DijkstraWithQueueAlgo : DijkstraAlgo
    {
        private SortedQueue<Vertex, double, string> Queue;

        protected override void CreateQueue()
        {
            Queue = new SortedQueue<Vertex, double, string>(GetDistanceToTarget, SetDistanceToTarget, GetVertexKey);
        }

        private double GetDistanceToTarget(Vertex vertex) => vertex.DistanceToTarget;

        private void SetDistanceToTarget(Vertex vertex, double newDistance)
        {
            vertex.DistanceToTarget = newDistance;
        }

        private string GetVertexKey(Vertex vertex) => vertex.ToString();

        protected override bool QueueIsNotEmpty() => Queue.Count > 0;

        protected override void Enqueue(Vertex target)
        {
            Queue.Enqueue(target);
        }

        protected override Vertex Dequeue()
        {
            return Queue.Dequeue();
        }

        protected override void SetNeighborDistance(Vertex neighbor, double newDistance)
        {
            if (Queue.ContainsKey(neighbor.ToString()))
            {
                Queue.UpdateSortKey(neighbor, newDistance);
            }
            else
            {
                neighbor.DistanceToTarget = newDistance;
            }
        }
    }
}
