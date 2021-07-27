using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowsFormsApp
{
    public abstract class Layer : IDisposable
    {
        public Color BackColor { get; set; } = Color.Transparent;
        public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.HighQuality;

        public GraphicsController Controller { protected get; set; }

        public Image Bitmap { get; protected set; }

        public virtual void Update()
        {
            if (Bitmap == null)
            {
                Bitmap = new Bitmap(Controller.BitmapSize.Width, Controller.BitmapSize.Height);
            }

            using (var g = Graphics.FromImage(Bitmap))
            {
                g.SmoothingMode = SmoothingMode;
                g.Clear(BackColor);

                Draw(g);
            }
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
        }

        protected abstract void Draw(Graphics g);
    }
}
