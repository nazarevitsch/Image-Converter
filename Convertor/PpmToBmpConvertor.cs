namespace Converter
{
    public class PpmToBmpConvertor : IImageConvertor
    {
        public IImageFormat Convert(IImageFormat format)
        {
            var pictureSize = format.Headers["Size"].Split(" ");
            var bmp = new Bmp
            {
                Bytes = format.Bytes,
                Pixels = format.Pixels,
                Headers = {["Width"] = pictureSize[0], ["Height"] = pictureSize[1]}
            };
            return bmp;
        }
    }
}