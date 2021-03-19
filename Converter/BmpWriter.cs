using System;
using System.IO;
using Converter.Image;

namespace Converter
{
    public class BmpWriter : IImageWriter
    {
        private static int HeaderSize = 14;
        private static int HeaderInfoSize = 40;
        private static byte[] HeaderSizeBytse = BitConverter.GetBytes(HeaderInfoSize);
        
        public void WriteImage(BmpImage image, String path)
        {
            byte[] widthBytes = BitConverter.GetBytes(image.Width);
            byte[] heightBytes = BitConverter.GetBytes(image.Height);
            byte[] imageSizeBytes = BitConverter.GetBytes(image.Pixels.Length);
            byte[] fileSizeBytes = BitConverter.GetBytes(HeaderSize + HeaderInfoSize + image.Pixels.Length);
            
            byte[] imageBytes = new byte[HeaderSize + HeaderInfoSize + image.Pixels.Length];

            byte[] pixelDataOffsetBytes = BitConverter.GetBytes(HeaderSize + HeaderInfoSize);

            imageBytes[0] = 66;
            imageBytes[1] = 77;
            imageBytes[26] = 1;
            imageBytes[28] = 32;
            for (int i = 0; i < fileSizeBytes.Length; i++)
            {
                imageBytes[i + 2] = fileSizeBytes[i];
            }
            
            for (int i = 0; i <pixelDataOffsetBytes.Length; i++)
            {
                imageBytes[i + 10] = pixelDataOffsetBytes[i];
            }
            
            for (int i = 0; i < HeaderSizeBytse.Length; i++)
            {
                imageBytes[i + 14] = HeaderSizeBytse[i];
            }
            
            for (int i = 0; i < widthBytes.Length; i++)
            {
                imageBytes[i + 18] = widthBytes[i];
            }
            
            for (int i = 0; i < heightBytes.Length; i++)
            {
                imageBytes[i + 22] = heightBytes[i];
            }
            
            for (int i = 0; i < imageSizeBytes.Length; i++)
            {
                imageBytes[i + 34] = imageSizeBytes[i];
            }
            
            for (int i = 0; i < image.Pixels.Length; i++)
            {
                imageBytes[i + HeaderSize + HeaderInfoSize] = image.Pixels[i];
            }
            File.WriteAllBytes(path, imageBytes);
        }
        
       
    }
}