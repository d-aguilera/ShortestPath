using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace WindowsFormsApp
{
    public class VertexDistancesLayer : Layer
    {
        public VertexDistancesLayer(string name, Color textColor) : base(name)
        {
            TextColor = textColor;
        }

        public Color TextColor { get; }

        protected override void Draw(Graphics g)
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
                var origin = new PointF(vertex.X, vertex.Y);
                var distance = string.Format("{0:0.#}", vertex.DistanceToTarget);
                path.AddString(distance, context.Font().FontFamily, (int)context.Font().Style, (float)(context.Font().SizeInPoints / 40.0), origin, format);
            }

            using (var brush = new SolidBrush(TextColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
