using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class SegmentLengthsLayer : Layer
    {
        protected override void Draw(Graphics g)
        {
            const float size = 1.4f;

            var context = Controller.Context;
            var gap = context.Zoom / 2.4;
            var path = new GraphicsPath();

            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near,
            };

            foreach (var vertex in context.Graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = context.Zoom + vertex.X * context.Zoom;
                    var y1 = context.Zoom + vertex.Y * context.Zoom;
                    var x2 = context.Zoom + next.X * context.Zoom;
                    var y2 = context.Zoom + next.Y * context.Zoom;

                    var angle = Math.Atan2(y2 - y1, x2 - x1);

                    var length = vertex.DistanceTo(next);
                    var x = x1 + gap * Math.Cos(angle);
                    var y = y1 + gap * Math.Sin(angle);
                    var origin = new PointF((float)x, (float)y);

                    var matrix = new Matrix();
                    var degrees = angle * 180.0 / Math.PI;
                    matrix.RotateAt((float)degrees, origin);

                    var path2 = new GraphicsPath();
                    path2.AddString(string.Format("{0:0.##}", length), context.Font().FontFamily, (int)context.Font().Style, context.Font().SizeInPoints * size, origin, format);
                    path2.Transform(matrix);

                    path.AddPath(path2, false);
                }
            }

            g.FillPath(Brushes.Black, path);
        }
    }
}
