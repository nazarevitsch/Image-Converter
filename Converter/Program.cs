using System;
using System.IO;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            PngReader pngReader = new PngReader();
            Image.Image dog = pngReader.ReadImage(@"D:\Work\ImageConvertor\Converter\resources\Big.png");
            
            IImageWriter writer = new PpmWriter();
            IImageWriter bmpWriter = new BmpWriter();
            writer.WriteImage(dog, @"D:\Work\ImageConvertor\Converter\resources\dog231.ppm");
            bmpWriter.WriteImage(dog, @"D:\Work\ImageConvertor\Converter\resources\dog231.bmp");
            
        }
    }
}