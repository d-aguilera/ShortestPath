using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading;

namespace WindowsFormsApp
{
    public class LayerController : IDisposable, IComponent
    {
        private readonly Dictionary<string, Layer> layers = new Dictionary<string, Layer>();

        public Context Context { get; }

        public IReadOnlyDictionary<string, Layer> Layers => new ReadOnlyDictionary<string, Layer>(layers);

        public Image Bitmap { get; private set; }

        public LayerController(Context context)
        {
            Context = context;
        }

        public void AddLayer(Layer layer)
        {
            layers.Add(layer.Name, layer);
            layer.Controller = this;
        }

        public void UpdateLayers()
        {
            var handles = new List<WaitHandle>();

            foreach (var layer in layers.Values)
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
            var size = Context.ClientRectangle().Size;

            if (Bitmap != null && Bitmap.Size != size)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }

            if (Bitmap == null)
            {
                Bitmap = new Bitmap(size.Width, size.Height);
            }

            using (var g = Graphics.FromImage(Bitmap))
            {
                g.Clip = new Region(Context.ClientRectangle());
                g.Clear(Context.BackColor());
                foreach (var layer in layers.Values)
                {
                    g.DrawImage(layer.Bitmap, 0, 0);
                }
            }
        }

        public void Dispose()
        {
            foreach (var layer in layers.Values)
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
