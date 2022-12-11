using System.Collections.Generic;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes {
    public class TriangleDepthComparer : IComparer<Triangle> { // class used to sort lists of triangles based on their z depth
        public int Compare(Triangle x, Triangle y) {
            return (x.Points[0].Z + x.Points[1].Z + x.Points[2].Z) / 3f > (y.Points[0].Z + y.Points[1].Z + y.Points[2].Z) / 3f ? -1 : 1;
        }
    }
}
