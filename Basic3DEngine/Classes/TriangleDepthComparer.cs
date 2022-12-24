using System.Collections.Generic;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes {
    public class TriangleDepthComparer : IComparer<Triangle> { // class used to sort lists of triangles based on their z depth
        public int Compare(Triangle x, Triangle y) {
            return (x.Verts[0].Z + x.Verts[1].Z + x.Verts[2].Z) / 3f > (y.Verts[0].Z + y.Verts[1].Z + y.Verts[2].Z) / 3f ? -1 : 1;
        }
    }
}
