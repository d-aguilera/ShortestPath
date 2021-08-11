using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class SegmentsLayer : Layer
    {
        public SegmentsLayer(string name, Color lineColor) : base(name)
        {
            LineColor = lineColor;
        }

        public Color LineColor { get; }

        protected override void Draw(Graphics g)
        {
            var context = Controller.Context;
            var path = new GraphicsPath();

            foreach (var vertex in context.Graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    path.StartFigure();
                    path.AddLine(vertex.X, vertex.Y, next.X, next.Y);
                    path.CloseFigure();
                }
            }

            using (var pen = new Pen(LineColor, 0.025f))
            {
                g.DrawPath(pen, path);
            }
        }
    }
}
