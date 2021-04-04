namespace Converter
{
    public interface IImageReader
    {
        public Image.Image ReadImage(string path);
    }
}