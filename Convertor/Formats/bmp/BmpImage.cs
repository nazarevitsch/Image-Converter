using System.Collections.Generic;
using Converter;

namespace Convertor.Formats.bmp
{
    public class BmpImage : IImageFormat
    {
        Pixel[] IImageFormat.Pixels { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
        public Pixel[][] _Pixels { get; set; }

        public Dictionary<string, string> Headers { get; set; }
        public byte[] Bytes { get; set; }
        public byte[] Pixels { get; set; }

        public override string ToString()
        {
            return "BMP: W: " + Width + ", H:" + Height + ", S: " + Pixels.Length;
        }
    }
}