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

        public void WriteImage(Image.Image image, String path)
        {
            byte[] widthBytes = BitConverter.GetBytes(image.Width);
            byte[] heightBytes = BitConverter.GetBytes(image.Height);
            byte[] imageSizeBytes = BitConverter.GetBytes(image.Width * image.Height * 4);
            byte[] fileSizeBytes = BitConverter.GetBytes(HeaderSize + HeaderInfoSize + (image.Width * image.Height * 4));
            byte[] pixelDataOffsetBytes = BitConverter.GetBytes(HeaderSize + HeaderInfoSize);
            
            byte[] imageBytes = new byte[HeaderSize + HeaderInfoSize + (image.Width * image.Height * 4)];

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
                int reversedI = image.Height - 1 - i;
                for (int l = 0; l < image.Pixels[i].Length; l++)
                {
                    imageBytes[i * image.Width * 4 + HeaderSize + HeaderInfoSize + l * 4] = image.Pixels[reversedI][l].B;
                    imageBytes[i * image.Width * 4 + HeaderSize + HeaderInfoSize + l * 4 + 1] = image.Pixels[reversedI][l].G;
                    imageBytes[i * image.Width * 4 + HeaderSize + HeaderInfoSize + l * 4 + 2] = image.Pixels[reversedI][l].R;
                    imageBytes[i * image.Width * 4 + HeaderSize + HeaderInfoSize + l * 4 + 3] = 0;
                } 
            }
            File.WriteAllBytes(path, imageBytes);
        }
    }
}