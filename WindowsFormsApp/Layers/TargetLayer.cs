using System.Drawing;

namespace WindowsFormsApp
{
    public class TargetLayer : Layer
    {
        public TargetLayer(string name) : base(name)
        {
        }

        protected override void Draw(Graphics g)
        {
            var context = Controller.Context;
            var size = context.Zoom / 2.0;
            var x1 = context.Zoom + context.Target.X * context.Zoom - size / 2.0;
            var y1 = context.Zoom + context.Target.Y * context.Zoom - size / 2.0;

            g.FillEllipse(Brushes.Yellow, (float)x1, (float)y1, (float)size, (float)size);
            g.DrawEllipse(Pens.Black, (float)x1, (float)y1, (float)size, (float)size);
        }
    }
}
