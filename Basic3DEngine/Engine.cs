using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanilla3DEngine.Structs;
using Vanilla3DEngine.Classes;

namespace Vanilla3DEngine {
    public class Canvas : Form {
        public Canvas() {
            DoubleBuffered = true;
        }
    }

    public abstract class Engine {
        public Engine(Size screenSize, string title) {
            ScreenSize = screenSize;
            _title = title;
            _window = new Canvas { 
                Size = new Size(ScreenSize.Width, ScreenSize.Height), 
                StartPosition = FormStartPosition.CenterScreen, 
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
            };
            _window.Paint += Renderer;
            _mainThread = new Thread(MainLoop);
            _mainThread.SetApartmentState(ApartmentState.STA);

        }

        public static bool ShowDebugInfo = false;
        public static Brush ScreenTextBrush = new SolidBrush(Color.White);
        public static Font ScreenTextFont = new Font(FontFamily.GenericMonospace, 16f);
        public static Size ScreenSize = new Size();
        public static Camera MainCamera = new Camera(80f, 0.1f);
        public static Vector3 LightDirection = Vector3.Normalize(new Vector3(0.65f, 0.5f, -1f));
        public static Vector3 Gravity = new Vector3(0f, -9.81f, 0f);
        
        private float _delta = 0f;
        private readonly DeltaTime _deltaTime = new DeltaTime();

        private readonly TriangleDepthComparer _depthComparer = new TriangleDepthComparer();
        private readonly string _title = "";
        private readonly Canvas _window = null;
        private readonly Thread _mainThread = null;
        private static readonly List<Triangle> _renderStack = new List<Triangle>();
        private static readonly List<GameObject> _objects = new List<GameObject>(); // gets cleared and remade every frame
        private int _renderType = 2;
        private string _currentRenderType;
        private Mat4x4 _projectionMat = Mat4x4.New;
        private float _playerYaw = 0f;
        private Graphics _mainGraphics;

        public void Run() {
            _mainThread.Start();
            Application.Run(_window);
        }

        private void MainLoop() {
            Awake();
            _projectionMat = Mat4x4.MakeProjection(MainCamera.FOV, (float)ScreenSize.Height / (float)ScreenSize.Width, MainCamera.NearPlane, 1000f);

            while (true) {
                try {
                    _window.BeginInvoke((MethodInvoker)delegate { _window.Refresh(); });
                }
                catch { } // ignore exception that gets thrown once at the very beginning every time

                Thread.Sleep(1); // prevents jittering / unstable fps
            }
        }

        public abstract void Awake();

        public abstract void Update(Graphics g, float deltaTime);

        public void HandleObjects(List<GameObject> objects) {
            for (int i = objects.Count - 1; i >= 0; i--) {
                objects[i].Step(_delta, Gravity);
                DrawObject(objects[i], objects[i].Transform.Pos);
            }
        }

        private void Renderer(object sender, PaintEventArgs e) {
            _mainGraphics = e.Graphics;

            _mainGraphics.Clear(Color.FromArgb(37, 37, 37));

            _delta = _deltaTime.Get();

            Update(_mainGraphics, _delta);

            Render(_mainGraphics, ShowDebugInfo);
        }

        private void Render(Graphics g, bool showDebugInfo = true) {
            for (int i = _objects.Count - 1; i >= 0; i--) {
                for (int j = _objects[i].Mesh.Tris.Count - 1; j >= 0; j--) {
                    _renderStack.Add(_objects[i].Mesh.Tris[j]);
                }
            }

            _renderStack.Sort(_depthComparer);

            DrawTriangles(g, _renderStack);

            if (showDebugInfo) {
                g.DrawString($"Camera Pos:  {MainCamera.Transform.Pos}\n" +
                         $"Camera Rot:  {MainCamera.Transform.Rot}\n\n" +
                         $"Render Mode: {_currentRenderType}\n" +
                         $"Triangles:   {_renderStack.Count}\n" +
                         $"Meshes:      {_objects.Count}",
                         ScreenTextFont, ScreenTextBrush, 0, 0);
            }

            _window.Text = _title + " | FPS: " + (1000f / _delta / 1000f).ToString().Split('.')[0];

            ClearRenderStack();
        }

        private void DrawTriangles(Graphics g, List<Triangle> tris) {
            int count = tris.Count;
            for (int i = 0; i < count; i++) {
                DrawTriangle(g, tris[i]);
            }

            switch (_renderType) {
                case 1:
                    _currentRenderType = "Shaded Wireframe";
                    break;
                case 2:
                    _currentRenderType = "Shaded";
                    break;
                case 3:
                    _currentRenderType = "Unshaded Wireframe";
                    break;
                case 4:
                    _currentRenderType = "Unshaded";
                    break;
            }
        }

