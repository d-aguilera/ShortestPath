using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class Layer2 : Layer
    {
        public Layer2(Context context) : base(context)
        {
            Steps = new Action<Graphics>[]
            {
                DrawWhiteCircles,
                DrawYellowCircle,
                DrawTextLabels,
                DrawArrows,
                DrawConnectorDistances,
            };
        }

        private void DrawWhiteCircles(Graphics g)
        {
            var size = Context.Zoom / 2.0;

            var path = new GraphicsPath();

            foreach (var vertex in Context.Graph)
            {
                var x1 = Context.Zoom + vertex.X * Context.Zoom - size / 2.0;
                var y1 = Context.Zoom + vertex.Y * Context.Zoom - size / 2.0;

                path.StartFigure();
                path.AddEllipse((float)x1, (float)y1, (float)size, (float)size);
                path.CloseFigure();
            }

            g.FillPath(Brushes.White, path);
            g.DrawPath(Pens.Black, path);
        }

        private void DrawYellowCircle(Graphics g)
        {
            var size = Context.Zoom / 2.0;

            var x1 = Context.Zoom + Context.Target.X * Context.Zoom - size / 2.0;
            var y1 = Context.Zoom + Context.Target.Y * Context.Zoom - size / 2.0;

            g.FillEllipse(Brushes.Yellow, (float)x1, (float)y1, (float)size, (float)size);
            g.DrawEllipse(Pens.Black, (float)x1, (float)y1, (float)size, (float)size);
        }

        private void DrawTextLabels(Graphics g)
        {
            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            var path = new GraphicsPath();

            foreach (var vertex in Context.Graph.Where(v => v.DistanceToTarget < double.MaxValue))
            {
                var x1 = Context.Zoom + vertex.X * Context.Zoom;
                var y1 = Context.Zoom + vertex.Y * Context.Zoom;
                var origin = new PointF((float)x1, (float)y1);
                var distance = string.Format("{0:0.#}", vertex.DistanceToTarget);
                path.AddString(distance, Context.Font.FontFamily, (int)Context.Font.Style, Context.Font.SizeInPoints * 1.5f, origin, format);
            }

            g.FillPath(Brushes.Black, path);
        }

        private void DrawArrows(Graphics g)
        {
            var size = Context.Zoom / 8.0;
            var gap = Context.Zoom / 3.0;

            var path = new GraphicsPath();

            var stash = new Dictionary<double, SizeF[]>();

            foreach (var vertex in Context.Graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = Context.Zoom + vertex.X * Context.Zoom;
                    var y1 = Context.Zoom + vertex.Y * Context.Zoom;
                    var x2 = Context.Zoom + next.X * Context.Zoom;
                    var y2 = Context.Zoom + next.Y * Context.Zoom;

                    var origin = new PointF((float)x1, (float)y1);

                    var angle1 = Math.Atan2(y2 - y1, x2 - x1);

                    if (!stash.ContainsKey(angle1))
                    {
                        var xm = gap * Math.Cos(angle1);
                        var ym = gap * Math.Sin(angle1);

                        double xa, ya;

                        xa = xm + size * Math.Cos(angle1);
                        ya = ym + size * Math.Sin(angle1);
                        var p1 = new PointF((float)xa, (float)ya);

                        var angle2 = angle1 + Math.PI * 5.0 / 6.0;
                        xa = xm + size * Math.Cos(angle2);
                        ya = ym + size * Math.Sin(angle2);
                        var p2 = new PointF((float)xa, (float)ya);

                        var angle3 = angle1 - Math.PI * 5.0 / 6.0;
                        xa = xm + size * Math.Cos(angle3);
                        ya = ym + size * Math.Sin(angle3);
                        var p3 = new PointF((float)xa, (float)ya);

                        stash.Add(angle1, new[]
                        {
                            new SizeF(p1),
                            new SizeF(p2),
                            new SizeF(p3),
                        });
                    }

                    var offsets = stash[angle1];

                    var arrow = new[]
                    {
                        PointF.Add(origin, offsets[0]),
                        PointF.Add(origin, offsets[1]),
                        PointF.Add(origin, offsets[2]),
                    };

                    path.StartFigure();
                    path.AddPolygon(arrow);
                    path.CloseFigure();
                }
            }

            g.FillPath(Brushes.Black, path);
        }

        private void DrawConnectorDistances(Graphics g)
        {
            const float size = 1.4f;

            var gap = Context.Zoom / 2.4;

            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near,
            };

            var path = new GraphicsPath();

            foreach (var vertex in Context.Graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = Context.Zoom + vertex.X * Context.Zoom;
                    var y1 = Context.Zoom + vertex.Y * Context.Zoom;
                    var x2 = Context.Zoom + next.X * Context.Zoom;
                    var y2 = Context.Zoom + next.Y * Context.Zoom;

                    var angle = Math.Atan2(y2 - y1, x2 - x1);

                    var length = vertex.DistanceTo(next);
                    var x = x1 + gap * Math.Cos(angle);
                    var y = y1 + gap * Math.Sin(angle);
                    var origin = new PointF((float)x, (float)y);

                    var matrix = new Matrix();
                    var degrees = angle * 180.0 / Math.PI;
                    matrix.RotateAt((float)degrees, origin);

                    var path2 = new GraphicsPath();
                    path2.AddString(string.Format("{0:0.##}", length), Context.Font.FontFamily, (int)Context.Font.Style, Context.Font.SizeInPoints * size, origin, format);
                    path2.Transform(matrix);

                    path.AddPath(path2, false);
                }
            }

            g.FillPath(Brushes.Black, path);
        }
    }
}
