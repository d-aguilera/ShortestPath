using System.Threading;

namespace WindowsFormsApp
{
    public interface IDrawStrategy
    {
        void Update();
        WaitHandle UpdateAsync();
    }
}
