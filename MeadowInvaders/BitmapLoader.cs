using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MeadowInvaders
{
    /// <summary>
    /// Load a 1bit BMP format file from embedded resource
    /// </summary>
    public static class BitmapLoader
    {
        enum BitmapCompressionMode : uint
        {
            BI_RGB = 0,
            BI_RLE8 = 1,
            BI_RLE4 = 2,
            BI_BITFIELDS = 3,
            BI_JPEG = 4,
            BI_PNG = 5
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public uint bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public uint bfOffBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public BitmapCompressionMode biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        }

        // Must be an "Embedded Resource"
        // throws away the header and returns the byte array - 
        public static byte[] LoadBMPResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MeadowInvaders.{filename}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using BinaryReader reader = new BinaryReader(stream);

            var fh = new BITMAPFILEHEADER()
            {
                bfType = reader.ReadUInt16(),
                bfSize = reader.ReadUInt32(),
                bfReserved1 = reader.ReadUInt16(),
                bfReserved2 = reader.ReadUInt16(),
                bfOffBits = reader.ReadUInt32()
            };

            var ih = new BITMAPINFOHEADER()
            {
                biSize = reader.ReadUInt32(),
                biWidth = reader.ReadInt32(),
                biHeight = reader.ReadInt32(),
                biPlanes = reader.ReadUInt16(),
                biBitCount = reader.ReadUInt16(),
                biCompression = (BitmapCompressionMode)reader.ReadUInt32(),
                biSizeImage = reader.ReadUInt32(),
                biXPelsPerMeter = reader.ReadInt32(),
                biYPelsPerMeter = reader.ReadInt32(),
                biClrUsed = reader.ReadUInt32(),
                biClrImportant = reader.ReadUInt32()
            };

            if (ih.biCompression != BitmapCompressionMode.BI_RGB)
                Console.WriteLine($"WARN not RGB {ih.biCompression}");

            Console.WriteLine($"{ih.biWidth} x {ih.biHeight}  {ih.biBitCount} bits {ih.biSizeImage} bytes: start {fh.bfOffBits}");

            stream.Position = 0;
            var header = reader.ReadBytes((int)fh.bfOffBits);

            return reader.ReadBytes(ih.biWidth / 8 * ih.biHeight);
        }

        public static int[] LoadDimensions(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MeadowInvaders.{filename}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using BinaryReader reader = new BinaryReader(stream);

            var fh = new BITMAPFILEHEADER()
            {
                bfType = reader.ReadUInt16(),
                bfSize = reader.ReadUInt32(),
                bfReserved1 = reader.ReadUInt16(),
                bfReserved2 = reader.ReadUInt16(),
                bfOffBits = reader.ReadUInt32()
            };

            var ih = new BITMAPINFOHEADER()
            {
                biSize = reader.ReadUInt32(),
                biWidth = reader.ReadInt32(),
                biHeight = reader.ReadInt32(),
                biPlanes = reader.ReadUInt16(),
                biBitCount = reader.ReadUInt16(),
                biCompression = (BitmapCompressionMode)reader.ReadUInt32(),
                biSizeImage = reader.ReadUInt32(),
                biXPelsPerMeter = reader.ReadInt32(),
                biYPelsPerMeter = reader.ReadInt32(),
                biClrUsed = reader.ReadUInt32(),
                biClrImportant = reader.ReadUInt32()
            };

            return new int[] { ih.biWidth, ih.biHeight };
        }
    }
}