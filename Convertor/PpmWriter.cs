using System.IO;
using System.Text;

namespace Converter
{
    public class PpmWriter : IFormatWriter
    {
        public void Write(IImageFormat format, string path = "test.ppm")
        {
            var headers = format.Headers;
            StringBuilder sb = new StringBuilder(1000);
            foreach (var keyValuePair in headers)
            {
                sb.Append(keyValuePair.Value);
            }

            var bytes = new byte[format.Pixels.Length * 3];
            for (int pixel = 0, bytePos=0; pixel < format.Pixels.Length; pixel++, bytePos+=3)
            {
                var _pixel = format.Pixels[pixel];
                if (_pixel is null) continue;
                bytes[bytePos] = (byte)_pixel.Red;
                bytes[bytePos + 1] = (byte)_pixel.Green;
                bytes[bytePos + 2] = (byte)_pixel.Blue;
            }
            
            File.WriteAllText(path, sb.ToString());
            using var stream = new FileStream(path, FileMode.Append);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}