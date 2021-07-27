using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowsFormsApp
{
    public class Layer
    {
        public Context Context { get; }
        public Image Bitmap { get; set; }
        public Color BackColor { get; set; } = Color.Transparent;
        public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.HighQuality;

        protected IEnumerable<Action<Graphics>> Steps { get; set; }

        public Layer(Context context)
        {
            Context = context;
        }

        public void Update(Size clientSize, Region clip)
        {
            Bitmap?.Dispose();
            Bitmap = new Bitmap(clientSize.Width, clientSize.Height);

            using (var g = Graphics.FromImage(Bitmap))
            {
                g.Clip = clip;
                g.SmoothingMode = SmoothingMode;
                g.Clear(BackColor);

                foreach (var step in Steps)
                {
                    step(g);
                }
            }
        }
    }
}
