using System;
using System.Drawing;
using Vanilla3DEngine.Structs;
using Vanilla3DEngine.Classes;

namespace Vanilla3DEngine {
    public class PrimitivesExplosionExample : Engine {
        public PrimitivesExplosionExample(Size screenSize, string title) : base(screenSize, title) {
        }

        private readonly Random _r = new Random();

        public override void Awake() {
            ShowDebugInfo = true;
            MainCamera.Transform.Pos = new Vector3(0f, 20f, -10f);
        }

        public override void Update(Graphics g, float deltaTime) {
            base.HandleInput(10f, 2.5f);

            HandlePseudoPhysics(deltaTime);

            if (Input.GetKeyUp('\t')) {
                base.ShowMessageOnCenter(g, "Generating...");
                Generate(100, 5, new Vector3(0f, 0f, 50f));
            }
            if (Input.GetKeyDown('R')) {
                MainCamera.Transform.Pos = Vector3.Zero;
            }

            g.DrawString("Press tab to generate again", ScreenTextFont, ScreenTextBrush, 0, ScreenSize.Height - 65);
        }

        private void HandlePseudoPhysics(float deltaTime) {
            GameObject obj;
            foreach (var key in GetObjects.Keys) {
                obj = GetObjects[key];
                if (!obj.Dynamic) continue;
                obj.Transform.Rot += new Vector3(1f, 0.5f, 1f) * deltaTime;
                if (obj.Transform.Pos.Y <= 0f) {
                    obj.Transform.Pos = new Vector3(obj.Transform.Pos.X, 0f, obj.Transform.Pos.Z);
                    obj.Vel = new Vector3(obj.Vel.X, Math.Abs(obj.Vel.Y / 2.2f), obj.Vel.Z);
                }
            }
        }

        private void Generate(int objectCount, int size, Vector3 offset) {
            ClearRenderStack();
            GetObjects.Clear();
            
            for (int i = 1; GetObjects.Count < objectCount; i++) {
                Vector3 v = Vector3.Random(-size, size);
                int selected = _r.Next(0, size);
                GameObject rand = new GameObject(selected < size / 2 ? Mesh.Cube : Mesh.Sphere) {
                    Col = Color.FromArgb(_r.Next(70, 255), _r.Next(70, 255), _r.Next(70, 255)),
                    Transform = new Transform() {
                        Rot = Vector3.Random(0, 360) * (float)Math.PI / 180f,
                        Pos = v + offset
                    },
                    Mass = _r.Next(1, 10),
                    Vel = new Vector3(_r.Next(-15, 15), _r.Next(15, 50), _r.Next(-15, 15))
                };
                base.Instantiate(rand, i);
            }
        }
    }
}






