using Converter;
using Convertor.Formats;

namespace Convertor
{
    public class GifToPpmConverter : IImageConvertor
    {
        public IImageFormat Convert(IImageFormat format)
        {
            var ppm = new Ppm
            {
                Pixels = format.Pixels,
                Headers =
                {
                    ["MagicNumber"] = "P6\n",
                    ["Size"] = format.Headers["Width"] + " " + format.Headers["Height"] + "\n",
                    ["MaxValue"] = "255\n"
                }
            };
            return ppm;
        }
    }
}