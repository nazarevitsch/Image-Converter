using System;
using System.IO;

namespace Converter
{
    public class BmpWriter : IFormatWriter
    {
        
        private static int HeaderSize = 14;
        private static int HeaderInfoSize = 40;
        private static byte[] HeaderSizeBytse = BitConverter.GetBytes(HeaderInfoSize);
        public void Write(IImageFormat image)
        {
            byte[] widthBytes = BitConverter.GetBytes(int.Parse(image.Headers["Width"]));
            byte[] heightBytes = BitConverter.GetBytes(int.Parse(image.Headers["Height"]));
            byte[] imageSizeBytes = BitConverter.GetBytes(image.Bytes.Length);
            byte[] fileSizeBytes = BitConverter.GetBytes(HeaderSize + HeaderInfoSize + image.Bytes.Length);
            
            byte[] imageBytes = new byte[HeaderSize + HeaderInfoSize + image.Bytes.Length];

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
            
            for (int i = 0; i < image.Bytes.Length; i++)
            {
                imageBytes[i + HeaderSize + HeaderInfoSize] = image.Bytes[i];
            }
            File.WriteAllBytes("test.bmp", imageBytes);
        }
    }
}