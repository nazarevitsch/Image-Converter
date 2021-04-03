using System.Linq;
using Convertor.Formats;

namespace Converter
{
    public class BmpToPpmConverter: IImageConvertor
    {
        public IImageFormat Convert(IImageFormat format)
        {
            var ppm = new Ppm
            {
                Pixels = format.Pixels.Reverse().ToArray(),
                Headers = {
                    ["MagicNumber"] = "P6\n",
                    ["Size"] = format.Headers["Width"] + " " + format.Headers["Height"]+"\n",
                    ["MaxValue"] = "255\n"
                    
                }
            };
            return ppm;
        }
    }
}