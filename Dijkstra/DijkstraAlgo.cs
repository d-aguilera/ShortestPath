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

            var visited = new HashSet<string>();
            CreateQueue();

            Enqueue(target);
            target.DistanceToTarget = 0.0;

            while (QueueIsNotEmpty())
            {
                var current = Dequeue();

                foreach (var segment in current.Edges)
                {
                    var neighbor = segment.To;
                    var distanceToCurrent = segment.Cost;
                    var newDistance = current.DistanceToTarget + distanceToCurrent;
                    var oldDistance = neighbor.DistanceToTarget;

                    if ((float)newDistance > (float)oldDistance)
                    {
                        continue;
                    }

                    if ((float)newDistance < (float)oldDistance)
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

                    var neighborKey = neighbor.ToString();
                    if (!visited.Contains(neighborKey))
                    {
                        Enqueue(neighbor);
                        visited.Add(neighborKey);
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
