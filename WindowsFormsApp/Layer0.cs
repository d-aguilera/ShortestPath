using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class Layer0 : Layer
    {
        public Layer0(Context context) : base(context)
        {
            Steps = new Action<Graphics>[]
            {
                DrawConnectors,
            };
        }

        private void DrawConnectors(Graphics g)
        {
            var path = new GraphicsPath();

            foreach (var vertex in Context.Graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = Context.Zoom + vertex.X * Context.Zoom;
                    var y1 = Context.Zoom + vertex.Y * Context.Zoom;
                    var x2 = Context.Zoom + next.X * Context.Zoom;
                    var y2 = Context.Zoom + next.Y * Context.Zoom;

                    path.StartFigure();
                    path.AddLine((float)x1, (float)y1, (float)x2, (float)y2);
                    path.CloseFigure();
                }
            }

            g.DrawPath(Pens.Black, path);
        }
    }
}
