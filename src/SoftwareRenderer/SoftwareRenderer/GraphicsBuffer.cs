using System.Drawing;

namespace SoftwareRenderer
{
    public class GraphicsBuffer
    {
        public GraphicsBuffer(int width, int height) {
            Current = new Bitmap(width, height);
            CurrentGraphicsDevice = new GraphicsDevice(Current);
            Background = new Bitmap(width, height);
            BackgroundGraphicsDevice = new GraphicsDevice(Background);
        }

        public Bitmap Background { get; private set; }
        public GraphicsDevice BackgroundGraphicsDevice { get; private set; }
        public Bitmap Current { get; private set; }
        public GraphicsDevice CurrentGraphicsDevice { get; private set; }

        public void SwapBuffers() {
            var t = Current;
            Current = Background;
            Background = t;
        }
    }
}