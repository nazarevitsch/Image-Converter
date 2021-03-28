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
            
            // PngWriter pngWriter = new PngWriter();
            // pngWriter.WriteImage(dog, @"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result.png");
            
            // byte[] array1 = File.ReadAllBytes(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.png");
            // byte[] array2 = File.ReadAllBytes(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result.png")
            
            // PngReader pngReader = new PngReader();
            // Image.Image dog = pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.png");
            
            BmpWriter writer = new BmpWriter();
            writer.WriteImage(dog, @"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result2.bmp");
            
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\BigTest.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\index.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\ricardo.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\git.png");
        }
    }
}