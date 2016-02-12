using System.Drawing;

namespace SoftwareRenderer
{
    public struct Vector
    {
        public Vector(float x, float y, float z) {
            Values = new float[3];
            Values[0] = x;
            Values[1] = y;
            Values[2] = z;
        }

        public Vector(float value) {
            Values = new float[3];
            Values[0] = value;
            Values[1] = value;
            Values[2] = value;
        }

        public float Length =>
            (float)System.Math.Sqrt(Values[0] * Values[0] + Values[1] * Values[1] + Values[2] * Values[2])
            ;

        public static Vector One => new Vector(1);

        public static Vector UnitX => new Vector(1, 0, 0);

        public static Vector UnitY => new Vector(0, 1, 0);

        public static Vector UnitZ => new Vector(0, 0, 1);

        public float X
        {
            get { return Values[0]; }
            set { Values[0] = value; }
        }

        public float Y
        {
            get { return Values[1]; }
            set { Values[1] = value; }
        }

        public float Z
        {
            get { return Values[2]; }
            set { Values[2] = value; }
        }

        public static Vector Zero => new Vector(0);
        private float[] Values { get; }

        public static implicit operator Point(Vector v) {
            return new Point((int)v.X, (int)v.Y);
        }

        public static implicit operator PointF(Vector v) {
            return new PointF(v.X, v.Y);
        }

        public static Vector operator -(Vector a, Vector b) {
            return new Vector(0) {
                Values = {
                    [0] = a.Values[0] - b.Values[0],
                    [1] = a.Values[1] - b.Values[1],
                    [2] = a.Values[2] - b.Values[2]
                }
            };
        }

        public static Vector operator *(Vector a, float factor) {
            return new Vector(0) {
                Values = {
                    [0] = a.Values[0]*factor,
                    [1] = a.Values[1]*factor,
                    [2] = a.Values[2]*factor
                }
            };
        }

        public static Vector operator /(Vector a, float factor) {
            return new Vector(0) {
                Values = {
                    [0] = a.Values[0]/factor,
                    [1] = a.Values[1]/factor,
                    [2] = a.Values[2]/factor
                }
            };
        }

        public static Vector operator +(Vector a, Vector b) {
            return new Vector(0) {
                Values = {
                    [0] = a.Values[0] + b.Values[0],
                    [1] = a.Values[1] + b.Values[1],
                    [2] = a.Values[2] + b.Values[2]
                }
            };
        }

        public Vector Cross(Vector v) {
            return new Vector(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        public float Dot(Vector v) {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public Vector Interpolate(Vector v, float factor) {
            return this + (v - this) * factor;
        }
        public Vector Normalize() {
            var length = Length;
            var factor = 0f;
            if (length > 0) {
                factor = 1.0f / length;
            }
            return new Vector(X * factor, Y * factor, Z * factor);
        }
    }
}