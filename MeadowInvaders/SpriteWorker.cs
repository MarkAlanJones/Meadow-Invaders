using Meadow;
using Meadow.Foundation.Displays;
using System.Collections;

namespace MeadowInvaders
{
    /// <summary>
    /// Encapsulate a Sprite that can flip between two images 
    /// Move it across the screen
    /// and can synchronize with others to update the display
    /// Writes to display dorectly avoiding graphicsLibrary
    /// </summary>
    public class SpriteWorker
    {
        public int Speed { get; set; }
        public Color Color { get; set; }

        public int Posx { get; set; }
        public int Posy { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private BitArray S1 { get; set; }
        private BitArray S2 { get; set; }
        private TftSpiBase Display { get; set; }

        public bool Done { get { return Posx > Display.Width; } }

        public SpriteWorker(BitArray s1, BitArray s2, int width, int height, int speed, Color color, TftSpiBase display)
        {
            S1 = s1;
            S2 = s2;
            Speed = speed;
            Color = color;
            Width = width;
            Height = height;
            Posx = 0;
            Posy = 0;
            Display = display;
        }

        // Update the display
        public void Drawframe(int frame)
        {
            BitArray ba = GetBA(frame);
            Draw(ba);

            // move ?
            if (S2 == null || !IsOdd(frame))
                Posx += Speed;
        }

        private BitArray GetBA(int frame)
        {
            BitArray ba = S1;
            if (S2 != null && IsOdd(frame))
                ba = S2;
            return ba;
        }

        private bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        private void Draw(BitArray ba)
        {
            DrawSprite(Posx, Posy, Width, Height, ba, Color);
        }

        // Display wraps now, add edge detect
        private void DrawSprite(int x, int y, int width, int height, BitArray sprite, Color c)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (sprite[i + width * j] && (x + i) >= 0 && (x + i) < Display.Width)
                        Display.DrawPixel(x + i, y + j, c);
                }
            }
        }
    }
}
