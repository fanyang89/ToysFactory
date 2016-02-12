using System;
using System.Drawing;

namespace SoftwareRenderer
{
    public class GraphicsBuffer : IDisposable
    {
        public GraphicsBuffer(int width, int height, Camera camera) {
            Current = new Bitmap(width, height);
            CurrentGraphicsDevice = new GraphicsDevice(Current, camera);
            Background = new Bitmap(width, height);
            BackgroundGraphicsDevice = new GraphicsDevice(Background, camera);
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

        public void Dispose() {
            CurrentGraphicsDevice.Dispose();
            Current.Dispose();
            BackgroundGraphicsDevice.Dispose();
            Background.Dispose();
        }
    }
}