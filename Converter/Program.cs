using System;
using Converter.Image;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            // BmpReader bmpReader = new BmpReader();
            // BmpImage dog1 = bmpReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.bmp");
            // Console.WriteLine(dog1);
            // BmpImage dog2 = bmpReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog2.bmp");
            // Console.WriteLine(dog2);

            PngReader pngReader = new PngReader();
            Image.Image png = pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.png");
            BmpWriter writer = new BmpWriter();
            writer.WriteImage(png, @"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog2.bmp");
            
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\BigTest.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\index.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\ricardo.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\git.png");
        }
    }
}