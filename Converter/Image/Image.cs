using System;

namespace Converter.Image
{
    public class Image
    {
        public Image()
        {
        }

        public Image(int width, int height, Pixel[][] pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public Pixel[][] Pixels { get; set; }

        public void PrintPixels()
        {
            for (int i = 0; i < Pixels.Length; i++)
            {
                for (int l = 0; l < Pixels.Length; l++)
                {
                    Console.Write(Pixels[i][l] + ",     ");
                }
                Console.WriteLine();
            }
        }

        public override string ToString()
        {
            return "Image: {Width: " + Width + ", Height: " + Height + "}";
        }
    }
}