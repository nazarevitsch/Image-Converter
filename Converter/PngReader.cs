﻿﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
  using Converter.Image;


namespace Converter
{
    public class PngReader : IImageReader
    {
        
        public Image.Image ReadImage(String fileName)
        {
            byte[] array = File.ReadAllBytes(fileName);

            List<PngChunk> chunks = ReadChunks(array);
            PngHeader header = GetPngHeader(chunks);

            byte[] decompressedData = Decompressing(GetCompressedDataFromAllIdatChunks(chunks));
            byte[] unfilteredData = Unfiltering(decompressedData, header);
            
            Image.Image image = new Image.Image(header.Width, header.Height, GetPixelMatrix(unfilteredData, header));
            return image;
        }

        private Pixel[][] GetPixelMatrix(byte[] unfilteredData, PngHeader pngHeader)
        {
            Pixel[][] pixels = new Pixel[pngHeader.Height][];
            int bytesPerPixel = pngHeader.ColorType == 2 ? 3 : 4;
            for (int i = 0; i < pngHeader.Height; i++)
            {
                int starOfRow = i * bytesPerPixel * pngHeader.Width + i;
                pixels[i] = new Pixel[pngHeader.Width];
                for (int l = 0; l < pngHeader.Width; l++)
                {
                    pixels[i][l] = pngHeader.ColorType == 2
                        ? new Pixel(unfilteredData[(starOfRow + 1) + (bytesPerPixel * l)],
                            unfilteredData[(starOfRow + 2) + (bytesPerPixel * l)],
                            unfilteredData[(starOfRow + 3) + (bytesPerPixel * l)],
                            0)
                        : new Pixel(unfilteredData[(starOfRow + 1) + (bytesPerPixel * l)],
                            unfilteredData[(starOfRow + 2) + (bytesPerPixel * l)],
                            unfilteredData[(starOfRow + 3) + (bytesPerPixel * l)],
                            unfilteredData[(starOfRow + 4) + (bytesPerPixel * l)]);
                }
            }
            return pixels;
        }

        private byte[] Unfiltering(byte[] filteredData, PngHeader pngHeader)
        {
            int bytesPerPixel = pngHeader.ColorType == 2 ? 3 : 4;
            for (int i = 0; i < filteredData.Length;)
            {
                if (filteredData[i] == 1)
                {
                    for (int l = i + 1 + bytesPerPixel; l < pngHeader.Width * bytesPerPixel + 1 + i; l++)
                    {
                        filteredData[l] += filteredData[l - bytesPerPixel];
                    }
                }
                if (filteredData[i] == 2)
                {
                    for (int l = i + 1; l < pngHeader.Width * bytesPerPixel + 1 + i; l++)
                    {
                        filteredData[l] += filteredData[l - (pngHeader.Width * bytesPerPixel) - 1];
                    }
                }
                if (filteredData[i] == 3)
                {
                    for (int l = i + 1; l < i + 1 + bytesPerPixel; l++)
                    {
                        filteredData[l] += (byte)(filteredData[l - pngHeader.Width * bytesPerPixel - 1] / 2);
                    }
                    for (int l = i + 1 + bytesPerPixel; l < pngHeader.Width * bytesPerPixel + 1 + i; l++)
                    {
                        filteredData[l] += (byte)((filteredData[l - bytesPerPixel] + filteredData[l - 1 - pngHeader.Width * bytesPerPixel]) / 2);
                    }
                }
                if (filteredData[i] == 4)
                {
                    for (int l = i + 1; l < i + 1 + bytesPerPixel; l++)
                    {
                        byte a = 0;
                        byte b = filteredData[l - (pngHeader.Width * bytesPerPixel) - 1];
                        byte c = 0;
                        int pa = Math.Abs(b);
                        int pb = Math.Abs(a);
                        int pc = Math.Abs(a + b);
                        byte pr = pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
                        filteredData[l] += pr;
                    }
                    for (int l = i + 1 + bytesPerPixel; l < pngHeader.Width * bytesPerPixel + 1 + i; l++)
                    {
                        byte a = filteredData[l - bytesPerPixel];
                        byte b = filteredData[l - (pngHeader.Width * bytesPerPixel) - 1];
                        byte c = filteredData[l - (pngHeader.Width * bytesPerPixel) - bytesPerPixel - 1];
                        int pa = Math.Abs(b - c);
                        int pb = Math.Abs(a - c);
                        int pc = Math.Abs(a + b - c - c);
                        byte pr = pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
                        filteredData[l] += pr;
                    }
                }
                i += pngHeader.Width * bytesPerPixel + 1;
            }
            return filteredData;
        }

