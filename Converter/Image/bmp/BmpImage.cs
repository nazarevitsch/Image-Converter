namespace Converter.Image
{
    public class BmpImage
    {

        private int width;
        private int height;
        private byte[] pixels;

        public int Width
        {
            get => width;
            set => width = value;
        }

        public int Height
        {
            get => height;
            set => height = value;
        }

        public byte[] Pixels
        {
            get => pixels;
            set => pixels = value;
        }

        public override string ToString()
        {
            return "BMP: W: " + width + ", H:" + height + ", S: " + pixels.Length;
        }
    }
}