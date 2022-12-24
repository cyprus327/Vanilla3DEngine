using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanilla3DEngine.Structs {
    public struct BoundingBox {
        public BoundingBox(Vector3 min, Vector3 max) {
            Min = min;
            Max = max;
        }

        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        public static BoundingBox GetBoundsOf(Mesh mesh) {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (Triangle triangle in mesh.Tris) {
                foreach (Vector3 point in triangle.Verts) {
                    min = Vector3.Min(min, point);
                    max = Vector3.Max(max, point);
                }
            }
            return new BoundingBox(min, max);
        }

        public static bool CheckCollision(BoundingBox a, BoundingBox b) {
            return a.Min.X <= b.Max.X && b.Min.X <= a.Max.X &&
             a.Min.Y <= b.Max.Y && b.Min.Y <= a.Max.Y &&
             a.Min.Z <= b.Max.Z && b.Min.Z <= a.Max.Z;
        }
    }
}
