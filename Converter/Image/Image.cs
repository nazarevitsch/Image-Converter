using System;

namespace Converter.Image
{
    public class Image
    {
        private int _Width;
        private int _Height;
        private Pixel[][] _Pixels;

        public Image()
        {
        }

        public Image(int width, int height, Pixel[][] pixels)
        {
            _Width = width;
            _Height = height;
            _Pixels = pixels;
        }

        public int Width
        {
            get => _Width;
            set => _Width = value;
        }

        public int Height
        {
            get => _Height;
            set => _Height = value;
        }

        public Pixel[][] Pixels
        {
            get => _Pixels;
            set => _Pixels = value;
        }

        public void PrintPixels()
        {
            for (int i = 0; i < _Pixels.Length; i++)
            {
                for (int l = 0; l < _Pixels.Length; l++)
                {
                    Console.Write(_Pixels[i][l] + ",     ");
                }
                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            return "Image: {Width: " + _Width + ", Height: " + _Height + "}";
        }
    }
}