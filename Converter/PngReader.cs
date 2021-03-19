﻿﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
  using System.Runtime.Remoting.Messaging;
  using Converter.Image;


namespace Converter
{
    public class PngReader : IImageReader
    {
        
        public Image.Image ReadImage(String fileName)
        {
            byte[] array = File.ReadAllBytes(fileName);
            
            // for (int i = 0; i < array.Length; i++)
            // {
            //     Console.WriteLine("I: " + i + ", " + array[i] + ", CHAR: " + (char) array[i]);
            //     if (i > 40) break;
            // }

            List<PngChunk> chunks = ReadChunks(array);
            PngHeader header = GetPngHeader(chunks);
            
            Console.WriteLine(header);
            
            byte[] decompressedData = Decompressing(GetCompressedDataFromAllIdatChunks(chunks));

            // for (int i = 0; i < decompressedData.Length; i++)
            // {
            //     Console.WriteLine("I: " + i + ", " + decompressedData[i]);
            // }
            
            Image.Image image = new Image.Image(header.Width, header.Height, Defiltering(decompressedData, header));
            // image.PrintPixels();
            return image;
        }

        private Pixel[][] Defiltering(byte[] filteredData, PngHeader pngHeader)
        {
            Pixel[][] pixels = new Pixel[pngHeader.Height][];
            int bytesPerPixel = pngHeader.ColorType == 2 ? 3 : 4;
            for (int i = 0; i < pngHeader.Height; i++)
            {
                pixels[i] = new Pixel[pngHeader.Width];
                int starOfRow = i * bytesPerPixel * pngHeader.Width + i;
                int filtering = filteredData[starOfRow];
                if (filtering == 1)
                {
                    for (int l = 1; l < pngHeader.Width; l++)
                    {
                        // Console.WriteLine("L: " + l);
                        // Console.WriteLine("1: " + (starOfRow + 1 + (bytesPerPixel * l)) + ", 2: " + (starOfRow + 1 + (bytesPerPixel * l)) + ", 3: " + (starOfRow + 1 + (bytesPerPixel * (l - 1))));
                        // Console.WriteLine("S1: " + filteredData[(starOfRow + 1 + (bytesPerPixel * l))] + ", S2: " + filteredData[(starOfRow + 1 + (bytesPerPixel * (l - 1)))]);
                        filteredData[starOfRow + 1 + (bytesPerPixel * l)] = (byte)
                            (filteredData[starOfRow + 1 + (bytesPerPixel * l)] + filteredData[starOfRow + 1 + (bytesPerPixel * (l - 1))]);
                        filteredData[starOfRow + 2 + (bytesPerPixel * l)] = (byte)
                            (filteredData[starOfRow + 2 + (bytesPerPixel * l)] + filteredData[starOfRow + 2 + (bytesPerPixel * (l - 1))]);
                        filteredData[starOfRow + 3 + (bytesPerPixel * l)] = (byte)
                            (filteredData[starOfRow + 3 + (bytesPerPixel * l)] + filteredData[starOfRow + 3 + (bytesPerPixel * (l - 1))]);
                        if (bytesPerPixel == 4) filteredData[starOfRow + 4 + (bytesPerPixel * l)] = (byte)
                            (filteredData[starOfRow + 4 + (bytesPerPixel * l)] + filteredData[starOfRow + 4 + (bytesPerPixel * (l - 1))]);
                    }
                }
                for (int l = 0; l < pngHeader.Width; l++)
                {
                    pixels[i][l] = pngHeader.ColorType == 2
                        ? new Pixel(filteredData[(starOfRow + 1) + (bytesPerPixel * l)],
                            filteredData[(starOfRow + 2) + (bytesPerPixel * l)],
                            filteredData[(starOfRow + 3) + (bytesPerPixel * l)])
                        : new Pixel(filteredData[(starOfRow + 1) + (bytesPerPixel * l)],
                            filteredData[(starOfRow + 2) + (bytesPerPixel * l)],
                            filteredData[(starOfRow + 3) + (bytesPerPixel * l)],
                            filteredData[(starOfRow + 4) + (bytesPerPixel * l)]);
                }
            }
            return pixels;
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
            var output = new MemoryStream();
            using(var zipStream = new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress))
            {
                zipStream.CopyTo(output);
                zipStream.Close();
                return output.ToArray();
            }
        }
    }
}