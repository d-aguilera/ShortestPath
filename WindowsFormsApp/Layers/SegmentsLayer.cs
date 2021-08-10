using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class SegmentsLayer : Layer
    {
        public SegmentsLayer(string name) : base(name)
        {
        }

        protected override void Draw(Graphics g)
        {
            var context = Controller.Context;
            var path = new GraphicsPath();

            foreach (var vertex in context.Graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = context.Zoom + vertex.X * context.Zoom;
                    var y1 = context.Zoom + vertex.Y * context.Zoom;
                    var x2 = context.Zoom + next.X * context.Zoom;
                    var y2 = context.Zoom + next.Y * context.Zoom;

                    path.StartFigure();
                    path.AddLine((float)x1, (float)y1, (float)x2, (float)y2);
                    path.CloseFigure();
                }
            }

            g.DrawPath(Pens.Black, path);
        }
    }
}
