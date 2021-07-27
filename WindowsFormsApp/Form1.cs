using System;
using System.ComponentModel;
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
        private static Context context = new Context();

        private LayerController controller;

        public Form1()
        {
            InitializeComponent();
            Randomize();
            InitContext();
            InitLayers();
            Recalc();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Randomize();
            Recalc();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawBitmap(new Region(e.ClipRectangle), e.Graphics);
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

            context.Target = newTarget;

            Recalc();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.None)
            {
                return;
            }

            var newSource = FindVertex(e.X, e.Y);

            if (newSource == null || newSource == context.Source)
            {
                return;
            }

            context.Source = newSource;

            controller.Layers["shortPath"].Update();
            controller.UpdateBitmap();

            DrawBitmap();
        }

        private void Randomize()
        {
            context.Graph = CreateGraph();
            context.Target = context.Graph.Skip(random.Next(context.Graph.Count)).First();
        }

        private void InitContext()
        {
            context.BackColor = () => BackColor;
            context.Font = () => Font;
        }

        private void InitLayers()
        {
            controller = new LayerController(context);
            controller.AddLayer("shortPath", new Layer0());
            // controller.AddLayer("segments", new Layer1());
            controller.AddLayer("vertices", new Layer2());
            controller.AddLayer("target", new Layer3());
            // controller.AddLayer("vertexDistances", new Layer4());
            // controller.AddLayer("arrows", new Layer5());
            // controller.AddLayer("segmentLengths", new Layer6());

            components = new Container();
            components.Add(controller);
        }

        private void Recalc()
        {
            algo.Process(context.Graph, context.Target);

            context.Source = null;

            controller.UpdateLayers();

            DrawBitmap();
        }

        private void DrawBitmap()
        {
            var clip = new Region(ClientRectangle);

            using (var g = Graphics.FromHwnd(Handle))
            {
                DrawBitmap(clip, g);
            }
        }

        private void DrawBitmap(Region clip, Graphics g)
        {
            g.Clip = clip;
            g.DrawImage(controller.Bitmap, 0, 0);
        }

        private Vertex FindVertex(int x, int y)
        {
            var x1 = Convert.ToInt32(x / context.Zoom - 1);
            var y1 = Convert.ToInt32(y / context.Zoom - 1);

            return context.Graph[x1, y1];
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
    }
}
