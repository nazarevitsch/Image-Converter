using System;
using System.Collections.Generic;
using Converter;
using LZW;

namespace Convertor
{
    
    class Program
    {
        static void Main(string[] args)
        {
            var ppmWriter = new PpmWriter();
            var gifToPpmConvertor = new GifToPpmConverter();
            var convertor = new Converter.Converter();
            var gif = new Gif("./images/tree.gif");
            
            var gifConvertedToPpm = convertor.Convert(gif, gifToPpmConvertor);
            ppmWriter.Write(gifConvertedToPpm);
        }
    }
}