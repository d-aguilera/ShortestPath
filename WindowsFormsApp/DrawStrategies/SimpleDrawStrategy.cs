using System;
using System.Drawing;

namespace WindowsFormsApp
{
    public class SimpleDrawStrategy : DrawStrategy
    {
        public SimpleDrawStrategy(Layer layer, Action<Graphics> drawAction) : base(layer, drawAction)
        {
        }

        protected override void BeginUpdate()
        {
            UpdateInternal();
        }
    }
}
