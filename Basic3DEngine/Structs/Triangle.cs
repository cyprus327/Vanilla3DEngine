using System;
using System.Drawing;
using Vanilla3DEngine.Classes;

namespace Vanilla3DEngine.Structs {
    public struct Triangle {
        public Triangle(Vector3[] points) {
            if (points.Length != 3) throw new ArgumentOutOfRangeException("Triangles only have 3 sides.");
            Verts = points;
            Col = default;
        }

        public Vector3[] Verts { get; set; }
        public Color Col { get; set; }

        public void Draw(Graphics g, Color col, bool fill) {
            Col = col;
            if (!fill) {
                using (Pen p = new Pen(Col)) {
                    g.DrawLine(p, Verts[0].X, Verts[0].Y, Verts[1].X, Verts[1].Y);
                    g.DrawLine(p, Verts[1].X, Verts[1].Y, Verts[2].X, Verts[2].Y);
                    g.DrawLine(p, Verts[2].X, Verts[2].Y, Verts[0].X, Verts[0].Y);
                }
            }
            else {
                using (SolidBrush b = new SolidBrush(Col)) {
                    g.FillPolygon(b, new PointF[] { 
                        new PointF(Verts[0].X, Verts[0].Y),
                        new PointF(Verts[1].X, Verts[1].Y),
                        new PointF(Verts[2].X, Verts[2].Y)
                    });
                }
            }
        }

        public static int ClipAgainstPlane(Vector3 planeP, Vector3 planeN, ref Triangle tri, ref Triangle outTri1, ref Triangle outTri2) {
            planeN = Vector3.Normalize(planeN);

            // Return signed shortest distance from point to plane, plane normal must be normalised
            float Dist(Vector3 p) {
                return (planeN.X * p.X + planeN.Y * p.Y + planeN.Z * p.Z - Vector3.Dot(planeN, planeP));
            }

            Vector3[] insidePoints = new Vector3[3]; 
            int insidePointCount = 0;
            Vector3[] outsidePoints = new Vector3[3]; 
            int outsidePointCount = 0;

            float d0 = Dist(tri.Verts[0]);
            float d1 = Dist(tri.Verts[1]);
            float d2 = Dist(tri.Verts[2]);

            if (d0 >= 0) { insidePoints[insidePointCount++] = tri.Verts[0]; }
            else {
                outsidePoints[outsidePointCount++] = tri.Verts[0];
            }
            if (d1 >= 0) {
                insidePoints[insidePointCount++] = tri.Verts[1];
            }
            else {
                outsidePoints[outsidePointCount++] = tri.Verts[1];
            }
            if (d2 >= 0) {
                insidePoints[insidePointCount++] = tri.Verts[2];
            }
            else {
                outsidePoints[outsidePointCount++] = tri.Verts[2];
            }

            if (insidePointCount == 0 || outsidePointCount== 3) {
                return 0;
            }

            if (insidePointCount == 3) {
                outTri1 = tri;

                return 1;
            }

            if (insidePointCount == 1 && outsidePointCount == 2) {
                outTri1.Col = tri.Col;

                outTri1.Verts[0] = insidePoints[0];
                outTri1.Verts[1] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);
                outTri1.Verts[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[1]);
                return 1;
            }

            if (insidePointCount == 2 && outsidePointCount == 1) {
                outTri1.Col = tri.Col;

                outTri2.Col = tri.Col;

                outTri1.Verts[0] = insidePoints[0];
                outTri1.Verts[1] = insidePoints[1];
                outTri1.Verts[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);

                outTri2.Verts[0] = insidePoints[1];
                outTri2.Verts[1] = outTri1.Verts[2];
                outTri2.Verts[2] = Vector3.IntersectPlane(planeP, planeN, insidePoints[1], outsidePoints[0]);

                return 2;
            }

            return -1;
        }
    }
}
