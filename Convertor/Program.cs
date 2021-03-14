using System;
using System.Collections.Generic;
using System.IO;

namespace Converter
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var ppm = new Ppm("./images/sample.ppm");
            var bmp = new Bmp("./images/dog.bmp");
            var ppmWriter = new PpmWriter();
            var bmpWriter = new BmpWriter();
            var bmpConverter = new BmpToPpmConverter();
            var convertor = new Converter();
            bmpWriter.Write(bmp);
            var newPpm = convertor.Convert(bmp, bmpConverter);
            ppmWriter.Write(newPpm);
           // bmpWriter.Write(bmp);
        }
    }
}