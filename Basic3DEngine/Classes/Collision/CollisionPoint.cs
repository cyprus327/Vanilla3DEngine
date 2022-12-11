using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes.Collision {
    public class CollisionPoint {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Vector3 Normal { get; set; }
        public float Depth { get; set; }
        public bool HasCollision { get; set; }
    }
}
