using System.Collections.Generic;

namespace ShortestPath
{
    public abstract class DijkstraAlgo : ShortestPathAlgo
    {
        public override void Process(Graph graph, Vertex target)
        {
            foreach (var vertex in graph)
            {
                vertex.DistanceToTarget = double.MaxValue;
                vertex.Next = null;
            }

            var visited = new HashSet<int>();
            CreateQueue();

            Enqueue(target);
            target.DistanceToTarget = 0.0;

            while (QueueIsNotEmpty())
            {
                var current = Dequeue();

                foreach (var neighbor in current.Neighbors)
                {
                    var distanceToCurrent = neighbor.DistanceTo(current);
                    var newDistance = current.DistanceToTarget + distanceToCurrent;
                    var oldDistance = neighbor.DistanceToTarget;

                    if (newDistance > oldDistance)
                    {
                        continue;
                    }

                    if (newDistance < oldDistance)
                    {
                        SetNeighborDistance(neighbor, newDistance);

                        neighbor.Next = new List<Vertex>
                        {
                            current,
                        };
                    }
                    else
                    {
                        neighbor.Next.Add(current);
                    }

                    if (!visited.Contains(neighbor.Id))
                    {
                        Enqueue(neighbor);
                        visited.Add(neighbor.Id);
                    }
                }
            }
        }

        protected abstract void CreateQueue();

        protected abstract bool QueueIsNotEmpty();

        protected abstract void Enqueue(Vertex target);

        protected abstract Vertex Dequeue();

        protected abstract void SetNeighborDistance(Vertex neighbor, double newDistance);
    }
}
