using System.Drawing;

namespace SoftwareRenderer
{
    public class GraphicsBuffer
    {
        public Bitmap Current { get; private set; }
        public Bitmap Background { get; private set; }

        public GraphicsBuffer(int width, int height) {
            Current = new Bitmap(width, height);
            Background = new Bitmap(width, height);
        }

        public void SwapBuffers() {
            var t = Current;
            Current = Background;
            Background = t;
        }
    }
}