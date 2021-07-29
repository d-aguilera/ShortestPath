using System.Collections.Generic;
using System.Linq;

namespace ShortestPath
{
    public class DijkstraBasicAlgo : DijkstraAlgo
    {
        private IList<Vertex> Queue;

        protected override void CreateQueue()
        {
            Queue = new List<Vertex>();
        }

        protected override bool QueueIsNotEmpty() => Queue.Count > 0;

        protected override void Enqueue(Vertex target)
        {
            Queue.Add(target);
        }

        protected override Vertex Dequeue()
        {
            var queueMin = Queue.Min(x2 => x2.DistanceToTarget);
            var current = Queue.First(x => x.DistanceToTarget == queueMin);
            Queue.Remove(current);
            return current;
        }

        protected override void SetNeighborDistance(Vertex neighbor, double newDistance)
        {
            neighbor.DistanceToTarget = newDistance;
        }
    }
}
