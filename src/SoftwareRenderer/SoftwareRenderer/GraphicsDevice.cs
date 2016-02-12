using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SoftwareRenderer
{
    public class GraphicsDevice : IDisposable
    {
        private readonly byte[] bytes;
        private readonly Camera camera;
        private readonly Bitmap canvas;
        private readonly Graphics canvasGraphics;

        private readonly byte[] emptyBytes;
        private readonly float[] zBuffer;

        private Color lastClearColor;

        public GraphicsDevice(Bitmap bitmap, Camera camera) {
            canvas = bitmap;
            this.camera = camera;
            canvasGraphics = Graphics.FromImage(canvas);
            bytes = new byte[Width * Height * 4];
            emptyBytes = new byte[Width * Height * 4];
            zBuffer = new float[Width * Height];
        }

        private int Height => canvas.Height;
        private int Width => canvas.Width;

        public void Clear(Color color) {
            Flush();
            if (color != lastClearColor) {
                for (var i = 0; i < emptyBytes.Length; i += 4) {
                    emptyBytes[i] = color.B;
                    emptyBytes[i + 1] = color.G;
                    emptyBytes[i + 2] = color.R;
                    emptyBytes[i + 3] = color.A;
                }
                lastClearColor = color;
            }
            Array.Clear(zBuffer, 0, zBuffer.Length);
            Array.Copy(emptyBytes, bytes, emptyBytes.Length);
        }

        public void Dispose() {
            canvasGraphics.Dispose();
        }

        public void DrawLine(Vector p0, Vector p1, Color color) {
            canvasGraphics.DrawLine(new Pen(color), p0, p1);
        }

        public void DrawMeshes(Mesh[] meshes, Color color) {
            var view = Matrix.LookAtLH(camera.Position, camera.Target, Vector.UnitY);
            var projection = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0), (float)Width / Height, 0.1f, 1);

            foreach (var mesh in meshes) {
                var rotation = Matrix.Rotation(mesh.Rotation);
                var translation = Matrix.Translation(mesh.Position);
                var world = rotation * translation;
                var transform = world * view * projection;

                foreach (var face in mesh.Surfaces) {
                    var v1 = Project(mesh.Vertices[face.A], transform);
                    var v2 = Project(mesh.Vertices[face.B], transform);
                    var v3 = Project(mesh.Vertices[face.C], transform);
                    DrawTriangle(v1, v2, v3, color);
                }
            }
        }

        public void DrawPoint(Vector point, Color color) {
            DrawPoint((int)point.X, (int)point.Y, point.Z, color);
        }

        public void DrawPoint(int x, int y, Color color) {
            SetPixel(x, y, color);
        }

        public void DrawPoint(int x, int y, float z, Color color) {
            var index = y * Width + x;
            if (zBuffer[index] > z) {
                return;
            }
            zBuffer[index] = z;
            SetPixel(x, y, color);
        }

        public void DrawString(string str, Font font, Brush brush, float x, float y) {
            canvasGraphics.DrawString(str, font, brush, x, y);
        }

        public void DrawTriangle(Vector pa, Vector pb, Vector pc, Color color) {
            var a = pa - pb;
            var b = pc - pb;
            var n = a.Cross(b);
            var v = camera.Target - camera.Position;
            if (n.Dot(v) >= 0) {
                return;
            }
            DrawLine(pa, pb, color);
            DrawLine(pa, pc, color);
            DrawLine(pc, pb, color);
        }

        public void Flush() {
            var bits = canvas.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, canvas.PixelFormat);
            Marshal.Copy(bytes, 0, bits.Scan0, bytes.Length);
            canvas.UnlockBits(bits);
        }

        private Vector Project(Vector coord, Matrix transformMatrix) {
            var p = transformMatrix.Transform(coord);
            p.X = p.X * Width + Width / 2f;
            p.Y = -p.Y * Height + Height / 2f;
            return p;
        }

        private void SetPixel(int x, int y, Color color) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) {
                return;
            }
            var index = (x + y * Width) * 4;
            bytes[index] = color.B;
            bytes[index + 1] = color.G;
            bytes[index + 2] = color.R;
            bytes[index + 3] = color.A;
        }
    }
}