using System.IO;
using Converter;
using Convertor.Compress;
using Convertor.Formats;

namespace Convertor
{
    
    class Program
    {
        static void Main(string[] args)
        {
            // var lzw = new GifLwz();
            // var colors = lzw.Decode(File.ReadAllBytes("test.bin"), new Pixel []
            // {
            //     new Pixel { Red = 0, Green = 0, Blue = 0, Alpha = 0 },
            //     new Pixel { Red = 127, Green = 127, Blue = 127, Alpha = 0 },
            //     new Pixel { Red = 191, Green = 191, Blue = 191, Alpha = 0 },
            //     new Pixel { Red = 63, Green = 63, Blue = 63, Alpha = 0 }
            // }, 3);
            
            PngReader pngReader = new PngReader();
            IImageFormat dog = pngReader.ReadImage(@"./images/dog.png");
           
            BmpWriter writer = new BmpWriter();
            writer.WriteImage(dog, @"./images/Result2.bmp");
            
            return;
            var ppmWriter = new PpmWriter();
            var gifWriter = new GifWriter();
            var gifToPpmConvertor = new GifToPpmConverter();
            var convertor = new Converter.Converter();
            var gif = new Gif("./images/man.gif");
            
            var gifConvertedToPpm = convertor.Convert(gif, gifToPpmConvertor);
            ppmWriter.Write(gifConvertedToPpm);
            gifWriter.Write(gif, "./images/test.gif");
        }
    }
}