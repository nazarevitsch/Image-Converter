using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Converter.Image;
using Convertor.Compress;

namespace Converter
{
    public enum ImageProcessingMethods
    {
        NO_PROCESSING = 0,
        WITHOUT_CHANGE = 1,
        PICTURE_DELETED_BY_BACKGROUND = 2,
        RESTORE_IMAGE_UNDER_PICTURE = 3,
    
    }

    class ImageControlExtensionBlock
    {
        public int ImageProcessingMethod { get; set; }
        public bool UserInputFlag { get; set; }
        public bool TransparencyFlag { get; set; }
        public int TransparencyIndex{ get; set; }
        public int BlockSize { get; set; }
        public double Delay { get; set; }
    
    }

    public static class BlockCodes
    {
        public const byte ExtensionBlock = 0x21;
        public const byte ImageBlock = 0x2C;
        public const byte EndOfFile = 0x3B;
    }

    public static class ExtensionCodes
    {
        public const byte TextExtension = 0x1;
        public const byte ImageControlExtension = 0xF9;
        public const byte CommentExtension = 0xFE;
        public const byte ProgramExtension = 0xFF;
    }

    public class GifReader : IImageReader
    {
        public Dictionary<string, string> Headers { get; set; } = new();
        
        private const int PaletteStartIndex = 13;
        
        public Image.Image ReadImage(string path)
        {
            var fileData = File.ReadAllBytes(path);
            
            var width = BitConverter.ToInt16(fileData, 6);
            var height = BitConverter.ToInt16(fileData, 8);
            var imageResult = new Image.Image {Width = width, Height = height};

            
            Headers["GifVersion"]  = Encoding.UTF8.GetString(fileData, 0, 6);
            Headers["Width"] = "" + width;
            Headers["Height"] = "" + height;
            
            var globalColorsDataByte = fileData[10];
            var isGlobalPaletteExist = (globalColorsDataByte & 0b10000000) >> 7;
            var isSortedColors = (globalColorsDataByte & 0b00001000) >> 3;
            var colorDepth = Math.Pow(2.0, ((globalColorsDataByte & 0b01110000) >> 4) + 1.0);
            var paletteSize = 3 * Math.Pow(2.0, (globalColorsDataByte & 0b00000111) + 1.0);
            if (isGlobalPaletteExist == 0)
            {
                throw new Exception("Not supported");
            }
            Headers["isGlobalPaletteExist"] = "" + isGlobalPaletteExist;
            Headers["ColorDepth"] = colorDepth + "";
            Headers["SortedColors"] = isSortedColors + "";
            Headers["PaletteSize"] = paletteSize + "";

            var colorPalette = new Pixel[(int) paletteSize / 3];
            
            for (int paletteIndex = PaletteStartIndex, row = 0; paletteIndex < PaletteStartIndex + paletteSize; paletteIndex+=3, row++)
            {
                colorPalette[row] = new Pixel
                {
                    Red = fileData[paletteIndex],
                    Green = fileData[paletteIndex + 1],
                    Blue = fileData[paletteIndex + 2]
                };
                
            }
            var dataBlockIndex = PaletteStartIndex + (int) paletteSize;
            var dataBlockType = fileData[dataBlockIndex];
            var dataBlockExtensionType = fileData[dataBlockIndex + 1];
            var dataBlockSize = Convert.ToInt32(fileData[dataBlockIndex + 2]);
            
            while (dataBlockType == BlockCodes.ExtensionBlock)
            {
                if (dataBlockExtensionType == ExtensionCodes.ProgramExtension || dataBlockExtensionType == ExtensionCodes.CommentExtension)
                {
                    dataBlockIndex = SkipBlock(fileData, dataBlockIndex);
                    dataBlockType = fileData[dataBlockIndex];
                    dataBlockExtensionType = fileData[dataBlockIndex + 1];
                    continue;
                }

                if (dataBlockExtensionType == ExtensionCodes.ImageControlExtension)
                {
                    dataBlockSize = fileData[dataBlockIndex + 2];
                    var imageProcessingByte = fileData[dataBlockIndex + 3];
                    var userInputFlag = (imageProcessingByte & 0b00000010) >> 1;
                    var transparencyFlag = imageProcessingByte & 0b0000001;
                    var imageProcessingMethod = (0b00011100 & imageProcessingByte) >> 2;
                    var delay = (double) BitConverter.ToInt16(fileData, dataBlockIndex + 4) / 100;
                    var transparencyIndex = Convert.ToInt32(fileData[dataBlockIndex + 6]);

                    dataBlockIndex += 8;
                    dataBlockType = fileData[dataBlockIndex];
                    dataBlockExtensionType = fileData[dataBlockIndex + 1];
                }
            }

            if (dataBlockType == BlockCodes.ImageBlock)
            {
                
                var logicalRow = BitConverter.ToInt16(fileData, dataBlockIndex + 1);
                var logicalCol = BitConverter.ToInt16(fileData, dataBlockIndex + 3);
               // var width = BitConverter.ToInt16(fileData, dataBlockIndex + 5);
               // var height = BitConverter.ToInt16(fileData, dataBlockIndex + 7);
                var localPaletteByte = fileData[dataBlockIndex + 8];
                var localPaletteUsingFlag = (localPaletteByte & 0b10000000) >> 7;
                var interlacedFlag = (localPaletteByte & 0b01000000) >> 6;
                var localPaletteSortedFlag = (localPaletteByte & 0b00100000) >> 5;
                var localPaletteSize = localPaletteByte & 0b00000111;
                var minLzwCode = Convert.ToByte(fileData[dataBlockIndex + 10]) + 1;
                var imageCompressedSize = Convert.ToByte(fileData[dataBlockIndex + 11]);
                var compressedImage = fileData.Skip(dataBlockIndex + 12).Take(imageCompressedSize).ToList();
                dataBlockIndex += 12 + imageCompressedSize;
                while (fileData[dataBlockIndex] != 0x00)
                {
                    imageCompressedSize = Convert.ToByte(fileData[dataBlockIndex]);
                    compressedImage.AddRange(fileData.Skip(dataBlockIndex + 1).Take(imageCompressedSize).ToList());
                    dataBlockIndex += imageCompressedSize + 1;
                }
                var decoder = new GifLwz();
                var pixelsList = decoder.Decode(compressedImage.ToArray(), colorPalette, minLzwCode);
                var pixels = new Pixel[height][];
                for (int row = 0; row < height; row++)
                {
                    var pixelRow = new Pixel[width];
                    for (int col = 0; col < width; col++)
                    {
                        pixelRow[col] = pixelsList[row * width + col];
                    }

                    pixels[row] = pixelRow;
                }

                imageResult.Pixels = pixels;
            }

            return imageResult;
        }
        public int SkipBlock(byte [] fileData, int dataBlockIndex)
        {
            int startIndex = dataBlockIndex;
            while (!(fileData[startIndex] == 0x00 && fileData[startIndex + 1] != 0x00))
            {
                startIndex++;
            }

            return startIndex + 1;
        }

        public static List<Pixel> GetUniqueColors<T> (T colors) where T : IList<Pixel>
        {
            var uniqueColors = new Dictionary<Pixel, byte>();

            foreach (var color in colors.Where(color => !uniqueColors.ContainsKey(color)))
            {
                uniqueColors[color] = 1;
            }

            return uniqueColors.Keys.ToList();
        }
        
        
    }
}