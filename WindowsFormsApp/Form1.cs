using System;
using System.Collections.Generic;
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
        private static IDictionary<Keys, Action<Context>> keyHandlers;

        public double pageUnitRatio;

        private LayerController controller;

        public Form1()
        {
            InitializeComponent();
            InitializeKeyHandlers();
            InitPageUnitRatio();
            Randomize();
            InitContext();
            InitLayers();
            Recalc();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!keyHandlers.ContainsKey(e.KeyData))
            {
                return;
            }

            keyHandlers[e.KeyData](context);

            e.SuppressKeyPress = true;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Randomize();
            Recalc();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawBitmap(e.ClipRectangle, e.Graphics);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            controller.UpdateLayers();
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

        private void InitializeKeyHandlers()
        {
            keyHandlers = new Dictionary<Keys, Action<Context>>
            {
                {
                    Keys.OemMinus, ctx =>
                    {
                        ctx.PageScale *= 0.8f;
                        ReDraw();
                    }
                },
                {
                    Keys.Oemplus, ctx =>
                    {
                        ctx.PageScale *= 1.25f;
                        ReDraw();
                    }
                },
                {
                    Keys.Up, ctx =>
                    {
                        ctx.Top++;
                        ReDraw();
                    }
                },
                {
                    Keys.Right, ctx =>
                    {
                        ctx.Left--;
                        ReDraw();
                    }
                },
                {
                    Keys.Down, ctx =>
                    {
                        ctx.Top--;
                        ReDraw();
                    }
                },
                {
                    Keys.Left, ctx =>
                    {
                        ctx.Left++;
                        ReDraw();
                    }
                },
            };
        }

        private void InitPageUnitRatio()
        {
            pageUnitRatio = GetPageUnitRatio(context.PageUnit, DeviceDpi);
        }

        private void Randomize()
        {
            context.Graph = GraphHelper.CreateGraph();
            context.Target = GraphHelper.GetRandomVertex(context.Graph);
        }

        private void InitContext()
        {
            context.ClientRectangle = () => ClientRectangle;
            context.BackColor = () => BackColor;
            context.Font = () => Font;
        }

        private void InitLayers()
        {
            controller = new LayerController(context);
            controller.AddLayer(new ShortPathLayer("shortPath", Color.Red, 60));
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

            ReDraw();
        }

        private void ReDraw()
        {
            controller.UpdateLayers();

            DrawBitmap();
        }

        private void DrawBitmap()
        {
            using (var g = CreateGraphics())
            {
                DrawBitmap(ClientRectangle, g);
            }
        }

        private void DrawBitmap(Rectangle clip, Graphics g)
        {
            g.Clip = new Region(clip);
            g.DrawImage(controller.Bitmap, 0, 0);
        }

        private Vertex FindVertex(int x, int y)
        {
            var x1 = Convert.ToInt32(x * pageUnitRatio / context.PageScale - context.Left);
            var y1 = Convert.ToInt32(y * pageUnitRatio / context.PageScale - context.Top);

            return context.Graph[x1, y1];
        }

        private static double GetPageUnitRatio(GraphicsUnit unit, int deviceDpi)
        {
            switch (unit)
            {
                case GraphicsUnit.Inch: return deviceDpi;
                case GraphicsUnit.Millimeter: return 25.4 / deviceDpi;
                case GraphicsUnit.Point: return 72.0 / deviceDpi;
                case GraphicsUnit.Document: return 300.0 / deviceDpi;
                default: return 1.0;
            }
        }
    }
}
