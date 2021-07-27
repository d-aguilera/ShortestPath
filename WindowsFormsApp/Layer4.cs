using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class Layer4 : Layer
    {
        public Layer4(LayerController controller) : base(controller)
        {
        }

        protected override void Draw(Graphics g)
        {
            DrawTextLabels(g);
        }

        private void DrawTextLabels(Graphics g)
        {
            var context = Controller.Context;
            var path = new GraphicsPath();

            var format = new StringFormat(StringFormat.GenericDefault)
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            foreach (var vertex in context.Graph.Where(v => v.DistanceToTarget < double.MaxValue))
            {
                var x1 = context.Zoom + vertex.X * context.Zoom;
                var y1 = context.Zoom + vertex.Y * context.Zoom;
                var origin = new PointF((float)x1, (float)y1);
                var distance = string.Format("{0:0.#}", vertex.DistanceToTarget);
                path.AddString(distance, context.Font().FontFamily, (int)context.Font().Style, context.Font().SizeInPoints * 1.5f, origin, format);
            }

            g.FillPath(Brushes.Black, path);
        }
    }
}
