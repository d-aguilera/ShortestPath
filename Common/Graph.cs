using System.Collections;
using System.Collections.Generic;

namespace ShortestPath
{
    public class Graph : ICollection<Vertex>
    {
        private IDictionary<string, Vertex> graph = new Dictionary<string, Vertex>();

        private static string GetKey(Vertex item)
        {
            return item.ToString();
        }

        public Vertex this[float x, float y]
        {
            get
            {
                var key = Vertex.GetKey(x, y);
                return graph.ContainsKey(key) ? graph[key] : null;
            }
        }

        public int Count => graph.Count;

        public bool IsReadOnly => graph.IsReadOnly;

        public void Add(Vertex item)
        {
            graph.Add(GetKey(item), item);
        }

        public void AddRange(IEnumerable<Vertex> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            graph.Clear();
        }

        public bool Contains(Vertex item)
        {
            return graph.ContainsKey(GetKey(item));
        }

        public void CopyTo(Vertex[] array, int arrayIndex)
        {
            graph.Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Vertex> GetEnumerator()
        {
            return graph.Values.GetEnumerator();
        }

        public Vertex RemoveAt(int x, int y)
        {
            var item = this[x, y];
            if (Remove(item))
            {
                return item;
            }
            return null;
        }

        public bool Remove(Vertex item)
        {
            return graph.Remove(GetKey(item));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
