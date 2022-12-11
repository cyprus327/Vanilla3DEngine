using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes {
    public struct Mesh { // mesh is pretty much GameObject
        public Mesh(List<Triangle> tris = null) {
            if (tris != null)
                Tris = tris;
            else
                Tris = new List<Triangle>();
        }

        public List<Triangle> Tris { get; set; }

        public static Mesh New => new Mesh() { Tris = new List<Triangle>() };

        public static Mesh LoadFromOBJFile(string filename) {
            Mesh mesh = New;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader stream = new StreamReader(fs);
            if (!stream.BaseStream.CanRead) return mesh;

            List<Vector3> verts = new List<Vector3>();

            while (!stream.EndOfStream) {
                string line = stream.ReadLine();

                if (line == "") continue;

                if (line[0] == 'v') {
                    if (line[1] != ' ') continue;
                    Vector3 v = Vector3.Zero;
                    string[] info = line.Split(' ');
                    v.X = (float)Convert.ToDouble(info[1]);
                    v.Y = (float)Convert.ToDouble(info[2]);
                    v.Z = (float)Convert.ToDouble(info[3]);
                    verts.Add(v);
                }
                else if (line[0] == 'f') {
                    string[] info = line.Split(' ');
                    int[] f;
                    if (info[1].Contains('/')) {
                        f = new int[info.Length];
                        for (int i = 1; i < info.Length; i++) {
                            if (info[i] == "\n") break;
                            f[i - 1] = Convert.ToInt32(info[i].Split('/')[0]);
                        }
                    }
                    else {
                        f = new int[3];
                        f[0] = Convert.ToInt32(info[1]);
                        f[1] = Convert.ToInt32(info[2]);
                        f[2] = Convert.ToInt32(info[3]);
                    }
                    if (verts[f[0] - 1] != null && verts[f[1] - 1] != null && verts[f[2] - 1] != null)
                        mesh.Tris.Add(new Triangle() { Points = new Vector3[3] { verts[f[0] - 1], verts[f[1] - 1], verts[f[2] - 1] } });
                }
            }

            return mesh;
        }

        public static Mesh Cube => new Mesh {
            Tris = new List<Triangle> {
                //south
                new Triangle(new Vector3[3] { new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(1f, 1f, 0f) }),
                new Triangle(new Vector3[3] { new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0f), new Vector3(1f, 0f, 0f) }),

                // east                                                                                                   
                new Triangle(new Vector3[3] { new Vector3(1f, 0f, 0f), new Vector3(1f, 1f, 0f), new Vector3(1f, 1f, 1f) }),
                new Triangle(new Vector3[3] { new Vector3(1f, 0f, 0f), new Vector3(1f, 1f, 1f), new Vector3(1f, 0f, 1f) }),

                // north                                                                                                                                         
                new Triangle(new Vector3[3] { new Vector3(1f, 0f, 1f), new Vector3(1f, 1f, 1f), new Vector3(0f, 1f, 1f) }),
                new Triangle(new Vector3[3] { new Vector3(1f, 0f, 1f), new Vector3(0f, 1f, 1f), new Vector3(0f, 0f, 1f) }),
                                                                                                                         
                // west                                                                                                                           
                new Triangle(new Vector3[3] { new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 1f), new Vector3(0f, 1f, 0f) }),
                new Triangle(new Vector3[3] { new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f) }),
                                                                                                                         
                // top                                                                                                                            
                new Triangle(new Vector3[3] { new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 1f), new Vector3(1f, 1f, 1f) }),
                new Triangle(new Vector3[3] { new Vector3(0f, 1f, 0f), new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 0f) }),
                                                                                                                         
                // bottom                                                                                                                      
                new Triangle(new Vector3[3] { new Vector3(1f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f) }),
                new Triangle(new Vector3[3] { new Vector3(1f, 0f, 1f), new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f) })
            }
        };


        public static Mesh Plane => new Mesh {
            Tris = new List<Triangle> {
                // two triangles that make up the top face of a cube
                new Triangle() { Points = new Vector3[3] { new Vector3(1f, 0f, 1f), new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, 0f) } },
                new Triangle() { Points = new Vector3[3] { new Vector3(1f, 0f, 1f), new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f) } }
            }
        };

        public static Mesh Sphere => LoadFromOBJFile(@"sphereLowPoly.obj");

        public static Mesh SphereHighPoly => LoadFromOBJFile(@"sphereHighPoly.obj");
    }
}
