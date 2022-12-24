using System;
using System.Drawing;
using System.Linq;
using Vanilla3DEngine.Classes;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine {
    public class BasicExample : Engine{
        public BasicExample(Size screenSize, string title) : base(screenSize, title) {
        }

        private readonly GameObject obj1 = new GameObject(Mesh.Cube) {
            Transform = new Transform() { Pos = new Vector3(0f, 5f, 10f) },
            Dynamic = true,
            Col = Color.Red
        };
        private readonly GameObject obj2 = new GameObject(Mesh.Cube) {
            Transform = new Transform() { Pos = new Vector3(0f, -2f, 10f) },
            Dynamic = false
        };

        public override void Awake() {
            //ShowDebugInfo = true;
            Instantiate(obj1, 0);
            Instantiate(obj2, 1);
        }

        public override void Update(Graphics g, float deltaTime) {
            base.HandleInput();
            GameObject.HandleCollisions();

            if (Input.GetKeyDown('R')) {
                obj1.Dynamic = false;
            }
            else if (obj1.Transform.Pos.Y <= -8.5f || Input.GetKeyDown('\t')) {
                obj1.Transform.Pos = new Vector3(0f, 5f, 10f);
                obj1.Vel = Vector3.Zero;
            }
            else {
                obj1.Dynamic = true;
            }
        }
    }
}
