using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using ShortestPath;

namespace WindowsFormsApp
{
    public class ShortPathLayer : Layer
    {
        public ShortPathLayer(string name, Color penColor, int penWidthPercent) : base(name)
        {
            PenColor = penColor;
            PenWidth = penWidthPercent;
        }

        public Color PenColor { get; }
        public int PenWidth { get; }

        protected override void Draw(Graphics g)
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
                        path.StartFigure();
                        path.AddLine(current.X, current.Y, next.X, next.Y);
                        path.CloseFigure();

                        if (!queue.Contains(next))
                        {
                            queue.Enqueue(next);
                        }
                    }
                }
            }

            using (var pen = new Pen(PenColor))
            {
                pen.Width = (float)(PenWidth / 200.0);
                g.DrawPath(pen, path);
            }
        }
    }
}
