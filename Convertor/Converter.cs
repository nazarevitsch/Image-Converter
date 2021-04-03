using Convertor.Formats;

namespace Converter
{
    public class Converter
    {
        public IImageFormat Convert(IImageFormat format, IImageConvertor convertor) 
        {
            return convertor.Convert(format);
        }
    }
}