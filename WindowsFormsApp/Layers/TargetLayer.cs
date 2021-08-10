using System.Drawing;

namespace WindowsFormsApp
{
    public class TargetLayer : Layer
    {
        public TargetLayer(string name, Color fillColor, Color borderColor) : base(name)
        {
            FillColor = fillColor;
            BorderColor = borderColor;
        }

        public Color FillColor { get; }
        public Color BorderColor { get; }

        protected override void Draw(Graphics g)
        {
            var context = Controller.Context;
            var size = context.Zoom / 2.0;
            var x1 = context.Zoom + context.Target.X * context.Zoom - size / 2.0;
            var y1 = context.Zoom + context.Target.Y * context.Zoom - size / 2.0;

            using (var brush = new SolidBrush(FillColor))
            using (var pen = new Pen(BorderColor))
            {
                g.FillEllipse(brush, (float)x1, (float)y1, (float)size, (float)size);
                g.DrawEllipse(pen, (float)x1, (float)y1, (float)size, (float)size);
            }
        }
    }
}
