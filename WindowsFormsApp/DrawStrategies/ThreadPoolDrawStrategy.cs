using System;
using System.Drawing;
using System.Threading;

namespace WindowsFormsApp
{
    public class ThreadPoolDrawStrategy : DrawStrategy
    {
        public ThreadPoolDrawStrategy(Layer layer, Action<Graphics> drawAction) : base(layer, drawAction)
        {
        }

        protected override void BeginUpdate()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                UpdateInternal();
            });
        }
    }
}
