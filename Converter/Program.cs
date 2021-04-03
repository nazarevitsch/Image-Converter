using System;
using System.IO;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            // BmpReader bmpReader = new BmpReader();
            // Image.Image dog = bmpReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\BigTest.bmp");
           
            PngReader pngReader = new PngReader();
            Image.Image dog = pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.png");
            
            BmpWriter writer = new BmpWriter();
            writer.WriteImage(dog, @"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result2.bmp");
            
            
        }
    }
}