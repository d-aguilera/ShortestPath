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

        private LayerController controller;

        public LayerController Controller
        {
            protected get => controller;
            set
            {
                controller = value;

                Bitmap = new Bitmap(value.BitmapSize.Width, value.BitmapSize.Height);

                ControllerSet();
            }
        }

        public Image Bitmap { get; private set; }
        public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.HighQuality;

        public Layer()
        {
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

        public void Dispose()
        {
            updateLoop.Cancel();
            updateThread.Join();
            updateLoop.Dispose();
            updateStart.Dispose();
            updateEnd.Dispose();
            Bitmap.Dispose();
        }

        protected virtual void ControllerSet()
        {
        }

        protected virtual void UpdateInternal()
        {
            using (var g = Graphics.FromImage(Bitmap))
            {
                g.SmoothingMode = SmoothingMode;
                g.Clear(Color.Transparent);

                Draw(g);
            }
        }

        protected abstract void Draw(Graphics g);

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
    }
}
