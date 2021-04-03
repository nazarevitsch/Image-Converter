using Convertor.Formats;

namespace Converter
{
    public interface IFormatWriter
    {
        public void Write(IImageFormat format, string path);
    }
}