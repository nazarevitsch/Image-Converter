using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Converter;
using LZW;


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

namespace Converter
{
    public class Gif : IImageFormat
    {
        public Dictionary<int, string> ColorTable { get; set; } = new();
        public Dictionary<string, string> Headers { get; set; } = new();
        public byte[] Bytes { get; set; }
        public Pixel[] Pixels { get; set; }

        private int _paletteStartIndex = 13;

        private List<ImageControlExtensionBlock> _blocks = new();
        public Gif(string path)
        {
            ReadData(path);
        }
        
        public void ReadData(string path)
        {
            var fileData = File.ReadAllBytes(path);
            Headers["GifVersion"]  = Encoding.UTF8.GetString(fileData, 0, 6);
            Headers["Width"] = "" + BitConverter.ToInt16(fileData, 6);
            Headers["Height"] = "" + BitConverter.ToInt16(fileData, 8);
            
            var globalColorsDataByte = fileData[10];
            var isGlobalPaletteExist = (globalColorsDataByte & 0b10000000) >> 7;
            var isSortedColors = (globalColorsDataByte & 0b00001000) >> 3;
            var colorDepth = Math.Pow(2.0, ((globalColorsDataByte & 0b01110000) >> 4) + 1.0);
            var paletteSize = 3 * Math.Pow(2.0, (globalColorsDataByte & 0b00000111) + 1.0);
            
            Headers["isGlobalPaletteExist"] = "" + isGlobalPaletteExist;
            Headers["ColorDepth"] = colorDepth + "";
            Headers["SortedColors"] = isSortedColors + "";
            Headers["PaletteSize"] = paletteSize + "";

            Pixels = new Pixel[(int) paletteSize / 3];
            
            for (int paletteIndex = _paletteStartIndex, row = 0; paletteIndex < _paletteStartIndex + paletteSize; paletteIndex+=3, row++)
            {
                Pixels[row] = new Pixel
                {
                    Red = fileData[paletteIndex],
                    Green = fileData[paletteIndex + 1],
                    Blue = fileData[paletteIndex + 2]
                };
                
            }
            var dataBlockIndex = _paletteStartIndex + (int) paletteSize;
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
                    
                    _blocks.Add(new ImageControlExtensionBlock
                    {
                        UserInputFlag = userInputFlag == 1,
                        ImageProcessingMethod = imageProcessingMethod,
                        BlockSize = dataBlockSize,
                        TransparencyFlag = transparencyFlag == 1,
                        TransparencyIndex = transparencyIndex,
                        Delay = delay
                    });

                    dataBlockIndex += 8;
                    dataBlockType = fileData[dataBlockIndex];
                    dataBlockExtensionType = fileData[dataBlockIndex + 1];
                }
            }

            if (dataBlockType == BlockCodes.ImageBlock)
            {
                var logicalRow = BitConverter.ToInt16(fileData, dataBlockIndex + 1);
                var logicalCol = BitConverter.ToInt16(fileData, dataBlockIndex + 3);
                var width = BitConverter.ToInt16(fileData, dataBlockIndex + 5);
                var height = BitConverter.ToInt16(fileData, dataBlockIndex + 7);
                var localPaletteByte = fileData[dataBlockIndex + 8];
                var localPaletteUsingFlag = (localPaletteByte & 0b10000000) >> 7;
                var interlacedFlag = (localPaletteByte & 0b01000000) >> 6;
                var localPaletteSortedFlag = (localPaletteByte & 0b00100000) >> 5;
                var localPaletteSize = localPaletteByte & 0b00000111;
                var minLzwCode = Convert.ToInt32(fileData[dataBlockIndex + 10]);
                var imageCompressedSize = Convert.ToInt32(fileData[dataBlockIndex + 11]);
                var compressedImage = fileData.Skip(dataBlockIndex + 12).Take(imageCompressedSize).ToArray();
                var decoder = new LZWDecoder();
                var decodedString = decoder.DecodeFromCodes(compressedImage);
            }
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
    }
}