        private void DrawTriangle(Graphics g, Triangle tri) {
            switch (_renderType) {
                case 1:
                    tri.Draw(g, tri.Col, true);
                    tri.Draw(g, Color.Black, false);
                    break;
                case 2:
                    tri.Draw(g, tri.Col, true);
                    break;
                case 3:
                    tri.Draw(g, Color.White, true);
                    tri.Draw(g, Color.Black, false);
                    break;
                case 4:
                    tri.Draw(g, Color.White, true);
                    break;
            }
        }
        
        public void DrawObject(GameObject obj, Vector3 position) => 
            RasterizeTris(GetTrisToRaster(obj, position));

        public void ShowMessageOnCenter(Graphics g, string message) {
            if (g == null) return;
            g.DrawString(message, ScreenTextFont, ScreenTextBrush, 
                ScreenSize.Width / 2 - message.Length / 2 * ScreenTextFont.Size,
                ScreenSize.Height / 2);
        }
        
        public void HandleInput(float moveSpeed, float turnSpeed) {
            if (Input.GetKeyDown('V')) // sprint
                moveSpeed *= 3f;

            // move up
            if (Input.GetKeyDown(' '))
                MainCamera.Transform.Pos += Vector3.Up * moveSpeed * _delta;
            // move down
            else if (Input.GetKeyDown('C'))
                MainCamera.Transform.Pos -= Vector3.Up * moveSpeed * _delta;

            // turn right
            if (Input.GetKeyDown('E'))
                _playerYaw += turnSpeed * _delta;
            // turn left
            else if (Input.GetKeyDown('Q'))
                _playerYaw -= turnSpeed * _delta;

            Vector3 forward = MainCamera.Transform.Rot * (moveSpeed * _delta);
            // move forward
            if (Input.GetKeyDown('W'))
                MainCamera.Transform.Pos += forward;
            // move backward
            else if (Input.GetKeyDown('S'))
                MainCamera.Transform.Pos -= forward;

            Vector3 right = Vector3.CrossProduct(forward, Vector3.Up);
            // move backward
            if (Input.GetKeyDown('D'))
                MainCamera.Transform.Pos += right;
            // move left
            else if (Input.GetKeyDown('A'))
                MainCamera.Transform.Pos -= right;

            if (Input.GetKeyDown('1'))
                _renderType = 1;
            else if (Input.GetKeyDown('2'))
                _renderType = 2;
            else if (Input.GetKeyDown('3'))
                _renderType = 3;
            else if (Input.GetKeyDown('4'))
                _renderType = 4; 
        }

