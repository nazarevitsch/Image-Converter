namespace Converter
{
    public class PpmToBmpConvertor : IImageConvertor
    {
        public IImageFormat Convert(IImageFormat format)
        {
            var bmp = new Bmp();
            bmp.Bytes = format.Bytes;
            var pictureSize = format.Headers["Size"].Split(" ");
            var height = int.Parse(pictureSize[0]);
            var width = int.Parse(pictureSize[1]);
            bmp.Headers["Width"] = "" + width;
            bmp.Headers["Height"] = "" + height;
            return bmp;
        }
    }
}