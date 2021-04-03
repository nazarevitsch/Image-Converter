namespace Converter.Image
{
    public class Pixel
    {
        public Pixel()
        {
        }

        public Pixel(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Pixel(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public byte Red { get; set; }

        public byte Green { get; set; }

        public byte Blue { get; set; }

        public byte Alpha { get; set; }

        public override string ToString()
        {
            return "Pixel: {R: " + Red + ", G: " + Green + ", B: " + Blue + ", A: " + Alpha + "}";
        }
    }
}