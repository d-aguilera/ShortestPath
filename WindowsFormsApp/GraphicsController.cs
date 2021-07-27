using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace WindowsFormsApp
{
    public class GraphicsController : IDisposable
    {
        public Context Context { get; }

        public Image Bitmap { get; private set; }
        public Size BitmapSize { get; private set; }
        public List<Layer> Layers { get; private set; }

        public GraphicsController(Context context)
        {
            Context = context;
            Layers = new List<Layer>();
        }

        public void AddLayer(Layer layer)
        {
            layer.Controller = this;
            Layers.Add(layer);
        }

        public void UpdateLayers()
        {
            if (BitmapSize == Size.Empty)
            {
                var width = Context.Graph.Max(v => v.X) * Context.Zoom + 2.0 * Context.Zoom;
                var height = Context.Graph.Max(v => v.Y) * Context.Zoom + 2.0 * Context.Zoom;
                BitmapSize = new Size((int)width, (int)height);
            }

            var countdown = new CountdownEvent(Layers.Count);

            foreach (var layer in Layers)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    layer.Update();
                    countdown.Signal();
                });
            }

            countdown.Wait();

            UpdateBitmap();
        }

        public void UpdateBitmap()
        {
            if (Bitmap == null)
            {
                Bitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height);
            }

            using (var g = Graphics.FromImage(Bitmap))
            {
                foreach (var layer in Layers)
                {
                    g.DrawImage(layer.Bitmap, 0, 0);
                }
            }
        }

        public void Dispose()
        {
            foreach (var layer in Layers)
            {
                layer?.Dispose();
            }

            Bitmap?.Dispose();
            Bitmap = null;
        }
    }
}
