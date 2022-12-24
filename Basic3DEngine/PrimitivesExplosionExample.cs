using System;
using System.Drawing;
using System.Linq;
using Vanilla3DEngine.Structs;
using Vanilla3DEngine.Classes;

namespace Vanilla3DEngine {
    public class PrimitivesExplosionExample : Engine {
        public PrimitivesExplosionExample(Size screenSize, string title) : base(screenSize, title) {
        }

        private readonly Random _r = new Random();

        public override void Awake() {
            ShowDebugInfo = true;
            MainCamera.Transform.Pos = new Vector3(0f, 10f, -20f);
        }

        public override void Update(Graphics g, float deltaTime) {
            base.HandleInput(10f, 2.5f);

            SimulateObjectBounds(10);
            GameObject.HandleCollisions();

            if (Input.GetKeyUp('\t')) {
                base.ShowMessageOnCenter(g, "Generating...");
                Generate(22, 10, new Vector3(0f, 0f, 50f));
            }
            if (Input.GetKeyDown('R')) {
                MainCamera.Transform.Pos = Vector3.Zero;
            }

            g.DrawString("Press tab to generate again", ScreenTextFont, ScreenTextBrush, 0, ScreenSize.Height - 65);
        }

        private void SimulateObjectBounds(int size) {
            GameObject obj;
            foreach (var key in Engine.GetObjects.Keys) {
                obj = GetObjects[key];
                if (!obj.Dynamic) continue;
                if (obj.Transform.Pos.X >= size || obj.Transform.Pos.X <= -size) {
                    obj.Transform.PosX(obj.Transform.Pos.X > 1 ? size : -size);
                    obj.Vel = new Vector3(obj.Vel.X * -1 / 2f, obj.Vel.Y, obj.Vel.Z);
                }
                if (obj.Transform.Pos.Y <= 0f) {
                    obj.Transform.PosY(0);
                    obj.Vel = new Vector3(obj.Vel.X, Math.Abs(obj.Vel.Y / 2.2f), obj.Vel.Z);
                }
                if (obj.Transform.Pos.Z >= size || obj.Transform.Pos.Z <= -size) {
                    obj.Transform.PosZ(obj.Transform.Pos.Z > 1 ? size : -size);
                    obj.Vel = new Vector3(obj.Vel.X, obj.Vel.Y, obj.Vel.Z * -1 / 2f);
                }
            }
        }

        private void Generate(int objectCount, int size, Vector3 offset) {
            ClearRenderStack();
            Engine.GetObjects.Clear();
            
            for (int i = 1; Engine.GetObjects.Count < objectCount; i++) {
                Vector3 v = Vector3.Random(-size, size) + offset;
                int selected = _r.Next(0, size);
                GameObject rand = new GameObject(selected < size / 2 ? Mesh.Sphere : Mesh.Sphere) {
                    Col = Color.FromArgb(_r.Next(70, 255), _r.Next(70, 255), _r.Next(70, 255)),
                    Transform = new Transform() {
                        Pos = v
                    },
                    Mass = _r.Next(1, size),
                    Vel = new Vector3(_r.Next(-15, 15), _r.Next(5, 15), _r.Next(-15, 15))
                };
                base.Instantiate(rand, i);
            }
        }
    }
}






