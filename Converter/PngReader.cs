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
            
            // for (int i = 0; i < array.Length; i++)
            // {
            //     Console.WriteLine("I: " + i + ", " + array[i] + ", CHAR: " + (char) array[i]);
            //     if (i > 40) break;
            // }

            List<PngChunk> chunks = ReadChunks(array);
            PngHeader header = GetPngHeader(chunks);
            
            Console.WriteLine(header);
            
            byte[] decompressedData = Decompressing(GetCompressedDataFromAllIdatChunks(chunks));
            Console.WriteLine(decompressedData.Length);
            Console.WriteLine(decompressedData[0]);
            // for (int i = 0; i < decompressedData.Length; i++)
            // {
            //     Console.WriteLine("I: " + i + ", " + decompressedData[i] + ", CHAR: " + (char) decompressedData[i]);
            // }
            return null;
        }

        private PngHeader GetPngHeader(List<PngChunk> chunks)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].PngChunkType.Equals(PngChunkType.IHDR))
                {
                    for (int j = 0; j < chunks[i].ChunkBody.Length; j++)
                    {
                        Console.Write(chunks[i].ChunkBody[j] + ", ");
                    }
                    Console.WriteLine();
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