using System;
using System.IO;
using System.Linq;
using Converter.Image;

namespace Converter
{
    public class BmpReader : IImageReader
    {
        public Image.Image ReadImage(String fileName)
        {
            byte[] array = File.ReadAllBytes(fileName);
            
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
            
            Image.Image image = new Image.Image();
            image.Width = width;
            image.Height = height;
            Pixel[][] pixels = new Pixel[height][];
            for (int i = 0; i < height; i++)
            {
                int rowStart = 14 + headerSize + (image.Height - 1 - i) * width * 4;
                pixels[i] = new Pixel[width];
                for (int l = 0; l < width; l++)
                {
                    pixels[i][l] = new Pixel(array[rowStart + l * 4], 
                        array[rowStart + l * 4 + 1],
                        array[rowStart + l * 4 + 2]);
                }
            }
            image.Pixels = pixels;
            image.PrintPixels();
            return image;
        }
    }
}