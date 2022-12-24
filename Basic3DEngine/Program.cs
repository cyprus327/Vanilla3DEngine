using System.Drawing;

namespace Vanilla3DEngine {
    class Program {
        static void Main() {
            Engine game = new PrimitivesExplosionExample(new Size(1280, 720), "window title");
            game.Run();
        }
    }
}
