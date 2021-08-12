using System;
using System.Drawing;
using ShortestPath;

namespace WindowsFormsApp
{
    public class Context
    {
        public double Left { get; set; } = 1.0;
        public double Top { get; set; } = 1.0;
        public GraphicsUnit PageUnit { get; set; } = GraphicsUnit.Pixel;
        public float PageScale { get; set; } = 60f;
        public Graph Graph { get; set; }
        public Vertex Target { get; set; }
        public Vertex Source { get; set; }

        public Func<Rectangle> ClientRectangle { get; set; }
        public Func<Color> BackColor { get; set; }
        public Func<Font> Font { get; set; }
    }
}
