using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Converter.Image;

namespace Converter
{
    enum HeaderState
    {
        Empty = 0,
        Filling,
        Filled,
    }
    public class PpmReader : IImageReader
    {
        public Dictionary<string, string> Headers { get; set; } = new()
        {
            { "MagicNumber", "" },
            { "Size", "" },
            { "MaxValue", "" }
        };

        public Image.Image ReadImage(string path)
        {
            HeaderState isMagicNum = HeaderState.Empty,
                 isSize = HeaderState.Empty,
                 isMaxVal = HeaderState.Empty;
            
            var bytes = File.ReadAllBytes(path);
            var memoryStream = new MemoryStream(bytes);
            using (var reader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                char letter;
                int charCode;
                while ((charCode = reader.Read()) != -1)
                {
                    letter = (char) charCode;
                    
                    if (letter == 'P')
                        isMagicNum = HeaderState.Filling;

                    if (isMagicNum == HeaderState.Filling)
                        Headers["MagicNumber"] += letter;

                    if (isSize == HeaderState.Empty && isMagicNum == HeaderState.Filled)
                        isSize = HeaderState.Filling;

                    if (isSize == HeaderState.Filling)
                        Headers["Size"] += letter;

                    if (isMaxVal == HeaderState.Empty && isSize == HeaderState.Filled)
                        isMaxVal = HeaderState.Filling;

                    if (isMaxVal == HeaderState.Filling)
                        Headers["MaxValue"] += letter;
                    
                    if (letter == '\n')
                    {
                        if (isMagicNum == HeaderState.Filling) isMagicNum = HeaderState.Filled;
                        
                        if (isSize == HeaderState.Filling) isSize = HeaderState.Filled;
                        
                        if (isMaxVal == HeaderState.Filling) isMaxVal = HeaderState.Filled;
                    }
                    
                    if (isMagicNum == HeaderState.Filled &&
                        isSize == HeaderState.Filled &&
                        isMaxVal == HeaderState.Filled)
                    {
                        var pictureSize = Headers["Size"].Split(' ');
                        var height = int.Parse(pictureSize[0]);
                        var width = int.Parse(pictureSize[1]);
                        var pixels = new Pixel[height][];
                        // for (int row = 0; row < height; row++)
                        // {
                        //     var pixelRow = new Pixel[width];
                        //     for (int col = 0; col < width; col++)
                        //     {
                        //         pixelRow[col] = pixelsList[row * width + col];
                        //     }
                        //
                        //     pixels[row] = pixelRow;
                        // }
                        for (var i = 0; i < pixels.Length; i++)
                        {
                            pixels[i] = new Pixel[width];
                        }
                        for (int pos = (int) memoryStream.Position, col=0, row = 0; pos < bytes.Length; pos += 3, col++)
                        {
                            if (pixels[row] is null)
                            {
                                pixels[row] = new Pixel[width];
                            } 
                            pixels[row][col] = new Pixel
                            {
                                Red = bytes[pos],
                                Green = bytes[pos + 1],
                                Blue = bytes[pos + 2]
                            };
                            if (col >= width - 1)
                            {
                                row++;
                                col = -1;
                            };
                        }
                        return new Image.Image
                        {
                            Width = width,
                            Height = height,
                            Pixels = pixels
                        };
                    }
                }
            }

            return new Image.Image();
        }
    }
}