using System;
using System.Drawing;
using System.Linq;

namespace ShortestPath
{
    public class GraphHelper
    {
        private static Random random = new Random();

        public static Graph CreateGraph()
        {
            const int sizeX = 31;
            const int sizeY = 16;

            var graph = new Graph();

            var offsets = Enumerable.Empty<PointF>()
                .Concat(new[] { -1, 0, 1 }.SelectMany(y => new[] { -1, 0, 1 }.Select(x => new PointF(x, y))))
                .Concat(new[] { -2, 2 }.SelectMany(y => new[] { -1, 1 }.Select(x => new PointF(x, y))))
                .Concat(new[] { -1, 1 }.SelectMany(y => new[] { -2, 2 }.Select(x => new PointF(x, y))))
                .Where(p => p.X != 0 || p.Y != 0)
                .ToArray();

            // add all vertices
            graph.AddRange(Enumerable.Range(0, sizeY)
                .SelectMany(y => Enumerable.Range(0, sizeX).Select(x => new Vertex(x, y))));

            // remove some to create obstacles
            var obstacleOffsets = offsets
                .Concat(new[] { -2, 2 }.Select(x => new PointF(x, 0)))
                .Concat(new[] { -2, 2 }.Select(y => new PointF(0, y)))
                .ToArray();

            for (var i = 0; i < 10; i++)
            {
                var index = random.Next(graph.Count);
                var vertex = graph.Skip(index).First();
                var toRemove = obstacleOffsets
                    .Select(offset => graph[vertex.X + offset.X, vertex.Y + offset.Y])
                    .Where(v => v != null)
                    .Concat(new[] { vertex });

                foreach (var item in toRemove)
                {
                    graph.Remove(item);
                }
            }

            // initialize neighbors
            foreach (var vertex in graph)
            {
                vertex.Neighbors = offsets
                    .Select(offset => graph[vertex.X + offset.X, vertex.Y + offset.Y])
                    .Where(v => v != null)
                    .ToList();
            }

            return graph;
        }

        public static Vertex GetRandomVertex(Graph graph)
        {
            return graph.Skip(random.Next(graph.Count)).First();
        }
    }
}
