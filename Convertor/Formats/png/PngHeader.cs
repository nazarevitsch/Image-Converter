namespace Convertor.Formats.png
{
    public class PngHeader
    {
        private int width;
        private int height;
        private int commpressionType;
        private int filteringType;
        private int bitDepth;
        private int colorType;
        private int interlace;

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

        public int CommpressionType
        {
            get => commpressionType;
            set => commpressionType = value;
        }

        public int FilteringType
        {
            get => filteringType;
            set => filteringType = value;
        }

        public int BitDepth
        {
            get => bitDepth;
            set => bitDepth = value;
        }

        public int ColorType
        {
            get => colorType;
            set => colorType = value;
        }

        public int Interlace
        {
            get => interlace;
            set => interlace = value;
        }

        public override string ToString()
        {
            return "PNG HEADER:{Width: " + width + ", Height: " + height + ", CompressionType: " + commpressionType + 
                   ", FilteringType: " + filteringType + ", Color Type: " + colorType + ", Bit Depth: " + bitDepth +
                   ", Interlace: " + interlace + "}";
        }
    }
}