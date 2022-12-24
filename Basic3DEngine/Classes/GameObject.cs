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
            BB = BoundingBox.GetBoundsOf(mesh);
            Mass = 0.1f;
            Force = Vector3.Zero;
            Vel = Vector3.Zero;
            Transform = new Transform() { Pos = Vector3.Zero, Rot = Vector3.Zero };
            Col = Color.White;
            Dynamic = true;
            Center = Mesh.GetCenter();
        }

        public float Mass { get; set; }
        public Vector3 Force { get; set; }
        public Vector3 Vel { get; set; }
        public Color Col { get; set; }
        public Mesh Mesh { get; set; }
        public Vector3[] Axes { get; set; }
        public BoundingBox BB { get; private set; }
        private Vector3 _center = Vector3.Zero;
        public Vector3 Center {
            get { 
                return _center + Transform.Pos; 
            }
            private set {
                _center = value;
            }
        }
        public Transform Transform { get; set; }
        public bool Dynamic { get; set; }


        public List<Vector3> GetVertices() {
            List<Vector3> output = new List<Vector3>();
            foreach (Triangle tri in Mesh.Tris) {
                foreach (Vector3 vert in tri.Verts) {
                    output.Add(vert + Transform.Pos);
                }
            }
            return output;
        }

        public virtual void Step(float deltaTime, Vector3 gravity, float friction = 1.0025f) {
            if (!Dynamic) return;

            Force = gravity * Mass;
            Vel += Force / Mass * deltaTime;
            Vel = new Vector3(Vel.X / friction, Vel.Y / 1.00001f, Vel.Z / friction);
            Transform.Pos += Vel * deltaTime;
        }

        public static void HandleCollisions() {
            foreach (GameObject obj in Engine.GetObjects.Values) {
                foreach (GameObject other in Engine.GetObjects.Values) {
                    if (obj != other && BoundingBox.CheckCollision(obj.BB, other.BB) && GJK(obj, other)) 
                        RespondToCollision(obj, other);
                }
            }
        }

        private static void RespondToCollision(GameObject a, GameObject b) {
            // vector pointing from a to b
            Vector3 ab = b.Center - a.Center;
            ab = Vector3.Normalize(ab);
            //ab /= 5f; how to make it like water

            // apply collisions
            if (a.Dynamic) ApplyLinearCollisionImpulse(a, ab * -1, 0.5f);
            if (b.Dynamic) ApplyLinearCollisionImpulse(b, ab, 0.5f);
        }

        private static void ApplyLinearCollisionImpulse(GameObject contact, Vector3 n, float cor) {
            float d = Vector3.Dot(contact.Vel, n);
            float j = Math.Max(-(1 + cor) * d, 0.15f); // the 0.15 causes the jitter but when it's 0 the objects slowly melt into one another
            contact.Vel += n * j;
        }

        // BLOG.WINTER.DEV !!!
        #region GJK
        private static readonly List<Vector3> _simplex = new List<Vector3>(); // not local to CheckCollision tostop having to make a new one each frame
        public static bool GJK(GameObject a, GameObject b) {
            _simplex.Clear();
            Vector3 support = Support(a, b, Vector3.Right);
            _simplex.Add(support);

            // new direction pointing towards the origin
            Vector3 dir = support * -1;

            for (int i = 0; i < 30; i++) {
                support = Support(a, b, dir);
                // no collision
                if (Vector3.Dot(support, dir) <= 0) return false;

                _simplex.Add(support);

                if (NextSimplex(out dir)) return true;
            }
            return false;
        }

        private Vector3 FindFarthestPoint(Vector3 direction) {
            Vector3 farthest = Vector3.Zero;
            float farthestDist = -float.MaxValue, dist;
            foreach (Triangle tri in this.Mesh.Tris) {
                foreach (Vector3 vert in tri.Verts) {
                    dist = Vector3.Dot(vert + this.Transform.Pos, direction);
                    if (dist > farthestDist) {
                        farthestDist = dist;
                        farthest = vert + this.Transform.Pos;
                    }
                }
            }
            return farthest;
        }

        private static Vector3 Support(GameObject a, GameObject b, Vector3 direction) {
            return a.FindFarthestPoint(direction) - b.FindFarthestPoint(direction * -1);
        }

        private static bool NextSimplex(out Vector3 direction) {
            switch (_simplex.Count) {
                case 2: return Line(out direction);
                case 3: return Triangle(out direction);
                case 4: return Tetrahedron(out direction);
            }
            // wont ever get to this point
            direction = Vector3.Zero;
            return false;
        }

        private static bool Line(out Vector3 direction) {
            Vector3 a = _simplex[0], b = _simplex[1];
            Vector3 ab = b - a, ao = a * -1;

            if (Vector3.Dot(ab, ao) > 0) {
                direction = Vector3.Cross(Vector3.Cross(ab, ao), ab);
            }
            else {
                _simplex.Clear();
                _simplex.Add(a);
                direction = ao;
            }
            return false;
        }
        private static bool Triangle(out Vector3 direction) {
            Vector3 a = _simplex[0], b = _simplex[1], c = _simplex[2];
            Vector3 ab = b - a, ac = c - a, ao = a * -1;
            Vector3 abc = Vector3.Cross(ab, ac);

            if (Vector3.Dot(Vector3.Cross(abc, ac), ao) > 0) {
                _simplex.Clear();
                if (Vector3.Dot(ac, ao) > 0) {
                    _simplex.Add(a);
                    _simplex.Add(c);
                    direction = Vector3.Cross(Vector3.Cross(ac, ao), ac);
                }
                else {
                    _simplex.Add(a);
                    _simplex.Add(b);
                    return Line(out direction);
                }
            }
            else {
                if (Vector3.Dot(Vector3.Cross(ab, abc), ao) > 0) {
                    _simplex.Clear();
                    _simplex.Add(a);
                    _simplex.Add(b);
                    return Line(out direction);
                }
                else {
                    if (Vector3.Dot(abc, ao) > 0) {
                        direction = abc;
                    }
                    else {
                        _simplex.Clear();
                        _simplex.Add(a);
                        _simplex.Add(b);
                        _simplex.Add(c);
                        direction = abc * -1;
                    }
                }
            }
            return false;
        }
        private static bool Tetrahedron(out Vector3 direction) {
            Vector3 a = _simplex[0], b = _simplex[1], c = _simplex[2], d = _simplex[3];
            Vector3 ab = b - a, ac = c - a, ad = d - a, ao = a * -1;
            Vector3 abc = Vector3.Cross(ab, ac);
            Vector3 acd = Vector3.Cross(ac, ad);
            Vector3 adb = Vector3.Cross(ad, ab);

            if (Vector3.Dot(abc, ao) > 0) {
                _simplex.Clear();
                _simplex.Add(a);
                _simplex.Add(b);
                _simplex.Add(c);
                return Triangle(out direction);
            }
            if (Vector3.Dot(acd, ao) > 0) {
                _simplex.Clear();
                _simplex.Add(a);
                _simplex.Add(c);
                _simplex.Add(d);
                return Triangle(out direction);
            }
            if (Vector3.Dot(adb, ao) > 0) {
                _simplex.Clear();
                _simplex.Add(a);
                _simplex.Add(d);
                _simplex.Add(b);
                return Triangle(out direction);
            }
            direction = Vector3.Zero;
            return true;
        }
        #endregion GJK
    }
}
