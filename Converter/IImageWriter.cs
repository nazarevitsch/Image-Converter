using System;

namespace Converter
{
    public interface IImageWriter
    {
        void WriteImage(Image.Image image, String path);
    }
}