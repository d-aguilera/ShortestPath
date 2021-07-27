using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;

namespace WindowsFormsApp
{
    public abstract class Layer : IDisposable
    {
        private readonly AutoResetEvent updateStart;
        private readonly AutoResetEvent updateEnd;
        private readonly CancellationTokenSource updateLoop;
        private readonly Thread updateThread;

        protected GraphicsController Controller { get; set; }

        public Image Bitmap { get; }
        public Color BackColor { get; set; } = Color.Transparent;
        public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.HighQuality;

        public Layer(GraphicsController controller)
        {
            Controller = controller;
            Bitmap = new Bitmap(Controller.BitmapSize.Width, Controller.BitmapSize.Height);

            updateStart = new AutoResetEvent(false);
            updateEnd = new AutoResetEvent(false);
            updateLoop = new CancellationTokenSource();
            updateThread = new Thread(new ParameterizedThreadStart(UpdateLoop));
            updateThread.Start(updateLoop.Token);
        }

        public void Update()
        {
            UpdateAsync().WaitOne();
        }

        public WaitHandle UpdateAsync()
        {
            updateStart.Set();
            return updateEnd;
        }

        private void UpdateLoop(object state)
        {
            var token = (CancellationToken)state;

            var handles = new[]
            {
                token.WaitHandle,
                updateStart,
            };

            while (WaitHandle.WaitAny(handles) > 0)
            {
                UpdateInternal();

                updateEnd.Set();
            }
        }

        protected virtual void UpdateInternal()
        {
            using (var g = Graphics.FromImage(Bitmap))
            {
                g.SmoothingMode = SmoothingMode;
                g.Clear(BackColor);

                Draw(g);
            }
        }

        public void Dispose()
        {
            updateLoop.Cancel();
            updateThread.Join();
            updateLoop.Dispose();
            updateStart.Dispose();
            updateEnd.Dispose();
            Bitmap.Dispose();
        }

        protected abstract void Draw(Graphics g);
    }
}
