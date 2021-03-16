using System;
using Converter;
using LZW;

namespace Convertor
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var ppmWriter = new PpmWriter();
            var bmpWriter = new BmpWriter();
            var bmpToPpmConverter = new BmpToPpmConverter();
            var ppmToBmpConvertor = new PpmToBmpConvertor();
            var convertor = new Converter.Converter();
            var ppm = new Ppm("./images/sample.ppm");
            var bmp = new Bmp("./images/BigTest.bmp");
            var boxes = new Ppm("./images/boxes_1.ppm");
            var gif = new Gif("./images/habr.gif");
            
            bmpWriter.Write(bmp, "./BigTest.bmp");
            var newPpm = convertor.Convert(bmp, bmpToPpmConverter);
            var bmpBoxes = convertor.Convert(boxes, ppmToBmpConvertor);
            ppmWriter.Write(newPpm);
            bmpWriter.Write(bmp, "./BigTest.ppm");
            bmpWriter.Write(bmpBoxes, "./boxes.bmp");
        }
    }
}