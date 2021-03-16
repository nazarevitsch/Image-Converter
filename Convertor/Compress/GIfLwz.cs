using System;
using System.Collections.Generic;
using Converter;

namespace Convertor.Compress
{
    public class GIfLwz
    {
        public Dictionary<int, Pixel> Codes { get; private set; }

        public Pixel ClearPixel { get; } = new()
        {
            Red = -1,
            Green = -1,
            Blue = -1
        };
        
        public Pixel EndPixel { get; } = new()
        {
            Red = -2,
            Green = -2,
            Blue = -2
        };
        
        private void InitTable <T> (T colors) where T : IList<Pixel>
        {
            if (Codes is not null) return;

            Codes = new Dictionary<int, Pixel>(10);
            
            for (var color = 0; color < colors.Count; color++)
            {
                Codes[color] = colors[color];
            }

            Codes[colors.Count] = ClearPixel;
            Codes[colors.Count + 1] = EndPixel;
        }
        
        public void Decode<T>(byte[] bytes, T colors, int minCodeSize) where T : IList<Pixel>
        {
            InitTable(colors);
            
            Convert.ToInt32(bytes[0]);
        }
    }
}