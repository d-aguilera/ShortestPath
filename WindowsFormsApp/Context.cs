using System;
using System.Drawing;
using ShortestPath;

namespace WindowsFormsApp
{
    public class Context
    {
        public double Zoom { get; set; } = 60.0;
        public Graph Graph { get; set; }
        public Vertex Target { get; set; }
        public Vertex Source { get; set; }

        public Func<Color> BackColor { get; set; }
        public Func<Font> Font { get; set; }
        public Func<Size> ClientSize { get; set; }
    }
}
