using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes {
    public class GameObject {
        public GameObject(Mesh mesh) {
            Mesh = mesh;
            Mass = 0.1f;
            Force = Vector3.Zero;
            Vel = Vector3.Zero;
            Transform = new Transform() { Pos = Vector3.Zero, Rot = Vector3.Zero };
            Col = Color.White;
            Dynamic = true;
        }

        public float Mass { get; set; }
        public Vector3 Force { get; set; }
        public Vector3 Vel { get; set; }
        public Color Col { get; set; }
        public Mesh Mesh { get; set; }
        public Transform Transform { get; set; }
        public bool Dynamic { get; set; }

        public virtual void Step(float deltaTime, Vector3 gravity) {
            if (!Dynamic) return;

            Force = gravity * Mass;
            Vel += Force / Mass * deltaTime;
            Transform.Pos += Vel * deltaTime;
            Force = Vector3.Zero;
        }
    }
}
