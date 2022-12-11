using System;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes {
    public class Transform {
        public Transform() {
            Pos = Vector3.Zero;
            Rot = Vector3.Zero;
        }
        public Vector3 Pos { get; set; }
        public Vector3 Rot { get; set; }
    }
}
