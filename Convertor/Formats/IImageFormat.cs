using System.Collections;
using System.Collections.Generic;

namespace Converter
{
    public interface IImageFormat
    {
        public Dictionary<string, string> Headers { get; set; }
        
        public byte[] Bytes { get; set; }

        public Pixel[] Pixels { get; set; }
    }
}