        private PngHeader GetPngHeader(List<PngChunk> chunks)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].PngChunkType.Equals(PngChunkType.IHDR))
                {
                    PngHeader header = new PngHeader();
                    byte[] widthBytes = {chunks[i].ChunkBody[3], chunks[i].ChunkBody[2], chunks[i].ChunkBody[1], chunks[i].ChunkBody[0]};
                    header.Width = BitConverter.ToInt32(widthBytes, 0);
                    byte[] heightBytes = {chunks[i].ChunkBody[7], chunks[i].ChunkBody[6], chunks[i].ChunkBody[5], chunks[i].ChunkBody[4]};
                    header.Height = BitConverter.ToInt32(heightBytes, 0);
                    header.BitDepth = chunks[i].ChunkBody[8];
                    header.ColorType = chunks[i].ChunkBody[9];
                    header.CommpressionType = chunks[i].ChunkBody[10];
                    header.FilteringType = chunks[i].ChunkBody[11];
                    header.Interlace = chunks[i].ChunkBody[12];
                    return header;
                }
            }
            return null;
        }

        private byte[] GetCompressedDataFromAllIdatChunks(List<PngChunk> chunks)
        {
            byte[] compressedData = {};
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].PngChunkType.Equals(PngChunkType.IDAT))
                    compressedData = compressedData.Concat(chunks[i].ChunkBody).ToArray();
            }
            return compressedData;
        }

        private List<PngChunk> ReadChunks(byte[] array)
        {
            List<PngChunk> chunks = new List<PngChunk>();
            
            for (int i = 8; i < array.Length;)
            {
                byte[] chunkSizeBytes = {array[i + 3], array[i + 2], array[i + 1], array[i]};
                int chunkSize = BitConverter.ToInt32(chunkSizeBytes, 0);
                byte[] chunkNameBytes = {array[i + 4], array[i + 5], array[i + 6], array[i + 7]};
                String chunkName = System.Text.Encoding.UTF8.GetString(chunkNameBytes, 0, chunkNameBytes.Length);
                if (chunkName.Equals(PngChunkType.IEND.ToString()))
                {
                    return chunks;
                }
                PngChunk chunk = new PngChunk();
                chunk.PngChunkType = DeterminePngChunkType(chunkName);
                chunk.ChunkBody = array.Skip(8 + i).Take(chunkSize).ToArray();
                chunks.Add(chunk);
                i += chunkSize + 8 + 4;
            }
            return chunks;
        }

        private PngChunkType DeterminePngChunkType(String chunkName)
        {
            if (PngChunkType.IDAT.ToString().Equals(chunkName))
                return PngChunkType.IDAT;
            if (PngChunkType.IHDR.ToString().Equals(chunkName))
                return PngChunkType.IHDR;
            if (PngChunkType.IEND.ToString().Equals(chunkName))
                return PngChunkType.IEND;
            if (PngChunkType.PLTE.ToString().Equals(chunkName))
                return PngChunkType.PLTE;
            return PngChunkType.UNNEDEED;
        }

        private byte[] Decompressing(byte[] compressedData)
        {
            Console.WriteLine("Before Decompressing: " + compressedData.Length);
            var output = new MemoryStream();
            using(var zipStream = new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress))
            {
                zipStream.CopyTo(output);
                zipStream.Close();
                Console.WriteLine("After Decompressing: " + output.Length);
                return output.ToArray();
            }
        }
    }
}