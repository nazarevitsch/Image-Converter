namespace Converter.Image
{
    public class Pixel
    {
        private byte _R;
        private byte _G;
        private byte _B;
        private byte _A;
        
        public Pixel()
        {
        }

        public Pixel(byte r, byte g, byte b)
        {
            _R = r;
            _G = g;
            _B = b;
        }

        public Pixel(byte r, byte g, byte b, byte a)
        {
            _R = r;
            _G = g;
            _B = b;
            _A = a;
        }

        public byte R
        {
            get => _R;
            set => _R = value;
        }

        public byte G
        {
            get => _G;
            set => _G = value;
        }

        public byte B
        {
            get => _B;
            set => _B = value;
        }

        public byte A
        {
            get => _A;
            set => _A = value;
        }

        public override string ToString()
        {
            return "Pixel: {R: " + _R + ", G: " + _G + ", B: " + _B + ", A: " + _A + "}";
        }
    }
}