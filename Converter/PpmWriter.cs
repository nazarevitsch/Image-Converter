using System.IO;

namespace Converter
{
    public class PpmWriter : IImageWriter
    {
        public void WriteImage(Image.Image format, string path = "test.ppm")
        {
            var width = format.Width;
            var height = format.Height;
            
            var bytes = new byte[width * height * 3];
            var bytePos = 0;
            for (int row = 0 ; row < format.Pixels.Length; row++)
            {
                for (int col = 0; col < width; col++, bytePos+=3)
                {
                    var _pixel = format.Pixels[row][col];
                    if (_pixel is null) continue;
                    bytes[bytePos] = (byte)_pixel.Red;
                    bytes[bytePos + 1] = (byte)_pixel.Green;
                    bytes[bytePos + 2] = (byte)_pixel.Blue;
                }
                
            }
            
            File.WriteAllText(path, $"P6\n{width} {height}\n255\n");
            using var stream = new FileStream(path, FileMode.Append);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}