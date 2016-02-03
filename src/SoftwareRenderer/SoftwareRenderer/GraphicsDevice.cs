using System;
using System.Drawing;

namespace SoftwareRenderer
{
    public class GraphicsDevice : IDisposable
    {
        private readonly Bitmap canvas;
        private readonly Graphics canvasGraphics;

        public GraphicsDevice(Bitmap bitmap) {
            canvas = bitmap;
            canvasGraphics = Graphics.FromImage(canvas);
        }

        private int Height => canvas.Height;
        private int Width => canvas.Width;

        public void Clear(Color color) {
            canvasGraphics.Clear(color);
        }

        public void Dispose() {
            canvasGraphics.Dispose();
        }

        public void DrawLine(Point p0, Point p1, Color color) {
            canvasGraphics.DrawLine(new Pen(color), p0, p1);
        }

        public void DrawLine(Vector p0, Vector p1, Color color) {
            DrawLine(new Point((int)p0.X, (int)p0.Y), new Point((int)p1.X, (int)p1.Y), color);
        }

        public void DrawMeshes(Mesh[] meshes, Camera camera, Color color) {
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

        public void DrawPoint(int x, int y, Color color) {
            SetPixel(x, y, color);
        }

        public void DrawString(string str, Font font, Brush brush, float x, float y) {
            canvasGraphics.DrawString(str, font, brush, x, y);
        }

        public void DrawTriangle(Vector pa, Vector pb, Vector pc, Color color) {
            DrawLine(pa, pb, color);
            DrawLine(pa, pc, color);
            DrawLine(pc, pb, color);
        }

        private Vector Project(Vector coord, Matrix transformMatrix) {
            var p = transformMatrix.Transform(coord);
            p.X = p.X * Width + Width / 2f;
            p.Y = -p.Y * Height + Height / 2f;
            return p;
        }

        private void SetPixel(int x, int y, Color color) {
            canvas.SetPixel(x, y, color);
        }
    }
}