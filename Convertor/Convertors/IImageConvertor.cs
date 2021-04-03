using Convertor.Formats;

namespace Converter
{
    public interface IImageConvertor
    {
        public IImageFormat Convert(IImageFormat format);
    }
}