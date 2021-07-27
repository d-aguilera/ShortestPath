using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ShortestPath;

namespace WindowsFormsApp
{
    public class Layer1 : Layer
    {
        public Layer1(Context context) : base(context)
        {
            Steps = new Action<Graphics>[]
            {
                DrawShortestPath,
            };
        }

        private void DrawShortestPath(Graphics g)
        {
            var path = new GraphicsPath();

            if (Context.Source == null)
            {
                return;
            }

            var queue = new Queue<Vertex>(new[] { Context.Source });

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Next != null)
                {
                    foreach (var next in current.Next)
                    {
                        var x1 = Context.Zoom + current.X * Context.Zoom;
                        var y1 = Context.Zoom + current.Y * Context.Zoom;
                        var x2 = Context.Zoom + next.X * Context.Zoom;
                        var y2 = Context.Zoom + next.Y * Context.Zoom;

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
