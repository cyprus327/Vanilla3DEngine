using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanilla3DEngine.Classes;

namespace Vanilla3DEngine.Structs {
    public struct Mat4x4 {
        public Mat4x4(float[][] mat) {
            if (mat != null)
                Mat = mat;
            else
                Mat = New.Mat;
        }

        public float[][] Mat { get; set; }

        public static Mat4x4 New => new Mat4x4() {
            Mat = new float[4][] {
                new float[4] { 0f, 0f, 0f, 0f },
                new float[4] { 0f, 0f, 0f, 0f },
                new float[4] { 0f, 0f, 0f, 0f },
                new float[4] { 0f, 0f, 0f, 0f }
            }
        };

        public static Mat4x4 IntrinsicFromEuler(Vector3 v) {
            Mat4x4 mat = New;

            float qx = (float)Math.Sin(v.X) / 2f;
            float qy = (float)Math.Sin(v.Y) / 2;
            float qz = (float)Math.Sin(v.Z) / 2;
            float qw = (float)Math.Sqrt(1 - qx * qx - qy * qy - qz * qz);

            // create the rotation matrix from the quaternion because with a quaternion gimble lock is avoided
            mat.Mat = new float[4][] {
                new float[4] { 1 - 2 * qy * qy - 2 * qz * qz, 2 * qx * qy - 2 * qz * qw, 2 * qx * qz + 2 * qy * qw, 0 },
                new float[4] { 2 * qx * qy + 2 * qz * qw, 1 - 2 * qx * qx - 2 * qz * qz, 2 * qy * qz - 2 * qx * qw, 0 },
                new float[4] { 2 * qx * qz - 2 * qy * qw, 2 * qy * qz + 2 * qx * qw, 1 - 2 * qx * qx - 2 * qy * qy, 0 },
                new float[4] { 1, 1, 1, 1 },
            };

            return mat;
        }

        public static Mat4x4 MakeIdentity() {
            Mat4x4 mat = New;
            mat.Mat[0][0] = 1f;
            mat.Mat[1][1] = 1f;
            mat.Mat[2][2] = 1f;
            mat.Mat[3][3] = 1f;
            return mat;
        }

        public static Mat4x4 MakeRotationX(float angleRad) {
            Mat4x4 mat = New;
            mat.Mat[0][0] = 1f;
            mat.Mat[1][1] = (float)Math.Cos(angleRad);
            mat.Mat[1][2] = (float)Math.Sin(angleRad);
            mat.Mat[2][1] = (float)-Math.Sin(angleRad);
            mat.Mat[2][2] = (float)Math.Cos(angleRad);
            mat.Mat[3][3] = 1f;
            return mat;
        }
        public static Mat4x4 MakeRotationY(float angleRad) {
            Mat4x4 mat = New;
            mat.Mat[0][0] = (float)Math.Cos(angleRad);
            mat.Mat[0][2] = (float)Math.Sin(angleRad);
            mat.Mat[2][0] = (float)-Math.Sin(angleRad);
            mat.Mat[1][1] = 1f;
            mat.Mat[2][2] = (float)Math.Cos(angleRad);
            mat.Mat[3][3] = 1f;
            return mat;
        }
        public static Mat4x4 MakeRotationZ(float angleRad) {
            Mat4x4 mat = New;
            mat.Mat[0][0] = (float)Math.Cos(angleRad);
            mat.Mat[0][1] = (float)Math.Sin(angleRad);
            mat.Mat[1][0] = (float)-Math.Sin(angleRad);
            mat.Mat[1][1] = (float)Math.Cos(angleRad);
            mat.Mat[2][2] = 1f;
            mat.Mat[3][3] = 1f;
            return mat;
        }

        public static Mat4x4 MakeTranslation(float x, float y, float z) {
            Mat4x4 mat = New;
            mat.Mat[0][0] = 1f;
            mat.Mat[1][1] = 1f;
            mat.Mat[2][2] = 1f;
            mat.Mat[3][3] = 1f;
            mat.Mat[3][0] = x;
            mat.Mat[3][1] = y;
            mat.Mat[3][2] = z;
            return mat;
        }

