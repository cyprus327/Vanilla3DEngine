using System;

namespace Vanilla3DEngine.Structs {
    public struct Vector3 {
        public Vector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
            W = 1f;
        }
        public Vector3(float x, float y, float z, float w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        private static readonly Random _r = new Random();

        public override string ToString() => $"({X, 4:F2}, {Y, 4:F2}, {Z, 4:F2})";
        
        public static Vector3 Zero => new Vector3(0f, 0f, 0f);
        public static Vector3 One => new Vector3(1f, 1f, 1f);
        public static Vector3 Back => new Vector3(0f, 0f, -1f);
        public static Vector3 Forward => new Vector3(0f, 0f, 1f);
        public static Vector3 Up => new Vector3(0f, 1f, 0f);
        public static Vector3 Down => new Vector3(0f, -1f, 0f);
        public static Vector3 Left => new Vector3(-1f, 0f, 0f);
        public static Vector3 Right => new Vector3(1f, 0f, 0f);
        public static Vector3 NegativeInfinity => new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
        public static Vector3 PositiveInfinity => new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);


        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator +(Vector3 a, float n) => new Vector3(a.X + n, a.Y + n, a.Z + n);
        public static Vector3 operator -(Vector3 a, float n) => new Vector3(a.X - n, a.Y - n, a.Z - n);
        public static Vector3 operator *(Vector3 a, float n) => new Vector3(a.X * n, a.Y * n, a.Z * n);
        public static Vector3 operator /(Vector3 a, float n) => new Vector3(a.X / n, a.Y / n, a.Z / n);
        public static bool operator ==(Vector3 a, Vector3 b) => a.X == b.X && a.Y == b.Z && a.Z == b.Z && a.W == b.W;
        public static bool operator !=(Vector3 a, Vector3 b) => a.X != b.X || a.Y != b.Z || a.Z != b.Z || a.W != b.W;
        public static bool operator <=(Vector3 a, Vector3 b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
        public static bool operator >=(Vector3 a, Vector3 b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;

        // overrides to make visual studio shut up
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        public static Vector3 MatrixMultiplyVector(Mat4x4 m, Vector3 v) {
            return new Vector3(v.X * m.Mat[0][0] + v.Y * m.Mat[1][0] + v.Z * m.Mat[2][0] + v.W * m.Mat[3][0],
                                      v.X * m.Mat[0][1] + v.Y * m.Mat[1][1] + v.Z * m.Mat[2][1] + v.W * m.Mat[3][1],
                                      v.X * m.Mat[0][2] + v.Y * m.Mat[1][2] + v.Z * m.Mat[2][2] + v.W * m.Mat[3][2],
                                      v.X * m.Mat[0][3] + v.Y * m.Mat[1][3] + v.Z * m.Mat[2][3] + v.W * m.Mat[3][3]);
        }

        public static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static float Magnitude(Vector3 v) => (float)Math.Sqrt(Dot(v, v));

        public float MagnitudeSquared() => (X * X) + (Y * Y) + (Z * Z);

        public static Vector3 Normalize(Vector3 v) => v / Magnitude(v);

        public static Vector3 Cross(Vector3 a, Vector3 b) => new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

        public static Vector3 IntersectPlane(Vector3 planeP, Vector3 planeN, Vector3 lineStart, Vector3 lineEnd) {
            planeN = Normalize(planeN);
            float planeD = -Dot(planeN, planeP);
            float ad = Dot(lineStart, planeN);
            float bd = Dot(lineEnd, planeN);
            float t = (-planeD - ad) / (bd - ad);
            Vector3 lineStartToEnd = lineEnd - lineStart;
            Vector3 lineToIntersect = lineStartToEnd * t;
            return lineStart + lineToIntersect;
        }

        public static Vector3 Project(Vector3 v, Vector3 onto) {
            return onto * (Dot(v, onto) / Dot(onto, onto));
        }

        public static Vector3 Min(Vector3 a, Vector3 b) {
            return new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static Vector3 Max(Vector3 a, Vector3 b) {
            return new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        public static Vector3 Random(int bottomRange, int topRange) => 
            new Vector3(_r.Next(bottomRange, topRange), _r.Next(bottomRange, topRange), _r.Next(bottomRange, topRange));
    }
}
