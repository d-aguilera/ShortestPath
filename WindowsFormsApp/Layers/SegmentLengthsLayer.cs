using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class SegmentLengthsLayer : Layer
    {
        private const double RadToDeg = 180.0 / Math.PI;

        public SegmentLengthsLayer(string name, Color textColor) : base(name)
        {
            TextColor = textColor;
        }

        public Color TextColor { get; }

        protected override void Draw(Graphics g)
        {
            var context = Controller.Context;
            var gap = 0.5;
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

                    var angle = Math.Atan2(next.Y - vertex.Y, next.X - vertex.X);

                    var length = vertex.DistanceTo(next);
                    var x = vertex.X + gap * Math.Cos(angle);
                    var y = vertex.Y + gap * Math.Sin(angle);
                    var origin = new PointF((float)x, (float)y);

                    var matrix = new Matrix();
                    var degrees = angle * RadToDeg;
                    matrix.RotateAt((float)degrees, origin);

                    var path2 = new GraphicsPath();
                    path2.AddString(string.Format("{0:0.##}", length), context.Font().FontFamily, (int)context.Font().Style, (float)(context.Font().SizeInPoints / 45.0), origin, format);
                    path2.Transform(matrix);

                    path.AddPath(path2, false);
                }
            }

            using (var brush = new SolidBrush(TextColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
