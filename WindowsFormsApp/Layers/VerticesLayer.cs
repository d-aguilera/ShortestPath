using System.Drawing;
using System.Drawing.Drawing2D;

namespace WindowsFormsApp
{
    public class VerticesLayer : Layer
    {
        public VerticesLayer(string name, Color fillColor, Color borderColor) : base(name)
        {
            FillColor = fillColor;
            BorderColor = borderColor;
        }

        public Color FillColor { get; }
        public Color BorderColor { get; }

        protected override void Draw(Graphics g)
        {
            var context = Controller.Context;
            var size = 0.5f;
            var path = new GraphicsPath();

            foreach (var vertex in context.Graph)
            {
                var x1 = (float)(vertex.X - size / 2.0);
                var y1 = (float)(vertex.Y - size / 2.0);

                path.StartFigure();
                path.AddEllipse(x1, y1, size, size);
                path.CloseFigure();
            }

            using (var brush = new SolidBrush(FillColor))
            using (var pen = new Pen(BorderColor, 0.03f))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }
        }
    }
}
