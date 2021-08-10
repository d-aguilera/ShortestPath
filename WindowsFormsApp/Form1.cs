using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ShortestPath;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        private static ShortestPathAlgo algo = new DijkstraWithQueueAlgo();
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
            context.Graph = GraphHelper.CreateGraph();
            context.Target = GraphHelper.GetRandomVertex(context.Graph);
        }

        private void InitContext()
        {
            context.BackColor = () => BackColor;
            context.Font = () => Font;
        }

        private void InitLayers()
        {
            controller = new LayerController(context);
            controller.AddLayer(new ShortPathLayer("shortPath", Color.Red, 15.0f));
            // controller.AddLayer(new SegmentsLayer("segments", Color.Black));
            controller.AddLayer(new VerticesLayer("vertices", Color.White, Color.Black));
            controller.AddLayer(new TargetLayer("target", Color.Yellow, Color.Black));
            // controller.AddLayer(new VertexDistancesLayer("vertexDistances", Color.Black));
            // controller.AddLayer(new ArrowsLayer("arrows", Color.Black));
            // controller.AddLayer(new SegmentLengthsLayer("segmentLengths", Color.Black));

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
    }
}
