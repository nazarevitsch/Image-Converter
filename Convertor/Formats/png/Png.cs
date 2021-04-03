using System.Collections.Generic;

namespace Convertor.Formats.png
{
    public class Png : IImageFormat
    {
        public Dictionary<string, string> Headers { get; set; }
        public byte[] Bytes { get; set; }
        public Pixel[] Pixels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Pixel[][] _Pixels { get; set; }
    }
}