        private List<Triangle> GetTrisToRaster(GameObject obj, Vector3 position) {
            List<Triangle> output = new List<Triangle>();
            
            Mat4x4 transMat = Mat4x4.MakeTranslation(position.X, position.Y, position.Z);
            Mat4x4 worldMat = Mat4x4.IntrinsicFromEuler(obj.Transform.Rot);
            worldMat = Mat4x4.MultiplyMatrix(worldMat, transMat); // translate/combine into one worldMat

            Vector3 target = Vector3.Forward;
            MainCamera.Transform.Rot = Vector3.MatrixMultiplyVector(Mat4x4.MakeRotationY(_playerYaw), target);
            target = MainCamera.Transform.Pos + MainCamera.Transform.Rot;

            Mat4x4 viewMat = Mat4x4.Inverse(Mat4x4.PointAt(MainCamera.Transform.Pos, target, Vector3.Up));

            int len = obj.Mesh.Tris.Count;
            for (int i = 0; i < len; i++) {
                Triangle projected = new Triangle(new Vector3[3]);
                Triangle transformed = new Triangle(new Vector3[3]);
                Triangle viewed = new Triangle(new Vector3[3]);

                transformed.Points[0] = Vector3.MatrixMultiplyVector(worldMat, obj.Mesh.Tris[i].Points[0]);
                transformed.Points[1] = Vector3.MatrixMultiplyVector(worldMat, obj.Mesh.Tris[i].Points[1]);
                transformed.Points[2] = Vector3.MatrixMultiplyVector(worldMat, obj.Mesh.Tris[i].Points[2]);

                // get lines on either side of triangle
                Vector3 line1 = transformed.Points[1] - transformed.Points[0];
                Vector3 line2 = transformed.Points[2] - transformed.Points[0];

                // take cross product of lines to get normal to triangle surface
                Vector3 normal = Vector3.Normalize(Vector3.CrossProduct(line1, line2));

                Vector3 cameraRay = transformed.Points[0] - MainCamera.Transform.Pos;

                if (Vector3.Dot(normal, cameraRay) < 0f) {
                    // how aligned are the light direction and the surface normal
                    float dp = Math.Max(0.125f, Vector3.Dot(LightDirection, normal));
                    dp = Math.Min(dp, 1f);

                    // color based on dp
                    transformed.Col = Color.FromArgb(obj.Col.A, Math.Abs((int)(dp * obj.Col.R)), Math.Abs((int)(dp * obj.Col.G)), Math.Abs((int)(dp * obj.Col.B)));

                    // convert from world space to view space
                    viewed.Points[0] = Vector3.MatrixMultiplyVector(viewMat, transformed.Points[0]);
                    viewed.Points[1] = Vector3.MatrixMultiplyVector(viewMat, transformed.Points[1]);
                    viewed.Points[2] = Vector3.MatrixMultiplyVector(viewMat, transformed.Points[2]);
                    viewed.Col = transformed.Col;

                    // clip viewed triangles against near plane
                    Triangle[] clipped = new Triangle[2] { new Triangle(new Vector3[3]), new Triangle(new Vector3[3]) };
                    int clippedTris = Triangle.ClipAgainstPlane(new Vector3(0f, 0f, MainCamera.NearPlane), new Vector3(0f, 0f, 1f), ref viewed, ref clipped[0], ref clipped[1]);

                    // choose how to project the two clipped triangles
                    for (int j = 0; j < clippedTris; j++) {
                        // project from 3d to 2d
                        projected.Points[0] = Vector3.MatrixMultiplyVector(_projectionMat, clipped[j].Points[0]);
                        projected.Points[1] = Vector3.MatrixMultiplyVector(_projectionMat, clipped[j].Points[1]);
                        projected.Points[2] = Vector3.MatrixMultiplyVector(_projectionMat, clipped[j].Points[2]);
                        projected.Col = clipped[j].Col;

                        // scale into view by normalising into cartesian space
                        projected.Points[0] /= projected.Points[0].W;
                        projected.Points[1] /= projected.Points[1].W;
                        projected.Points[2] /= projected.Points[2].W;

                        // x and y are inverted somehow so invert them again to make them normal
                        projected.Points[0].X *= -1f;
                        projected.Points[0].Y *= -1f;
                        projected.Points[1].X *= -1f;
                        projected.Points[1].Y *= -1f;
                        projected.Points[2].X *= -1f;
                        projected.Points[2].Y *= -1f;

                        // scale to screen
                        Vector3 viewOffset = new Vector3(1f, 1f, 0f);
                        projected.Points[0] += viewOffset;
                        projected.Points[1] += viewOffset;
                        projected.Points[2] += viewOffset;

                        projected.Points[0].X *= 0.5f * (float)ScreenSize.Width;
                        projected.Points[0].Y *= 0.5f * (float)ScreenSize.Height;
                        projected.Points[1].X *= 0.5f * (float)ScreenSize.Width;
                        projected.Points[1].Y *= 0.5f * (float)ScreenSize.Height;
                        projected.Points[2].X *= 0.5f * (float)ScreenSize.Width;
                        projected.Points[2].Y *= 0.5f * (float)ScreenSize.Height;

                        output.Add(projected);
                    }
                }
            }
            return output;
        }

        private void RasterizeTris(List<Triangle> trisToRaster) {
            if (trisToRaster.Count == 0) return;

            List<Triangle> output = new List<Triangle>();
            Triangle[] clipped = new Triangle[2] { new Triangle(), new Triangle() };

            foreach (Triangle tri in trisToRaster) {
                Triangle test = tri;

                // clip the triangle against each of the four clipping planes
                for (int p = 0; p < 4; p++) {
                    Vector3 planeNormal = Vector3.Zero;
                    Vector3 planePoint = Vector3.Zero;

                    switch (p) {
                        case 0:
                            planeNormal = Vector3.Up;
                            break;
                        case 1:
                            planeNormal = Vector3.Down;
                            planePoint = new Vector3(0f, ScreenSize.Height - 1f, 0f);
                            break;
                        case 2:
                            planeNormal = Vector3.Right;
                            break;
                        case 3:
                            planeNormal = Vector3.Left;
                            planePoint = new Vector3(ScreenSize.Width - 1f, 0f, 0f);
                            break;
                    }

                    int trisToAdd = Triangle.ClipAgainstPlane(planeNormal, planePoint, ref test, ref clipped[0], ref clipped[1]);

                    if (trisToAdd == 0) {
                        // the triangle is completely outside the clipping plane, so discard it
                        break;
                    }
                    else if (trisToAdd == 1) {
                        // the triangle was partially inside and partially outside the clipping plane,
                        // so replace the input triangle with the newly created triangle
                        test = clipped[0];
                    }
                    else {
                        // the triangle was split into two new triangles by the clipping plane,
                        // so add both triangles to the list of triangles to process
                        output.Add(clipped[0]);
                        test = clipped[1];
                    }
                }

                // if the triangle survived all four clipping planes, add it to the final list of triangles
                if (test.Points[0] != Vector3.Zero || test.Points[1] != Vector3.Zero || test.Points[2] != Vector3.Zero)
                    output.Add(test);
            }

            _objects.Add(new GameObject(new Mesh() { Tris = output }));
        }

        public static void ClearRenderStack() {
            _renderStack.Clear();
            _objects.Clear();
        }
    }
}
