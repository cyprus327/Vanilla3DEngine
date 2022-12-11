using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Vanilla3DEngine.Classes;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine {
    public class BasicExample : Engine{
        public BasicExample(Size screenSize, string title) : base(screenSize, title) {
        }

        private readonly GameObject sphere = new GameObject(Mesh.SphereHighPoly) {
            Transform = new Transform() { Pos = new Vector3(2.5f, 0f, 0f) },
            Dynamic = false
        };
        private readonly GameObject cube = new GameObject(Mesh.Cube) {
            Transform = new Transform() { Pos = new Vector3(-2.5f, 0f, 0f) },
            Dynamic = false
        };

        // You can either instantiate an object and have it stored and rendered until
        // you remove it, or just Draw a certain object.
        public override void Awake() {
            Instantiate(cube, 0);
        }

        public override void Update(Graphics g, float deltaTime) {
            HandleInput();
            g.DrawString("Press F to show a sphere", ScreenTextFont, ScreenTextBrush, 0, 0);
            if (Input.GetKeyDown('F')) DrawObject(sphere);
        }
    }
}