        public static Mat4x4 MultiplyMatrix(Mat4x4 a, Mat4x4 b) {
            Mat4x4 mat = New;
            for (int c = 0; c < 4; c++) {
                for (int r = 0; r < 4; r++) {
                    mat.Mat[r][c] = a.Mat[r][0] * b.Mat[0][c] + a.Mat[r][1] * b.Mat[1][c] + a.Mat[r][2] * b.Mat[2][c] + a.Mat[r][3] * b.Mat[3][c];
                }
            }
            return mat;
        }

        public static Mat4x4 MakeProjection(float fovDeg, float aspectRatio, float near, float far) {
            Mat4x4 mat = New;
            float fovRad = 1f / (float)Math.Tan(fovDeg / 2f / 180f * Math.PI);
            mat.Mat[0][0] = aspectRatio * fovRad;
            mat.Mat[1][1] = fovRad;
            mat.Mat[2][2] = far / (far - near);
            mat.Mat[3][2] = (-far * near) / (far - near);
            mat.Mat[2][3] = 1f;
            mat.Mat[3][3] = 0f;
            return mat;
        }

        public static Mat4x4 PointAt(Vector3 pos, Vector3 target, Vector3 up) {
            Vector3 newForward = target - pos;
            newForward = Vector3.Normalize(newForward);

            // calculate up direction
            Vector3 a = newForward * Vector3.Dot(up, newForward);
            Vector3 newUp = up - a;
            newUp = Vector3.Normalize(newUp);

            // new right direction is cross product
            Vector3 newRight = Vector3.Cross(newUp, newForward);

            Mat4x4 mat = New;
            mat.Mat[0][0] = newRight.X;
            mat.Mat[0][1] = newRight.Y;
            mat.Mat[0][2] = newRight.Z;
            mat.Mat[0][3] = 1f;

            mat.Mat[1][0] = newUp.X;
            mat.Mat[1][1] = newUp.Y;
            mat.Mat[1][2] = newUp.Z;
            mat.Mat[1][3] = 1f;

            mat.Mat[2][0] = newForward.X;
            mat.Mat[2][1] = newForward.Y;
            mat.Mat[2][2] = newForward.Z;
            mat.Mat[2][3] = 1f;

            mat.Mat[3][0] = pos.X;
            mat.Mat[3][1] = pos.Y;
            mat.Mat[3][2] = pos.Z;
            mat.Mat[3][3] = 1f;

            return mat;
        }

        public static Mat4x4 Inverse(Mat4x4 m) {
            Mat4x4 mat = New;
            mat.Mat[0][0] = m.Mat[0][0];
            mat.Mat[0][1] = m.Mat[1][0];
            mat.Mat[0][2] = m.Mat[2][0];

            mat.Mat[1][0] = m.Mat[0][1];
            mat.Mat[1][1] = m.Mat[1][1];
            mat.Mat[1][2] = m.Mat[2][1];

            mat.Mat[2][0] = m.Mat[0][2];
            mat.Mat[2][1] = m.Mat[1][2];
            mat.Mat[2][2] = m.Mat[2][2];

            mat.Mat[3][0] = -(m.Mat[3][0] * mat.Mat[0][0] + m.Mat[3][1] * mat.Mat[1][0] + m.Mat[3][2] * mat.Mat[2][0]);
            mat.Mat[3][1] = -(m.Mat[3][0] * mat.Mat[0][1] + m.Mat[3][1] * mat.Mat[1][1] + m.Mat[3][2] * mat.Mat[2][1]);
            mat.Mat[3][2] = -(m.Mat[3][0] * mat.Mat[0][2] + m.Mat[3][1] * mat.Mat[1][2] + m.Mat[3][2] * mat.Mat[2][2]);
            mat.Mat[3][3] = 1f;

            return mat;
        }
    }
}
