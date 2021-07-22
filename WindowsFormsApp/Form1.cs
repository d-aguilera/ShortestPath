using System;
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
            eraseImage = CopyClientWindowToImage();
            algo.Process(graph, target);
            eraseImage = null;
        }

        private void Repaint()
        {
            using (var g = Graphics.FromHwnd(Handle))
            {
                Form1_Paint(this, new PaintEventArgs(g, ClientRectangle));
            }
        }

        private Image eraseImage;

        private void Algo_ShortPathFound(object sender, ShortPathFoundEventArgs e)
        {
            using (var g = Graphics.FromHwnd(Handle))
            using (var pen = new Pen(Color.FromArgb(255, 255, 0, 0)))
            {
                pen.Width = 2.0f;

                var x1 = zoom + e.Neighbor.X * zoom;
                var y1 = zoom + e.Neighbor.Y * zoom;
                var x2 = zoom + e.Current.X * zoom;
                var y2 = zoom + e.Current.Y * zoom;

                ErasePrevious(g);

                g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
            }
        }

        private void ErasePrevious(Graphics g)
        {
            if (eraseImage != null)
            {
                g.DrawImage(eraseImage, 0, 0);
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Randomize();
            Recalc();
            Repaint();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clip = new Region(e.ClipRectangle);
            g.Clear(BackColor);

            using (var pen = new Pen(Color.FromArgb(255, 0, 0, 0)))
            {
                DrawConnectors(g, pen);
                DrawWhiteCircles(g, pen);
                DrawYellowCircle(g, pen);
                DrawTextLabels(g);
                DrawArrows(g);
            }
        }

        private void DrawArrows(Graphics g)
        {
            const double size = zoom / 8.0;
            const double gap = zoom / 3.0;

            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near,
            };

            using (var brush = new SolidBrush(Color.FromArgb(255, 0, 0, 0)))
            using (var textBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 255)))
            {
                foreach (var vertex in graph.Where(v => v.Next != null))
                {
                    foreach (var next in vertex.Next)
                    {
                        var x1 = zoom + vertex.X * zoom;
                        var y1 = zoom + vertex.Y * zoom;
                        var x2 = zoom + next.X * zoom;
                        var y2 = zoom + next.Y * zoom;

                        var angle1 = Math.Atan2(y2 - y1, x2 - x1);
                        var angle2 = angle1 + Math.PI * 5.0 / 6.0;
                        var angle3 = angle1 - Math.PI * 5.0 / 6.0;

                        var xm = x1 + gap * Math.Cos(angle1);
                        var ym = y1 + gap * Math.Sin(angle1);

                        double xa, ya;

                        xa = xm + size * Math.Cos(angle1);
                        ya = ym + size * Math.Sin(angle1);
                        var p1 = new PointF((float)xa, (float)ya);

                        xa = xm + size * Math.Cos(angle2);
                        ya = ym + size * Math.Sin(angle2);
                        var p2 = new PointF((float)xa, (float)ya);

                        xa = xm + size * Math.Cos(angle3);
                        ya = ym + size * Math.Sin(angle3);
                        var p3 = new PointF((float)xa, (float)ya);

                        g.FillPolygon(brush, new[] { p1, p2, p3 });

                        var length = vertex.DistanceTo(next);
                        var gapl = zoom / 2.5;
                        var xl = x1 + gapl * Math.Cos(angle1);
                        var yl = y1 + gapl * Math.Sin(angle1);
                        DrawRotatedTextAt(g, (float)angle1, string.Format("{0:0.##}", length), (int)xl, (int)yl, Font, textBrush, format);
                    }
                }
            }
        }

        private void DrawTextLabels(Graphics g)
        {
            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            using (var brush = new SolidBrush(Color.FromArgb(255, 0, 0, 0)))
            {
                foreach (var vertex in graph.Where(v => v.DistanceToTarget < double.MaxValue))
                {
                    var x1 = zoom + vertex.X * zoom;
                    var y1 = zoom + vertex.Y * zoom;
                    var distance = string.Format("{0:0.#}", vertex.DistanceToTarget);

                    g.DrawString(distance, Font, brush, (float)x1, (float)y1, format);
                }
            }
        }

        private void DrawYellowCircle(Graphics g, Pen pen)
        {
            const double size = zoom / 2.0;

            using (var brush = new SolidBrush(Color.FromArgb(255, 255, 255, 0)))
            {
                var x1 = zoom + target.X * zoom - size / 2.0;
                var y1 = zoom + target.Y * zoom - size / 2.0;
                g.FillEllipse(brush, (float)x1, (float)y1, (float)size, (float)size);
                g.DrawEllipse(pen, (float)x1, (float)y1, (float)size, (float)size);
            }
        }

        private void DrawWhiteCircles(Graphics g, Pen pen)
        {
            const double size = zoom / 2.0;

            using (var brush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)))
            {
                foreach (var vertex in graph)
                {
                    var x1 = zoom + vertex.X * zoom - size / 2.0;
                    var y1 = zoom + vertex.Y * zoom - size / 2.0;
                    g.FillEllipse(brush, (float)x1, (float)y1, (float)size, (float)size);
                    g.DrawEllipse(pen, (float)x1, (float)y1, (float)size, (float)size);
                }
            }
        }

        private void DrawConnectors(Graphics g, Pen pen)
        {
            foreach (var vertex in graph.Where(v => v.Next != null))
            {
                foreach (var next in vertex.Next)
                {
                    var x1 = zoom + vertex.X * zoom;
                    var y1 = zoom + vertex.Y * zoom;
                    var x2 = zoom + next.X * zoom;
                    var y2 = zoom + next.Y * zoom;

                    g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
                }
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var x = Convert.ToInt32(e.X / zoom - 1);
            var y = Convert.ToInt32(e.Y / zoom - 1);
            target = graph[x, y];

            if (target == null)
            {
                return;
            }

            Recalc();
            Repaint();
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

        private void DrawRotatedTextAt(Graphics g, float angle, string text, int x, int y, Font font, Brush brush, StringFormat format)
        {
            var degrees = Convert.ToSingle(angle * 180.0 / Math.PI);
            var state = g.Save();
            g.ResetTransform();
            g.RotateTransform(degrees);
            g.TranslateTransform(x, y, MatrixOrder.Append);
            g.DrawString(text, font, brush, 0, 0, format);
            g.Restore(state);
        }

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, int rop);

        private Image CopyClientWindowToImage()
        {
            var bmp = new Bitmap(ClientSize.Width, ClientSize.Height);

            using (var bmpGraphics = Graphics.FromImage(bmp))
            {
                var bmpDC = bmpGraphics.GetHdc();

                using (var formGraphics = Graphics.FromHwnd(Handle))
                {
                    var formDC = formGraphics.GetHdc();
                    BitBlt(bmpDC, 0, 0, ClientSize.Width, ClientSize.Height, formDC, 0, 0, SRCCOPY);
                    formGraphics.ReleaseHdc(formDC);
                }

                bmpGraphics.ReleaseHdc(bmpDC);
            }

            return bmp;
        }
    }
}
