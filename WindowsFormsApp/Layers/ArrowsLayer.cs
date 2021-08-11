using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class ArrowsLayer : Layer
    {
        public ArrowsLayer(string name, Color fillColor) : base(name)
        {
            FillColor = fillColor;
        }

        public Color FillColor { get; }

        protected override void Draw(Graphics g)
        {
            var context = Controller.Context;
            var size = 0.125;
            var gap = 0.35;

            var path = new GraphicsPath();

            var stash = new Dictionary<double, SizeF[]>();

            foreach (var vertex in context.Graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {

                    var origin = new PointF(vertex.X, vertex.Y);

                    var angle1 = Math.Atan2(next.Y - origin.Y, next.X - origin.X);

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

            using (var brush = new SolidBrush(FillColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
