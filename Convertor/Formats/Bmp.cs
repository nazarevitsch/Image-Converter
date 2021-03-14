using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Converter
{
    public class Bmp : IImageFormat
    {
        public Dictionary<string, string> Headers { get; set; } = new();
        public byte[] Bytes { get; set; }
        public Pixel[] Pixels { get; set; }

        public Bmp(string path)
        {
            ReadData(path);
        }

        public Bmp()
        {
        }

        public void ReadData(string path)
        {
            byte[] array = File.ReadAllBytes(path);
            
            byte[] fileSizeBytes = {array[2], array[3], array[4], array[5]};
            int fileSize = BitConverter.ToInt32(fileSizeBytes, 0);
            
            byte[] sizeHeaderBytes = {array[14], array[15], array[16], array[17]};
            int headerSize = BitConverter.ToInt32(sizeHeaderBytes, 0);
            
            byte[] widthBytes = {array[18], array[19], array[20], array[21]};
            int width = BitConverter.ToInt32(widthBytes, 0);
            
            byte[] heightBytes = {array[22], array[23], array[24], array[25]};
            int height = BitConverter.ToInt32(heightBytes, 0);

            byte[] imageSizeBytes = {array[34], array[35], array[36], array[37]};
            int imageSize = BitConverter.ToInt32(imageSizeBytes, 0);
            
            Headers["Width"] = "" + width;
            Headers["Height"] = "" + height;
            Bytes = array.Skip(14 + headerSize).Take(imageSize).ToArray();
            Pixels = new Pixel[width * height];
            for (int bytePos = 0, pixel = 0; bytePos < Bytes.Length - 3; bytePos+=4, pixel++)
            {
                Pixels[pixel] = new Pixel
                {
                    Red = Bytes[bytePos + 2],
                    Green = Bytes[bytePos + 1],
                    Blue = Bytes[bytePos],
                    Alpha = Bytes[bytePos + 3]
                };
            }
        }
    }
}