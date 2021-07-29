using System;
using System.Drawing;
using System.Threading;

namespace WindowsFormsApp
{
    public abstract class DrawStrategy : IDrawStrategy, IDisposable
    {
        public DrawStrategy(Layer layer, Action<Graphics> drawAction)
        {
            Layer = layer;
            Draw = drawAction;
            UpdateEnd = new AutoResetEvent(false);
        }

        public virtual void Update()
        {
            UpdateAsync().WaitOne();
        }

        public virtual WaitHandle UpdateAsync()
        {
            BeginUpdate();
            return UpdateEnd;
        }

        protected Layer Layer { get; }
        protected Action<Graphics> Draw { get; }
        protected AutoResetEvent UpdateEnd { get; }

        protected abstract void BeginUpdate();

        protected virtual void UpdateInternal()
        {
            using (var g = Graphics.FromImage(Layer.Bitmap))
            {
                g.SmoothingMode = Layer.SmoothingMode;
                g.Clear(Color.Transparent);

                Draw(g);
            }

            UpdateEnd.Set();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            UpdateEnd.Dispose();
        }
    }
}
