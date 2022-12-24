using System;
using Vanilla3DEngine.Structs;

namespace Vanilla3DEngine.Classes {
    public class Transform {
        public Transform() {
            Pos = Vector3.Zero;
            Rot = Vector3.Zero;
        }
        public Vector3 Pos { get; set; }
        public Vector3 Rot { get; set; }

        public void PosX(float val) => Pos = new Vector3(val, Pos.Y, Pos.Z);
        public void PosY(float val) => Pos = new Vector3(Pos.X, val, Pos.Z);
        public void PosZ(float val) => Pos = new Vector3(Pos.X, Pos.Y, val);
        public void RotX(float val) => Rot = new Vector3(val, Rot.Y, Rot.Z);
        public void RotY(float val) => Rot = new Vector3(Rot.X, val, Rot.Z);
        public void RotZ(float val) => Rot = new Vector3(Rot.X, Rot.Y, val);

        public Vector3 Forward => Rot;
        public Vector3 Right => Vector3.Cross(Forward, Vector3.Up);
        public Vector3 Up => Vector3.Cross(Forward, Right);
    }
}
