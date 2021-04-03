using System;
using System.IO;
using System.IO.Compression;
using System.Net.Mime;
using Convertor.Formats;
using Convertor.Formats.png;

namespace Converter
{
    public class PngWriter
    {
        public void WriteImage(IImageFormat image, String path)
        {
            PngChunk ihdr = GenerateIhdrChunk(image);
            PngChunk idat = GenerateIdatChunk(image);
            
            byte[] file = new byte[57 + idat.ChunkBody.Length];
            
            file[0] = 137;
            file[1] = 80;
            file[2] = 78;
            file[3] = 71;
            file[4] = 13;
            file[5] = 10;
            file[6] = 26;
            file[7] = 10;
            
            file[8] = 0;
            file[9] = 0;
            file[10] = 0;
            file[11] = 13;
            
            file[12] = 73;
            file[13] = 72;
            file[14] = 68;
            file[15] = 82;
            
            for (int i = 0; i <  ihdr.ChunkBody.Length; i++)
            {
                file[16 + i] = ihdr.ChunkBody[i];
            }
            
            file[29] = 141;
            file[30] = 111;
            file[31] = 38;
            file[32] = 229;
            
            byte[] idatSizeBytes = BitConverter.GetBytes(idat.ChunkBody.Length);
            
            file[33] = idatSizeBytes[3];
            file[34] = idatSizeBytes[2];
            file[35] = idatSizeBytes[1];
            file[36] = idatSizeBytes[0];
            
            file[37] = 73;
            file[38] = 68;
            file[39] = 65;
            file[40] = 84;
            
            for (int i = 0; i <  idat.ChunkBody.Length; i++)
            {
                file[41 + i] = idat.ChunkBody[i];
            }
            
            int iterator = 41 + idat.ChunkBody.Length;
            
            file[iterator] = 45;
            file[iterator + 1] = 27;
            file[iterator + 2] = 23;
            file[iterator + 3] = 249;
            
            file[iterator + 4] = 0;
            file[iterator + 5] = 0;
            file[iterator + 6] = 0;
            file[iterator + 7] = 0;
            
            file[iterator + 8] = 73;
            file[iterator + 9] = 69;
            file[iterator + 10] = 78;
            file[iterator + 11] = 68;
            
            file[iterator + 12] = 174;
            file[iterator + 13] = 66;
            file[iterator + 14] = 96;
            file[iterator + 15] = 130;
            
            File.WriteAllBytes(path, file);
        }

        private PngChunk GenerateIdatChunk(IImageFormat image)
        {
            PngChunk chunk = new PngChunk();
            chunk.PngChunkType = PngChunkType.IDAT;
            chunk.ChunkBody = Compressing(Filtering(image));
            return chunk;
        }
        
        private PngChunk GenerateIhdrChunk(IImageFormat image)
        {
            PngChunk chunk = new PngChunk();
            chunk.PngChunkType = PngChunkType.IHDR;
            chunk.ChunkBody = new byte[13];
            
            byte[] widthBytes = BitConverter.GetBytes(image.Width);
            byte[] heightBytes = BitConverter.GetBytes(image.Height);
            
            chunk.ChunkBody[0] = widthBytes[3];
            chunk.ChunkBody[1] = widthBytes[2];
            chunk.ChunkBody[2] = widthBytes[1];
            chunk.ChunkBody[3] = widthBytes[0];
            
            chunk.ChunkBody[4] = heightBytes[3];
            chunk.ChunkBody[5] = heightBytes[2];
            chunk.ChunkBody[6] = heightBytes[1];
            chunk.ChunkBody[7] = heightBytes[0];
            
            chunk.ChunkBody[8] = 8;
            chunk.ChunkBody[9] = 2;
            chunk.ChunkBody[10] = 0;
            chunk.ChunkBody[11] = 0;
            chunk.ChunkBody[12] = 0;
            
            return chunk;
        }

        private byte[] Filtering(IImageFormat image)
        {
            byte[] filteredData = new byte[image.Height * ((image.Width * 3) + 1)];
            int iteratorI = 0;
            for (int i = 0; i < filteredData.Length;)
            {
                filteredData[i] = 0;
                i++;
                for (int l = 0; l < image.Width; l++)
                {
                    filteredData[i + l * 3] = (byte) image._Pixels[iteratorI][l].Red;
                    filteredData[i + l * 3 + 1] = (byte) image._Pixels[iteratorI][l].Green;
                    filteredData[i + l * 3 + 2] = (byte)image._Pixels[iteratorI][l].Blue;
                }
                iteratorI++;
                i += image.Width * 3;
            }
            return filteredData;
        }

        private byte[] Compressing(byte[] filteredData)
        {
            Console.WriteLine("Before Compressing: " + filteredData.Length);
            var output = new MemoryStream();
            using(var zipStream = new DeflateStream(output, CompressionLevel.Fastest))
            {
                new MemoryStream(filteredData).CopyTo(zipStream);
            }
            Console.WriteLine(" After Compressing: " + output.ToArray().Length);
            return output.ToArray();
        }

    }
}