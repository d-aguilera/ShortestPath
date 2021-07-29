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

                Bitmap = new Bitmap(value.BitmapSize.Width, value.BitmapSize.Height);

                drawStrategy = new ForegroundThreadDrawStrategy(this, Draw);
            }
        }

        public Image Bitmap { get; private set; }
        public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.HighQuality;

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

        protected abstract void Draw(Graphics g);
    }
}
