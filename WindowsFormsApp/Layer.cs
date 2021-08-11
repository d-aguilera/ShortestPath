using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace WindowsFormsApp
{
    public abstract class Layer : IDrawStrategy, IDisposable
    {
        private LayerController controller;
        private IDrawStrategy drawStrategy;

        public LayerController Controller
        {
            protected get => controller;
            set
            {
                controller = value;

                drawStrategy = new ForegroundThreadDrawStrategy(this, DrawLayer);
            }
        }

        public string Name { get; }
        public Context Context => Controller?.Context;
        public Image Bitmap { get; set; }
        public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.HighQuality;

        public Layer(string name)
        {
            Name = name;
        }

        public void Update()
        {
            drawStrategy.Update();
        }

        public WaitHandle UpdateAsync()
        {
            return drawStrategy.UpdateAsync();
        }

        public void Dispose()
        {
            if (drawStrategy != null && drawStrategy is IDisposable disposableDrawStrategy)
            {
                disposableDrawStrategy.Dispose();
            }

            Bitmap.Dispose();
        }

        private void DrawLayer(Graphics g)
        {
            Draw(g);
        }

        protected abstract void Draw(Graphics g);
    }
}
