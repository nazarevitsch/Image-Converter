using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Converter;
using Convertor.Compress;
using Convertor.Formats;

namespace Convertor
{
    public class GifWriter : IFormatWriter
    {
        public void Write(IImageFormat format, string path = "test.gif")
        {
            var gifHeaderVersion = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 };
            var height = BitConverter.GetBytes(ushort.Parse(format.Headers["Height"]));
            var width = BitConverter.GetBytes(ushort.Parse(format.Headers["Height"]));
            var uniqueColors = Gif.GetUniqueColors(format.Pixels);
            var paletteSize = (int) Math.Ceiling(Math.Log2(uniqueColors.Count) - 1);
            var paletteDescription = "1100" + Convert.ToString(paletteSize, 2).PadLeft(4, '0');
            var logicalScreenDescriptor = new byte[]
            {
                height[0], height[1],
                width[0], width[1],
                Convert.ToByte(paletteDescription, 2),
                0x00, // transparency background
                0x00
            };

            var globalPalette = new byte[(int) Math.Pow(2, paletteSize + 1.0) * 3];
            for (int color = 0, byteIndex = 0; color < uniqueColors.Count; color++, byteIndex += 3)
            {
                var pixel = uniqueColors[color];
                globalPalette[byteIndex] = (byte) pixel.Red;
                globalPalette[byteIndex + 1] = (byte) pixel.Green;
                globalPalette[byteIndex + 2] = (byte) pixel.Blue;
            }
            
            GifLwz gifLwz = new GifLwz();
            var code = gifLwz.Encode(format.Pixels, (int) Math.Pow(2, paletteSize + 1.0));
            
            var imageBlock = new List<byte>
            {
                0x2C, // Image block flag
                0x00, 0x00, // row
                0x00, 0x00, // col
                width[0], width[1], // width
                height[0], height[1], // height
                0x00, // localPalette,
                code.MinCodeSize
            };
            if (code.EncodedData.Count <= 255)
            {
                imageBlock.Add((byte) code.EncodedData.Count);
                imageBlock.AddRange(code.EncodedData);
            }
            else
            {
                var offset = 0;
                while (offset < code.EncodedData.Count)
                {
                    int bytesLeft = code.EncodedData.Count - offset;
                    imageBlock.Add(bytesLeft > 255 ? (byte) 255 : (byte) bytesLeft);
                    var bytes = code.EncodedData.Skip(offset).Take(255);
                    imageBlock.AddRange(bytes);
                    offset += 255;
                }
            }
           
            imageBlock.Add(0x00);
            imageBlock.Add(0x3B);

            using var stream = new FileStream(path, FileMode.Create);
            stream.Write(gifHeaderVersion, 0, gifHeaderVersion.Length);
            stream.Write(logicalScreenDescriptor, 0, logicalScreenDescriptor.Length);
            stream.Write(globalPalette, 0, globalPalette.Length);
            stream.Write(imageBlock.ToArray(), 0, imageBlock.Count);
        }
    }
}