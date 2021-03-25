using System;
using System.IO;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            // BmpReader bmpReader = new BmpReader();
            // Image.Image dog = bmpReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.bmp");
           
            // PngReader pngReader = new PngReader();
            // Image.Image dog = pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\git.png");
            //
            // PngWriter pngWriter = new PngWriter();
            // pngWriter.WriteImage(dog, @"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result.png");
            
            byte[] array1 = File.ReadAllBytes(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.png");
            byte[] array2 = File.ReadAllBytes(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result.png");
            
            // for (int i = 0; i < array1.Length; i++)
            // {
            //     Console.WriteLine("I: " + i + ", O: " + array1[i] + ", C: " + array2[i] + ", TRUE: " + (array1[i] == array2[i]));
            // }
            for (int i = 0; i < array1.Length; i++)
            {
                Console.WriteLine("I: " + i + ", O: " + array1[array1.Length - i - 1] + ", C: " + array2[array2.Length - i - 1] + ", TRUE: " + (array1[array1.Length - i - 1] == array2[array2.Length - i - 1]));
            }
            
            
            // PngReader pngReader = new PngReader();
            // Image.Image dog = pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result.png");
            
            
            
            // BmpWriter writer = new BmpWriter();
            // writer.WriteImage(dog, @"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\Result.bmp");
            
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\BigTest.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\index.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\ricardo.png");
            // pngReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\git.png");
        }
    }
}