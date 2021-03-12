using System;
using System.IO;
using System.Linq;
using Converter.Image;

namespace Converter
{
    public class BmpReader : IImageReader
    {
        public BmpImage ReadImage(String fileName)
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
            
            BmpImage image = new BmpImage();
            image.Width = width;
            image.Height = height;
            image.Pixels = array.Skip(14 + headerSize).Take(imageSize).ToArray();
            
            // Console.WriteLine("F.S.: " + fileSize);
            // Console.WriteLine("H.S.: " + headerSize);
            // Console.WriteLine("W.: " + width);
            // Console.WriteLine("H.: " + height);
            // Console.WriteLine("I.S.: " + imageSize);
            // for (int i = 0; i < array.Length; i++)
            // {
            //     Console.WriteLine(i + ": " + array[i]);
            // }
            return image;
        }
    }
}