using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SoftwareRenderer
{
    public class AppDelegate
    {
        private Form form;
        private GraphicsBuffer buffer;
        private const int Width = 1024;
        private const int Height = 768;
        private Font defaultFont;
        private readonly TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(1000.0 / 30);

        private void LoadContent() {
            form = new Form {
                Size = new Size(Width, Height),
                StartPosition = FormStartPosition.CenterScreen
            };
            buffer = new GraphicsBuffer(Width, Height);
            defaultFont = new Font(new FontFamily("Microsoft Yahei Mono"), 14);
        }

        public AppDelegate() {
            LoadContent();
        }

        public void Run() {
            var stopwatch = new Stopwatch();
            var deltatime = maxElapsedTime;

            form.Show();

            while (!form.IsDisposed) {
                stopwatch.Start();

                Render(deltatime);
                buffer.SwapBuffers();
                Present();
                Application.DoEvents();
                if (stopwatch.Elapsed < maxElapsedTime) {
                    Thread.Sleep(maxElapsedTime - stopwatch.Elapsed);
                }
                else {
                    deltatime = maxElapsedTime;
                }

                stopwatch.Stop();
                deltatime = stopwatch.Elapsed;
                stopwatch.Reset();
            }
        }

        private void Present() {
            using (var g = form.CreateGraphics()) {
                g.DrawImage(buffer.Current, Point.Empty);
            }
        }

        private void Render(TimeSpan dt) {
            using (var g = Graphics.FromImage(buffer.Background)) {
                g.Clear(Color.White);
                g.DrawString($"FPS: {1000.0 / dt.Milliseconds:F2}", defaultFont, Brushes.Black, 0, 0);
            }
        }
    }
}