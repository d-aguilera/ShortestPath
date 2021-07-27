using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ShortestPath;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private static Random random = new Random();
        private static ShortestPathAlgo algo = new DijkstraAlgo();
        private static Context Context = new Context();

        private Layer[] layers;
        private Image bitmap;

        public Form1()
        {
            InitializeComponent();

            Context.Font = Font;

            layers = new Layer[]
            {
                new Layer0(Context)
                {
                    BackColor = Color.Yellow,
                },
                new Layer1(Context)
                {
                    BackColor = Color.Blue,
                },
                new Layer2(Context)
                {
                    BackColor = Color.Transparent,
                },
            };

            Randomize();
            Recalc();

            // algo.ShortPathFound += Algo_ShortPathFound;
        }

        private void Randomize()
        {
            Context.Graph = CreateGraph();
            Context.Target = Context.Graph.Skip(random.Next(Context.Graph.Count)).First();
        }

        private void Recalc()
        {
            algo.Process(Context.Graph, Context.Target);

            Context.Source = null;

            var clip = new Region(ClientRectangle);

            UpdateLayers(clip);
            DrawBitmap(clip);
        }

        private void UpdateLayers(Region clip)
        {
            foreach (var layer in layers)
            {
                layer.Update(ClientSize, clip);
            }

            UpdateBitmap(clip);
        }

        private void UpdateBitmap(Region clip)
        {
            bitmap?.Dispose();
            bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clip = clip;
                foreach (var layer in layers)
                {
                    g.DrawImage(layer.Bitmap, 0, 0);
                }
            }
        }

        private void DrawBitmap(Region clip)
        {
            using (var g = Graphics.FromHwnd(Handle))
            {
                DrawBitmap(clip, g);
            }
        }

        private void DrawBitmap(Region clip, Graphics g)
        {
            g.Clip = clip;
            g.DrawImage(bitmap, 0, 0);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Randomize();
            Recalc();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var clip = new Region(e.ClipRectangle);

            UpdateLayers(clip);
            DrawBitmap(clip, e.Graphics);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var newTarget = FindVertex(e.X, e.Y);

            if (newTarget == null)
            {
                return;
            }

            Context.Target = newTarget;

            Recalc();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
            {
                return;
            }

            var newSource = FindVertex(e.X, e.Y);

            if (newSource == null || newSource == Context.Source)
            {
                return;
            }

            Context.Source = newSource;

            var clip = new Region(ClientRectangle);

            layers[1].Update(ClientSize, clip);

            UpdateBitmap(clip);
            DrawBitmap(clip);
        }

        private Vertex FindVertex(int x, int y)
        {
            var x1 = Convert.ToInt32(x / Context.Zoom - 1);
            var y1 = Convert.ToInt32(y / Context.Zoom - 1);

            return Context.Graph[x1, y1];
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
    }
}
