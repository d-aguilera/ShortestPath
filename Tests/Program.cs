using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortestPath.Tests
{
    class Program
    {
        private static Graph graph;
        private static Vertex target;
        private static ShortestPathAlgo algo;

        private static void Main(string[] args)
        {
            algo = new DijkstraWithQueueAlgo();

            Randomize();

            Test1();
        }

        private static void Test1()
        {
            for (var i = 0; i < 1000; i++)
            {
                Recalc();
            }
        }

        private static void Randomize()
        {
            graph = GraphHelper.CreateGraph();
            target = GraphHelper.GetRandomVertex(graph);
        }

        private static void Recalc()
        {
            algo.Process(graph, target);
        }
    }
}
