using System;
using System.Collections.Generic;
using System.Drawing;
using Vanilla3DEngine.Structs;
using Vanilla3DEngine.Classes;

namespace Vanilla3DEngine {
    public class UsageExample : Engine {
        public UsageExample(Size screenSize, string title) : base(screenSize, title) {
        }

        private readonly Random _r = new Random();
        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private readonly GameObject _teapot = new GameObject(Mesh.LoadFromOBJFile(@"teapot.obj")) {
            Col = Color.LightCyan,
            Transform = new Transform() { Pos = new Vector3(0f, 0f, 15f) },
        };

        public override void Awake() {
            ShowDebugInfo = true;
            MainCamera.Transform.Pos = new Vector3(0f, 20f, -10f);
            _gameObjects.Add(_teapot);
        }

        public override void Update(Graphics g, float deltaTime) {
            base.HandleInput(10f, 2.5f);
            base.HandleObjects(_gameObjects);

            // pseudo-physics, real physics with collision later
            HandlePseudoPhysics(deltaTime);

            if (Input.GetKeyUp('\t')) {
                base.ShowMessageOnCenter(g, "Generating...");
                Generate(100, 5, new Vector3(0f, 0f, 50f));
            }
            if (Input.GetKeyDown('R')) {
                MainCamera.Transform.Pos = Vector3.Zero;
                _gameObjects.Clear();
            }
        }

        private void HandlePseudoPhysics(float deltaTime) {
            for (int i = _gameObjects.Count - 1; i >= 0; i--) {
                _gameObjects[i].Transform.Rot += new Vector3(1f, 0.5f, 1f) * deltaTime;
                if (_gameObjects[i].Transform.Pos.Y <= 0f) {
                    _gameObjects[i].Transform.Pos = new Vector3(_gameObjects[i].Transform.Pos.X, 0f, _gameObjects[i].Transform.Pos.Z);
                    _gameObjects[i].Vel = new Vector3(_gameObjects[i].Vel.X, Math.Abs(_gameObjects[i].Vel.Y / 2.2f), _gameObjects[i].Vel.Z);
                }
            }
        }

        private void Generate(int objectCount, int size, Vector3 offset) {
            _gameObjects.Clear();
            _teapot.Transform.Pos = offset;
            _gameObjects.Add(_teapot);
            ClearRenderStack();
            for (; _gameObjects.Count < objectCount;) {
                Vector3 v = Vector3.Random(-size, size);
                int selected = _r.Next(0, size);
                GameObject rand = new GameObject(selected < size / 2 ? Mesh.Cube : Mesh.Sphere) {
                    Col = Color.FromArgb(_r.Next(70, 255), _r.Next(70, 255), _r.Next(70, 255)),
                    Transform = new Transform() {
                        Rot = Vector3.Random(0, 360) * (float)Math.PI / 180f,
                        Pos = v + offset
                    },
                    Mass = _r.Next(1, 10),
                    Vel = new Vector3(_r.Next(-15, 15), _r.Next(15, 50), _r.Next(-15, 15))//Vector3.Random(-30, 30)
                };
                _gameObjects.Add(rand);
            }
        }
    }
}






