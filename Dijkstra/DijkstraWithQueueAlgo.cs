using System.Collections.Generic;
using System.Linq;
using ShortestPath.Common;

namespace ShortestPath
{
    public class DijkstraWithQueueAlgo : ShortestPathAlgo
    {
        public override void Process(Graph graph, Vertex target)
        {
            var queue = new SortedQueue<Vertex, double, int>(
                v => v.DistanceToTarget,
                (v, d) => v.DistanceToTarget = d,
                v => v.Id
            );

            foreach (var vertex in graph)
            {
                vertex.DistanceToTarget = vertex == target ? 0.0 : double.MaxValue;
                vertex.Next = new List<Vertex>();
                queue.Enqueue(vertex);
            }

            while (queue.Count > 0)
            {
                var u = queue.Dequeue();

                foreach (var v in u.Neighbors)
                {
                    var distanceToCurrent = v.DistanceTo(u);
                    var newDistance = u.DistanceToTarget + distanceToCurrent;
                    var oldDistance = v.DistanceToTarget;

                    if (newDistance > oldDistance)
                    {
                        continue;
                    }

                    if (newDistance < oldDistance)
                    {
                        if (queue.Contains(v))
                        {
                            queue.UpdateSortKey(v, newDistance);
                        }
                        else
                        {
                            v.DistanceToTarget = newDistance;
                        }

                        v.Next.Clear();
                    }

                    v.Next.Add(u);
                }
            }
        }
    }
}
