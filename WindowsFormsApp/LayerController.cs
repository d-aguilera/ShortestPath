using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace WindowsFormsApp
{
    public class LayerController : IDisposable, IComponent
    {
        public Context Context { get; }

        public Image Bitmap { get; private set; }
        public Size BitmapSize { get; private set; }
        public List<Layer> Layers { get; private set; }

        public LayerController(Context context)
        {
            Context = context;

            var width = Context.Graph.Max(v => v.X) * Context.Zoom + 2.0 * Context.Zoom;
            var height = Context.Graph.Max(v => v.Y) * Context.Zoom + 2.0 * Context.Zoom;
            BitmapSize = new Size((int)width, (int)height);

            Layers = new List<Layer>();
        }

        public void AddLayer(Layer layer)
        {
            Layers.Add(layer);
        }

        public void UpdateLayers()
        {
            var handles = new List<WaitHandle>();

            foreach (var layer in Layers)
            {
                handles.Add(layer.UpdateAsync());
            }

            foreach (var handle in handles)
            {
                handle.WaitOne();
            }

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

        #region IComponent

        ISite IComponent.Site { get; set; }

        event EventHandler IComponent.Disposed
        {
            add
            {
            }

            remove
            {
            }
        }

        #endregion
    }
}
