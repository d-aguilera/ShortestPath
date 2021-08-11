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

            var innerSize = 0.5f;
            var x1 = (float)(context.Target.X - innerSize / 2.0);
            var y1 = (float)(context.Target.Y - innerSize / 2.0);

            var outerSize = 0.75f;
            var x2 = (float)(context.Target.X - outerSize / 2.0);
            var y2 = (float)(context.Target.Y - outerSize / 2.0);

            using (var brush = new SolidBrush(FillColor))
            using (var pen = new Pen(BorderColor, 0.05f))
            {
                g.FillEllipse(brush, x1, y1, innerSize, innerSize);
                g.DrawEllipse(pen, x1, y1, innerSize, innerSize);
                g.DrawEllipse(pen, x2, y2, outerSize, outerSize);
            }
        }
    }
}
