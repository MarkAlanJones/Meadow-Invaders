using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeadowInvaders
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        St7789 display;
        GraphicsLibrary graphics;
        const int displayWidth = 240;
        const int displayHeight = 240;

        int spriteW;
        int spriteH;

        List<SpriteWorker> workers = new List<SpriteWorker>();

        Random rand;

        Dictionary<int, BitArray> sprites = new Dictionary<int, BitArray>();

        public MeadowApp()
        {
            Stopwatch sw = new Stopwatch();
            Initialize();

            sw.Start();

            // load sprites
            for (int i = 1; i < 8; i++)
            {
                sw.Restart();
                sprites.Add(i, LoadAlienSprite(i));
                Console.WriteLine($"Load {i}: {sw.ElapsedMilliseconds}ms");
            }

            // the bitmaps are actually 2 frame animations except the ship
            workers.Add(new SpriteWorker(sprites[1], sprites[2], spriteW, spriteH, rand.Next(5, 25), RandColor(), display));
            workers.Add(new SpriteWorker(sprites[3], sprites[4], spriteW, spriteH, rand.Next(5, 25), RandColor(), display));
            workers.Add(new SpriteWorker(sprites[5], sprites[6], spriteW, spriteH, rand.Next(5, 25), RandColor(), display));
            workers.Add(new SpriteWorker(sprites[7], null, spriteW, spriteH, rand.Next(5, 25), RandColor(), display));

            // position
            foreach (var w in workers)
            {
                w.posx = -spriteW; // start offscreen
                w.posy = rand.Next((int)display.Height - spriteH);
            }

            sw.Restart();
            int frame = 0;
            while (true)
            {
                graphics.Clear();

                frame += 1;
                // get all the sprites to draw before updating the display
                foreach (var w in workers)
                {
                    w.Drawframe(frame);
                    if (w.done)
                    {
                        w.posx = -w.width;
                        w.color = RandColor();
                        w.speed = rand.Next(5, 25);
                        w.posy = rand.Next((int)display.Height - spriteH);
                    }
                }

                graphics.Show();

                Console.WriteLine($"{frame}) {frame * 1000.0 / sw.ElapsedMilliseconds:F} fps");
            }
        }

        void Initialize()
        {
            Console.WriteLine("Initializing...");

            var config = new SpiClockConfiguration(48000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: (uint)displayWidth, height: (uint)displayHeight);

            graphics = new GraphicsLibrary(display);

            graphics.Rotation = GraphicsLibrary.RotationType._270Degrees;
            graphics.Stroke = 1;

            rand = new Random();

            graphics.Clear();
        }

        void GridTest(int size)
        {
            graphics.Clear();

            for (int x = 0; x < display.Width; x += size)
            {
                for (int y = 0; y < display.Height; y += size)
                {
                    graphics.DrawRectangle(x, y, size, size, RandColor(), true);
                    // Console.WriteLine($"{x} {y}");
                    graphics.Show();
                }
            }
        }

        // lower level move rectangle bitmap to display bypassing graphics library
        void GridBitmapTest(int size)
        {
            byte[] rect = new byte[size * (int)Math.Ceiling((double)size / 8)];
            for (int r = 0; r < rect.Length; r++)
                rect[r] = (byte)rand.Next(255);

            graphics.Clear();

            for (int x = 0; x < display.Width; x += size)
            {
                for (int y = 0; y < display.Height; y += size)
                {
                    graphics.DrawBitmap(x, y, (int)Math.Ceiling((double)size / 8), size, rect, RandColor());
                    // Console.WriteLine($"{x} {y}");
                    graphics.Show();
                }
            }
        }

        void InvaderTest(int alien)
        {
            byte[] inv = BitmapLoader.LoadBMPResource($"Invaders.jooyetech_icon_0{alien}_invader.bmp");

            Console.WriteLine($"{inv.Length} bytes");

            //bmp start at lower left
            //flip vertically - and flip the bits in the byte ~
            // also reverse the bits for little endian ?
            byte[] flipinv = new byte[inv.Length];
            for (int r = 39; r > 0; r--)
                for (int i = 0; i < 8; i++)
                    flipinv[r * 8 + i] = Reverse((byte)~inv[(39 - r) * 8 + i]);

            graphics.Clear();

            for (int x = 0; x < display.Width; x += 64)
            {
                for (int y = 0; y < display.Height; y += 40)
                {
                    //display.DrawBitmap(x, y, 64 / 8, 40, flipinv, RandColor());
                    DrawBitmap(x, y, 64, 40, flipinv, RandColor());
                    // Console.WriteLine($"{x} {y}");
                    graphics.Show();
                }
            }
        }

        BitArray LoadAlienSprite(int alien)
        {
            string filename = $"Invaders.jooyetech_icon_0{alien}_invader.bmp";
            byte[] inv = BitmapLoader.LoadBMPResource(filename);

            Console.WriteLine($"{alien} has {inv.Length} bytes");

            //bmp start at lower left
            //flip vertically - and flip the bits in the byte ~
            // also reverse the bits for little endian ?
            byte[] flipinv = new byte[inv.Length];
            for (int r = 39; r > 0; r--)
                for (int i = 0; i < 8; i++)
                    flipinv[r * 8 + i] = Reverse((byte)~inv[(39 - r) * 8 + i]);

            // assume all are same size for now
            int[] d = BitmapLoader.LoadDimensions(filename);
            spriteW = d[0];
            spriteH = d[1];

            return new BitArray(flipinv);
        }

        void DrawBitmap(int x, int y, int width, int height, byte[] bmp, Color c)
        {
            BitArray ba = new BitArray(bmp);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (ba[i + width * j])
                        display.DrawPixel(x + i, y + j, c);
                }
            }
        }

        // Reverses bits in a byte
        public static byte Reverse(byte inByte)
        {
            byte result = 0x00;

            for (byte mask = 0x80; Convert.ToInt32(mask) > 0; mask >>= 1)
            {
                // shift right current result
                result = (byte)(result >> 1);

                // tempbyte = 1 if there is a 1 in the current position
                var tempbyte = (byte)(inByte & mask);
                if (tempbyte != 0x00)
                {
                    // Insert a 1 in the left
                    result = (byte)(result | 0x80);
                }
            }

            return (result);
        }

        Color RandColor()
        {
            return Color.FromRgb(rand.Next(255), rand.Next(255), rand.Next(255));
        }
    }
}


