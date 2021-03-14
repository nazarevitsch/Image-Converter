using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Converter
{
    
    public class BmpOld : IImageFormat
    {
        public Dictionary<string, string> Headers { get; set; }
        public byte[] Bytes { get; set; }
        public Pixel[] Pixels { get; set; }

        public Pixel[,] Pixels1 { get; set; }
        public BmpOld(string path)
        {
            ReadData(path);
        }
        
        private void ReadData(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var memoryStream = new MemoryStream(bytes);
            using (var reader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                byte[] BITMAPFILEHEADER = reader.ReadBytes(14);
                byte[] BITMAPINFOHEADER = reader.ReadBytes(40);
                
                byte[] heightt = new byte[4];
                byte[] weightt = new byte[4];
                byte[] startPicture = new byte[4];
                for(short i = 0; i < 4; i++){
                    startPicture[i] = BITMAPFILEHEADER[i + 10];
                    weightt[i] = BITMAPINFOHEADER[i + 4];
                    heightt[i] = BITMAPINFOHEADER[i + 8];
                }

                int startImage = BitConverter.ToInt32(startPicture, 0);
                int height = BitConverter.ToInt32(heightt, 0);
                int weight = BitConverter.ToInt32(weightt, 0);
                
                CreatPixelMatrix(reader, BITMAPFILEHEADER, BITMAPINFOHEADER, startImage, weight, height);
            }
        }
        private void CreatPixelMatrix(BinaryReader reader, byte [] BITMAPFILEHEADER, byte [] BITMAPINFOHEADER, int startImage, int weight, int height)
        {
            byte[] buf = weight * 3 % 4 == 0 ? new byte[weight * 3] : new byte[weight * 3 + 4 - weight * 3 % 4];
            byte[] extaHead;
            if(BITMAPFILEHEADER.Length + BITMAPINFOHEADER.Length < startImage)
            {
                extaHead = new byte[startImage - BITMAPFILEHEADER.Length + BITMAPINFOHEADER.Length];
                reader.Read(extaHead);
            }

            Pixels1 = new Pixel[Math.Abs(height) + 1, weight + 1];
            
            int realSize = weight * 3;
            int start = height > 0 ? Math.Abs(height) - 1 : 0;
            
            for (int i = start; i >= 0; i--) {
                reader.Read(buf);
                int iterator = 0;
                for (int l = 0; l < realSize; l += 3)
                {
                    Pixels1[i,iterator] =  new Pixel
                    {
                        Red = buf[l],
                        Green = buf[l + 1],
                        Blue = buf[l + 2]
                    };
                    iterator++;
                }
            }
            Bytes = buf;
        }
    }
}