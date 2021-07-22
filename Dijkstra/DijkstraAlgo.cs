using System.Collections.Generic;
using System.Linq;

namespace ShortestPath
{
    public class DijkstraAlgo : ShortestPathAlgo
    {
        public override event ShortPathFoundEventHandler ShortPathFound;

        public override void Process(Graph graph, Vertex target)
        {
            var queue = new List<Vertex>();

            foreach (var vertex in graph)
            {
                vertex.DistanceToTarget = double.MaxValue;
                vertex.Next = null;
                queue.Add(vertex);
            }

            target.DistanceToTarget = 0.0;

            while (queue.Count > 0)
            {
                var current = queue.First(x => x.DistanceToTarget == queue.Min(x2 => x2.DistanceToTarget));

                queue.Remove(current);

                foreach (var neighbor in current.Neighbors)
                {
                    var distanceToCurrent = neighbor.DistanceTo(current);
                    var newDistance = current.DistanceToTarget + distanceToCurrent;
                    if (newDistance < double.MaxValue)
                    {
                        if (newDistance < neighbor.DistanceToTarget)
                        {
                            neighbor.DistanceToTarget = newDistance;
                            neighbor.Next = new List<Vertex>
                            {
                                current,
                            };
                            ShortPathFound?.Invoke(this, new ShortPathFoundEventArgs(neighbor, current));
                        }
                        else if (newDistance == neighbor.DistanceToTarget)
                        {
                            neighbor.Next.Add(current);
                            ShortPathFound?.Invoke(this, new ShortPathFoundEventArgs(neighbor, current));
                        }
                    }
                }
            }
        }
    }
}
