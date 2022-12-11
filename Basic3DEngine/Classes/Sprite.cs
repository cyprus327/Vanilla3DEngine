using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanilla3DEngine.Classes {
    public class Sprite : IDisposable {
        public Sprite(Bitmap sprite) {
            Bmp = sprite;
            Textures = new Color[Bmp.Width, Bmp.Height];
            for (int i = 0; i < Textures.GetLength(1); i++) {
                for (int j = 0; j < Textures.GetLength(0); j++) {
                    Textures[j, i] = Bmp.GetPixel(j, i);
                }
            }
        }

        private Color[,] Textures { get; } // some random memory thing and this being modified somehow is why the textures permanantly change
        public Bitmap Bmp { get; private set; }

        public void Dispose() {
            Bmp.Dispose();
        }

        public Color GetPixel(int x, int y) => Textures[x, y];
    }
}
