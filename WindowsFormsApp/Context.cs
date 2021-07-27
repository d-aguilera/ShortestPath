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
        public Font Font { get; set; }
    }
}
