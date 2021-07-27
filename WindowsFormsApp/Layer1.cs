using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ShortestPath;

namespace WindowsFormsApp
{
    public class Layer1 : Layer
    {
        protected override void Draw(Graphics g)
        {
            DrawShortestPath(g);
        }

        private void DrawShortestPath(Graphics g)
        {
            var context = Controller.Context;

            if (context.Source == null)
            {
                return;
            }

            var queue = new Queue<Vertex>(new[] { context.Source });
            var path = new GraphicsPath();

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Next != null)
                {
                    foreach (var next in current.Next)
                    {
                        var x1 = context.Zoom + current.X * context.Zoom;
                        var y1 = context.Zoom + current.Y * context.Zoom;
                        var x2 = context.Zoom + next.X * context.Zoom;
                        var y2 = context.Zoom + next.Y * context.Zoom;

                        path.StartFigure();
                        path.AddLine((float)x1, (float)y1, (float)x2, (float)y2);
                        path.CloseFigure();

                        if (!queue.Contains(next))
                        {
                            queue.Enqueue(next);
                        }
                    }
                }
            }

            using (var pen = new Pen(Color.Red))
            {
                pen.Width = 4;
                g.DrawPath(pen, path);
            }
        }
    }
}
