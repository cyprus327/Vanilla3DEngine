using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes.Collision {
    public abstract class Collider {
        public abstract CollisionPoint Test(Transform t, Collider col, Transform colTransform);
    }
}
