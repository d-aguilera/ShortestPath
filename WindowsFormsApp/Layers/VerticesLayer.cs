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
            var size = context.Zoom / 2.0;
            var path = new GraphicsPath();

            foreach (var vertex in context.Graph)
            {
                var x1 = context.Zoom + vertex.X * context.Zoom - size / 2.0;
                var y1 = context.Zoom + vertex.Y * context.Zoom - size / 2.0;

                path.StartFigure();
                path.AddEllipse((float)x1, (float)y1, (float)size, (float)size);
                path.CloseFigure();
            }

            using (var brush = new SolidBrush(FillColor))
            using (var pen = new Pen(BorderColor))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }
        }
    }
}
