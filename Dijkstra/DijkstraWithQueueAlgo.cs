namespace ShortestPath
{
    public class DijkstraWithQueueAlgo : DijkstraAlgo
    {
        private SortedQueue<Vertex, double, int> Queue;

        protected override void CreateQueue()
        {
            Queue = new SortedQueue<Vertex, double, int>(
                v => v.DistanceToTarget,
                (v, d) => v.DistanceToTarget = d,
                v => v.Id
            );
        }

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
            if (Queue.Contains(neighbor))
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
