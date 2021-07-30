using System;
using System.Collections.Generic;

namespace ShortestPath
{
    public class Vertex : IEquatable<Vertex>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public double DistanceToTarget { get; set; }
        public IList<Vertex> Next { get; set; }
        public IList<Vertex> Neighbors { get; set; }

        public Vertex() : this(0f, 0f)
        {
        }

        public Vertex(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static string GetKey(float x, float y)
        {
            return $"{x},{y}";
        }

        public double DistanceTo(Vertex neighbor)
        {
            return Distance(this, neighbor);
        }

        public static double Distance(Vertex from, Vertex to)
        {
            return Math.Sqrt(Math.Pow(to.X - from.X, 2) + Math.Pow(to.Y - from.Y, 2));
        }

        public override string ToString()
        {
            return GetKey(X, Y);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vertex);
        }

        public bool Equals(Vertex other)
        {
            return other != null && X == other.X && Y == other.Y;
        }
    }
}
