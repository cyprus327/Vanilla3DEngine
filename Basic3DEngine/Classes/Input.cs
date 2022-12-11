using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Vanilla3DEngine {

    public class Input {
        [DllImport("user32.dll")] static extern int GetAsyncKeyState(int key);

        private static bool _pressed = false;

        public static bool GetKeyDown(int key) {
            return GetAsyncKeyState(key) >= 1;
        }

        public static bool GetKeyUp(int key) {
            if (GetAsyncKeyState(key) >= 1) {
                _pressed = true;
                return false;
            }
            if (GetAsyncKeyState(key) == 0 && _pressed) {
                _pressed = false;
                return true;
            }
            return false;
        }

        private void MouseDown(object sender, MouseEventArgs e) {
            Point mouseDownLocation = new Point(e.X, e.Y);

            
        }
    }
}
