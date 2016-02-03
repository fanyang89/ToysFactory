using System;
using System.Linq;

namespace SoftwareRenderer
{
    public struct Matrix
    {
        public Matrix(float[] values = null) {
            Values = new float[16];
            if (values == null) {
                return;
            }
            for (var i = 0; i < Values.Length; i++) {
                Values[i] = values[i];
            }
        }

        public static Matrix Identity => new Matrix(new float[16] {
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        });

        public float[] Values { get; }

        public static Matrix LookAtLH(Vector eye, Vector target, Vector up) {
            var axisZ = (target - eye).Normalize();
            var axisX = up.Cross(axisZ).Normalize();
            var axisY = axisZ.Cross(axisX).Normalize();

            var eyeX = -axisX.Dot(eye);
            var eyeY = -axisY.Dot(eye);
            var eyeZ = -axisZ.Dot(eye);

            return new Matrix(new[] {
                axisX.X, axisY.X, axisZ.X, 0,
                axisX.Y, axisY.Y, axisZ.Y, 0,
                axisX.Z, axisY.Z, axisZ.Z, 0,
                eyeX, eyeY, eyeZ, 1
            });
        }

        public static bool operator !=(Matrix a, Matrix b) {
            return !(a == b);
        }

        public static Matrix operator *(Matrix a, Matrix b) {
            var values = new float[16];
            for (var index = 0; index < 16; index++) {
                var i = index / 4;
                var j = index % 4;
                values[index] =
                    a.Values[i * 4] * b.Values[j] +
                    a.Values[i * 4 + 1] * b.Values[1 * 4 + j] +
                    a.Values[i * 4 + 2] * b.Values[2 * 4 + j] +
                    a.Values[i * 4 + 3] * b.Values[3 * 4 + j];
            }
            return new Matrix(values);
        }

        public static bool operator ==(Matrix a, Matrix b) {
            return !a.Values.Where((t, i) => Math.Abs(t - b.Values[i]) > float.MinValue).Any();
        }

        public static Matrix PerspectiveFovLH(float fieldOfView, float aspect, float znear, float zfar) {
            var height = 1 / (float)Math.Tan(fieldOfView / 2);
            var width = height / aspect;
            return new Matrix(new[] {
                width, 0, 0, 0,
                0, height, 0, 0,
                0, 0, zfar/(zfar - znear), 1,
                0, 0, znear*zfar/(znear - zfar), 0
            });
        }

        public static Matrix Rotation(Vector r) {
            var x = RotationX(r.X);
            var y = RotationY(r.Y);
            var z = RotationZ(r.Z);
            return z * x * y;
        }

        public static Matrix RotationX(float angle) {
            var s = (float)Math.Sin(angle);
            var c = (float)Math.Cos(angle);
            var values = new[] {
            1, 0,  0, 0,
            0, c,  s, 0,
            0, -s, c, 0,
            0, 0,  0, 1
        };
            return new Matrix(values);
        }

        public static Matrix RotationY(float angle) {
            var s = (float)Math.Sin(angle);
            var c = (float)Math.Cos(angle);
            var values = new[] {
            c, 0, -s, 0,
            0, 1, 0,  0,
            s, 0, c,  0,
            0, 0, 0,  1
        };
            return new Matrix(values);
        }

        public static Matrix RotationZ(float angle) {
            var s = (float)Math.Sin(angle);
            var c = (float)Math.Cos(angle);
            var values = new[] {
            c,  s, 0, 0,
            -s, c, 0, 0,
            0,  0, 1, 0,
            0,  0, 0, 1,
        };
            return new Matrix(values);
        }

        public static Matrix Scale(Vector s) {
            var values = new[] {
                s.X, 0, 0, 0,
                0, s.Y, 0, 0,
                0, 0, s.Z, 0,
                0, 0, 0, 1
            };
            return new Matrix(values);
        }

        public static Matrix Translation(Vector t) {
            var values = new[] {
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                t.X, t.Y, t.Z, 1
            };
            return new Matrix(values);
        }

        public bool Equals(Matrix other) {
            return Equals(Values, other.Values);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            return obj is Matrix && Equals((Matrix)obj);
        }

        public override int GetHashCode() {
            return Values?.GetHashCode() ?? 0;
        }

        public Vector Transform(Vector v) {
            var x = v.X * Values[0 * 4 + 0] + v.Y * Values[1 * 4 + 0] + v.Z * Values[2 * 4 + 0] + Values[3 * 4 + 0];
            var y = v.X * Values[0 * 4 + 1] + v.Y * Values[1 * 4 + 1] + v.Z * Values[2 * 4 + 1] + Values[3 * 4 + 1];
            var z = v.X * Values[0 * 4 + 2] + v.Y * Values[1 * 4 + 2] + v.Z * Values[2 * 4 + 2] + Values[3 * 4 + 2];
            var w = v.X * Values[0 * 4 + 3] + v.Y * Values[1 * 4 + 3] + v.Z * Values[2 * 4 + 3] + Values[3 * 4 + 3];
            return new Vector(x / w, y / w, z / w);
        }
    }
}