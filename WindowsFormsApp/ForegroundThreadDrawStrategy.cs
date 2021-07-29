using System;
using System.Drawing;
using System.Threading;

namespace WindowsFormsApp
{
    public class ForegroundThreadDrawStrategy : DrawStrategy
    {
        private readonly AutoResetEvent updateStart;
        private readonly CancellationTokenSource updateLoop;
        private readonly Thread updateThread;

        public ForegroundThreadDrawStrategy(Layer layer, Action<Graphics> drawAction) : base(layer, drawAction)
        {
            updateStart = new AutoResetEvent(false);
            updateLoop = new CancellationTokenSource();
            updateThread = new Thread(new ParameterizedThreadStart(UpdateLoop));
            updateThread.Start(updateLoop.Token);
        }

        protected override void BeginUpdate()
        {
            updateStart.Set();
        }

        protected override void Dispose(bool disposing)
        {
            updateLoop.Cancel();
            updateThread.Join();
            updateLoop.Dispose();
            updateStart.Dispose();
            base.Dispose(disposing);
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
            }
        }
    }
}
