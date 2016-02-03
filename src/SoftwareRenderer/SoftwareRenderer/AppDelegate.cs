using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SoftwareRenderer
{
    public class AppDelegate : IDisposable
    {
        private const int Height = 768;
        private const int MaxFps = 24;
        private const int Width = 1024;
        private readonly Font defaultFont = new Font(new FontFamily("Microsoft Yahei Mono"), 14);
        private readonly TimeSpan maxBreakTime = TimeSpan.FromSeconds(2);
        private readonly TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(1000.0 / MaxFps);
        private GraphicsBuffer buffer;
        private Camera camera;
        private Form form;
        private Graphics formGraphics;
        private Mesh[] meshes;

        public AppDelegate() {
            LoadContent();
        }

        public void Dispose() {
            buffer.BackgroundGraphicsDevice.Dispose();
            buffer.CurrentGraphicsDevice.Dispose();
            formGraphics.Dispose();
        }

        public void Run() {
            var stopwatch = new Stopwatch();
            var deltatime = maxElapsedTime;

            form.Show();

            while (!form.IsDisposed) {
                stopwatch.Start();

                Update(deltatime);
                Render(deltatime);
                buffer.SwapBuffers();
                Present();
                Application.DoEvents();
                if (stopwatch.Elapsed < maxElapsedTime) {
                    Thread.Sleep(maxElapsedTime - stopwatch.Elapsed);
                }

                stopwatch.Stop();
                deltatime = stopwatch.Elapsed;
                if (deltatime > maxBreakTime) {
                    deltatime = maxElapsedTime;
                }
                stopwatch.Reset();
            }
        }

        private void LoadContent() {
            form = new Form {
                Size = new Size(Width, Height),
                StartPosition = FormStartPosition.CenterScreen
            };
            formGraphics = form.CreateGraphics();
            buffer = new GraphicsBuffer(Width, Height);
            meshes = Mesh.FromBabylonModel("Contents/monkey.babylon");
            camera = new Camera { Position = new Vector(0, 0, 10), Target = Vector.Zero };
        }

        private void Present() {
            formGraphics.DrawImage(buffer.Current, Point.Empty);
        }

        private void Render(TimeSpan dt) {
            var device = buffer.BackgroundGraphicsDevice;
            device.Clear(Color.White);
            device.DrawMeshes(meshes, camera, Color.CadetBlue);
            device.DrawString($"FPS: {1000.0 / dt.Milliseconds:F2}", defaultFont, Brushes.Black, 0, 0);
        }

        private void Update(TimeSpan dt) {
            foreach (var mesh in meshes) {
                mesh.Rotation += new Vector(0, 0.1f, 0);
            }
        }
    }
}