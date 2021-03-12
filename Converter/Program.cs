using System;
using Converter.Image;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            BmpReader bmpReader = new BmpReader();
            BmpImage dog1 = bmpReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog.bmp");
            Console.WriteLine(dog1);
            // BmpImage dog2 = bmpReader.ReadImage(@"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog2.bmp");
            // Console.WriteLine(dog2);
            BmpWriter writer = new BmpWriter();
            writer.WriteImage(dog1, @"C:\Users\nazar\RiderProjects\\Converter\Converter\resources\dog2.bmp");
        }
    }
}