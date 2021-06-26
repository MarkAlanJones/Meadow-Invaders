using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using System.Collections;

namespace MeadowInvaders
{

    /// <summary>
    /// Encapsulate a Sprite that can flip between two images 
    /// Move it across the screen
    /// and can synchronize with others to update the display
    /// </summary>
    public class SpriteWorker
    {
        public int speed { get; set; }
        public Color color { get; set; }

        public int posx { get; set; }
        public int posy { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        private BitArray S1 { get; set; }
        private BitArray S2 { get; set; }
        private TftSpiBase display { get; set; }

        public bool done { get { return posx > display.Width; } }

        public SpriteWorker(BitArray s1, BitArray s2, int width, int height, int speed, Color color, TftSpiBase display)
        {
            S1 = s1;
            S2 = s2;
            this.speed = speed;
            this.color = color;
            this.width = width;
            this.height = height;
            posx = 0;
            posy = 0;
            this.display = display;
        }

        // Update the display
        public void Drawframe(int frame)
        {
            BitArray ba = getBA(frame);
            Draw(ba);

            // move ?
            if (S2 == null || !IsOdd(frame))
                posx += speed;
        }

        private BitArray getBA(int frame)
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
            DrawSprite(posx, posy, width, height, ba, color);
        }

        private void DrawSprite(int x, int y, int width, int height, BitArray sprite, Color c)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (sprite[i + width * j])
                        display.DrawPixel(x + i, y + j, c);
                }
            }
        }
    }
}
