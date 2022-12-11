using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes {
    public class Camera {
        public Camera(float fov, float nearPlane) {
            FOV = fov;
            NearPlane = nearPlane;
            Transform = new Transform() {
                Pos = Vector3.Zero,
                Rot = Vector3.Zero
            };
        }

        public float FOV { get; }
        public float NearPlane { get; set; }
        public Transform Transform { get; set; }
    }
}
