
namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            PngReader pngReader = new PngReader();
            GifReader gifReader = new GifReader();
            IImageReader ppmReader = new PpmReader();
            Image.Image dog = pngReader.ReadImage(@"D:\Work\ImageConvertor\Converter\resources\Big.png");
            Image.Image bananaGif = gifReader.ReadImage(@"D:\Work\ImageConvertor\Converter\resources\banana.gif");
            Image.Image ppm = ppmReader.ReadImage(@"D:\Work\ImageConvertor\Converter\resources\dog.ppm");
            
            IImageWriter writer = new PpmWriter();
            IImageWriter bmpWriter = new BmpWriter();
            writer.WriteImage(dog, @"D:\Work\ImageConvertor\Converter\resources\Big.ppm");
            bmpWriter.WriteImage(dog, @"D:\Work\ImageConvertor\Converter\resources\Big.bmp");
            
            writer.WriteImage(bananaGif, @"D:\Work\ImageConvertor\Converter\resources\banana.ppm");
            bmpWriter.WriteImage(bananaGif, @"D:\Work\ImageConvertor\Converter\resources\banana.bmp");
            
            writer.WriteImage(ppm, @"D:\Work\ImageConvertor\Converter\resources\dogGIF.ppm");
            bmpWriter.WriteImage(ppm, @"D:\Work\ImageConvertor\Converter\resources\dogGIF.bmp");
            
        }
    }
}