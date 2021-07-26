using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ShortestPath;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private const int SRCCOPY = 0xCC0020;
        private const double zoom = 60.0;

        private static Random random = new Random();
        private static ShortestPathAlgo algo = new DijkstraAlgo();

        private Graph graph;
        private Vertex target;
        private Vertex source;
        private GraphicsPath connectorsPath;
        private GraphicsPath arrowsPath;
        private GraphicsPath textLabelsPath;
        private GraphicsPath whiteCirclesPath;
        private GraphicsPath connectorDistancesPath;

        public Form1()
        {
            InitializeComponent();
            Randomize();
            Recalc();

            // algo.ShortPathFound += Algo_ShortPathFound;
        }

        private void Randomize()
        {
            graph = CreateGraph();
            target = graph.Skip(random.Next(graph.Count)).First();
        }

        private void Recalc()
        {
            algo.Process(graph, target);

            RecalcConnectors();
            RecalcWhiteCircles();
            RecalcTextLabels();
            RecalcArrows();
            RecalcConnectorDistances();

            Redraw();
        }

        private void Redraw()
        {
            using (var g = Graphics.FromHwnd(Handle))
            {
                RedrawBackground(g);
            }
        }

        private void RedrawBackground(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.Clear(BackColor);

            DrawConnectors(g);
            DrawShortestPath(g);
            DrawWhiteCircles(g);
            DrawYellowCircle(g);
            DrawTextLabels(g);
            DrawArrows(g);
            DrawConnectorDistances(g);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Randomize();
            Recalc();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clip = new Region(e.ClipRectangle);
            RedrawBackground(g);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            target = FindVertex(e.X, e.Y);

            if (target == null)
            {
                return;
            }

            Recalc();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
            {
                return;
            }

            var newSource = FindVertex(e.X, e.Y);

            if (newSource == null || newSource == source)
            {
                return;
            }

            source = newSource;

            Redraw();
        }

        private Vertex FindVertex(int x, int y)
        {
            var x1 = Convert.ToInt32(x / zoom - 1);
            var y1 = Convert.ToInt32(y / zoom - 1);

            return graph[x1, y1];
        }

        private void DrawShortestPath(Graphics g)
        {
            if (source == null)
            {
                return;
            }

            var queue = new Queue<Vertex>();

            queue.Enqueue(source);

            using (var pen = new Pen(Color.Red))
            {
                pen.Width = 4;

                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();

                    if (current.Next != null)
                    {
                        foreach (var next in current.Next)
                        {
                            var x1 = zoom + current.X * zoom;
                            var y1 = zoom + current.Y * zoom;
                            var x2 = zoom + next.X * zoom;
                            var y2 = zoom + next.Y * zoom;

                            g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);

                            if (!queue.Contains(next))
                            {
                                queue.Enqueue(next);
                            }
                        }
                    }
                }
            }
        }

        private void RecalcArrows()
        {
            const double size = zoom / 8.0;
            const double gap = zoom / 3.0;

            arrowsPath = new GraphicsPath();

            var stash = new Dictionary<double, SizeF[]>();

            foreach (var vertex in graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = zoom + vertex.X * zoom;
                    var y1 = zoom + vertex.Y * zoom;
                    var x2 = zoom + next.X * zoom;
                    var y2 = zoom + next.Y * zoom;

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

                    arrowsPath.StartFigure();
                    arrowsPath.AddPolygon(arrow);
                    arrowsPath.CloseFigure();
                }
            }
        }

        private void DrawArrows(Graphics g)
        {
            g.FillPath(Brushes.Black, arrowsPath);
        }

        private void RecalcConnectorDistances()
        {
            const float size = 1.4f;
            const double gap = zoom / 2.4;

            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near,
            };

            connectorDistancesPath = new GraphicsPath();

            foreach (var vertex in graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = zoom + vertex.X * zoom;
                    var y1 = zoom + vertex.Y * zoom;
                    var x2 = zoom + next.X * zoom;
                    var y2 = zoom + next.Y * zoom;

                    var angle = Math.Atan2(y2 - y1, x2 - x1);

                    var length = vertex.DistanceTo(next);
                    var x = x1 + gap * Math.Cos(angle);
                    var y = y1 + gap * Math.Sin(angle);
                    var origin = new PointF((float)x, (float)y);

                    var matrix = new Matrix();
                    var degrees = angle * 180.0 / Math.PI;
                    matrix.RotateAt((float)degrees, origin);

                    var path = new GraphicsPath();
                    path.AddString(string.Format("{0:0.##}", length), Font.FontFamily, (int)Font.Style, Font.SizeInPoints * size, origin, format);
                    path.Transform(matrix);

                    connectorDistancesPath.AddPath(path, false);
                }
            }
        }

        private void DrawConnectorDistances(Graphics g)
        {
            g.FillPath(Brushes.Black, connectorDistancesPath);
        }

        private void RecalcTextLabels()
        {
            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            textLabelsPath = new GraphicsPath();

            foreach (var vertex in graph.Where(v => v.DistanceToTarget < double.MaxValue))
            {
                var x1 = zoom + vertex.X * zoom;
                var y1 = zoom + vertex.Y * zoom;
                var origin = new PointF((float)x1, (float)y1);
                var distance = string.Format("{0:0.#}", vertex.DistanceToTarget);
                textLabelsPath.AddString(distance, Font.FontFamily, (int)Font.Style, Font.SizeInPoints * 1.5f, origin, format);
            }
        }

        private void DrawTextLabels(Graphics g)
        {
            g.FillPath(Brushes.Black, textLabelsPath);
        }

        private void DrawYellowCircle(Graphics g)
        {
            const double size = zoom / 2.0;

            var x1 = zoom + target.X * zoom - size / 2.0;
            var y1 = zoom + target.Y * zoom - size / 2.0;

            g.FillEllipse(Brushes.Yellow, (float)x1, (float)y1, (float)size, (float)size);
            g.DrawEllipse(Pens.Black, (float)x1, (float)y1, (float)size, (float)size);
        }

        private void RecalcWhiteCircles()
        {
            const double size = zoom / 2.0;

            whiteCirclesPath = new GraphicsPath();

            foreach (var vertex in graph)
            {
                var x1 = zoom + vertex.X * zoom - size / 2.0;
                var y1 = zoom + vertex.Y * zoom - size / 2.0;

                whiteCirclesPath.StartFigure();
                whiteCirclesPath.AddEllipse((float)x1, (float)y1, (float)size, (float)size);
                whiteCirclesPath.CloseFigure();
            }
        }

        private void DrawWhiteCircles(Graphics g)
        {
            g.FillPath(Brushes.White, whiteCirclesPath);
            g.DrawPath(Pens.Black, whiteCirclesPath);
        }

        private void RecalcConnectors()
        {
            connectorsPath = new GraphicsPath();

            foreach (var vertex in graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = zoom + vertex.X * zoom;
                    var y1 = zoom + vertex.Y * zoom;
                    var x2 = zoom + next.X * zoom;
                    var y2 = zoom + next.Y * zoom;

                    connectorsPath.StartFigure();
                    connectorsPath.AddLine((float)x1, (float)y1, (float)x2, (float)y2);
                    connectorsPath.CloseFigure();
                }
            }
        }

        private void DrawConnectors(Graphics g)
        {
            g.DrawPath(Pens.Black, connectorsPath);
        }

        private static Graph CreateGraph()
        {
            const int sizeX = 31;
            const int sizeY = 16;

            var graph = new Graph();

            var offsets = Enumerable.Empty<PointF>()
                .Concat(new[] { -1, 0, 1 }.SelectMany(y => new[] { -1, 0, 1 }.Select(x => new PointF(x, y))))
                .Concat(new[] { -2, 2 }.SelectMany(y => new[] { -1, 1 }.Select(x => new PointF(x, y))))
                .Concat(new[] { -1, 1 }.SelectMany(y => new[] { -2, 2 }.Select(x => new PointF(x, y))))
                .Where(p => p.X != 0 || p.Y != 0)
                .ToArray();

            // add all vertices
            graph.AddRange(Enumerable.Range(0, sizeY)
                .SelectMany(y => Enumerable.Range(0, sizeX).Select(x => new Vertex(x, y))));

            /*
            // remove some to create obstacles
            var obstacleOffsets = offsets
                .Concat(new[] { -2, 2 }.Select(x => new PointF(x, 0)))
                .Concat(new[] { -2, 2 }.Select(y => new PointF(0, y)))
                .ToArray();

            for (var i = 0; i < 10; i++)
            {
                var index = random.Next(graph.Count);
                var vertex = graph.Skip(index).First();
                var toRemove = obstacleOffsets
                    .Select(offset => graph[vertex.X + offset.X, vertex.Y + offset.Y])
                    .Where(v => v != null)
                    .Concat(new[] { vertex });

                foreach (var item in toRemove)
                {
                    graph.Remove(item);
                }
            }
            */

            // initialize neighbors
            foreach (var vertex in graph)
            {
                vertex.Neighbors = offsets
                    .Select(offset => graph[vertex.X + offset.X, vertex.Y + offset.Y])
                    .Where(v => v != null)
                    .ToList();
            }

            return graph;
        }

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, int rop);

        private Image CopyClientWindowToImage()
        {
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height);

            using (var bmpGraphics = Graphics.FromImage(bmp))
            {
                var bmpDC = bmpGraphics.GetHdc();

                using (var g = Graphics.FromHwnd(Handle))
                {
                    var formDC = g.GetHdc();
                    BitBlt(bmpDC, 0, 0, ClientSize.Width, ClientSize.Height, formDC, 0, 0, SRCCOPY);
                    g.ReleaseHdc(formDC);
                }

                bmpGraphics.ReleaseHdc(bmpDC);
            }

            return bmp;
        }
    }
}
