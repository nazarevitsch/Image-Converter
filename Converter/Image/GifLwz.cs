using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Converter;
using Converter.Image;
using LZW;

namespace Convertor.Compress
{
    public class GifLwz
    {
        public record EncodedResult
        {
            public byte MinCodeSize { get; set; }
            public List<byte> EncodedData { get; set; }
        }

        public Dictionary<int, List<int>> TestDictionary;
        
        public Dictionary<int, Pixel> Codes { get; private set; }

        public Dictionary<int, List<int>> DecodedDictionary;
        public int ClearPixel { get; } = -1;
        public int EndPixel { get; } = -2;
        public List<Pixel> SavedPixels { get; } = new List<Pixel>(100);
        
        private void InitTable <T> (T colors) where T : IList<Pixel>
        {
            DecodedDictionary = new Dictionary<int, List<int>>(10);
            Codes = new Dictionary<int, Pixel>(10);
            for (var color = 0; color < colors.Count; color++)
            {
                Codes[color] = colors[color];
                DecodedDictionary[color] = new List<int>{color};
            }

            DecodedDictionary[DecodedDictionary.Count] = new List<int>{ClearPixel};
            DecodedDictionary[DecodedDictionary.Count] = new List<int>{EndPixel};

        }
        
        public List<Pixel> Decode<T>(byte[] bytes, T colors, int minCodeSize) where T : IList<Pixel>
        {
            InitTable(colors);
            
            int initSize = minCodeSize;
            using (var writer = new BitReader(bytes.Reverse().ToArray()))
            {
                var bits = writer.ReadAll();
                var indexVal = new BitArray(bits.Skip(bits.Length - minCodeSize).Take(bits.Length).Reverse().ToArray());
                int[] data = new int[1];
                indexVal.CopyTo(data, 0);
                
                int dictionaryLastKeyIndex = DecodedDictionary.Count;
                int lastDictionaryFindIndex = data[0];
                int bitCursorPosition = bits.Length - minCodeSize;
                var indexHistory = new List<int>(10) {data[0]};
                
                while (!DecodedDictionary.ContainsKey(data[0]) || DecodedDictionary[data[0]][0] != EndPixel)
                {
                    if (data[0] < colors.Count + 2 && DecodedDictionary[data[0]][0] == ClearPixel)
                    {   
                        SavePixels(indexHistory);
                        indexHistory.Clear();
                        InitTable(colors);
                        minCodeSize = initSize;
                        dictionaryLastKeyIndex = DecodedDictionary.Count;
                    } 
                    else if (lastDictionaryFindIndex > colors.Count + 1 || DecodedDictionary[lastDictionaryFindIndex][0] != ClearPixel)
                    {
                        var lastFindIndexVal = DecodedDictionary[lastDictionaryFindIndex];
                        if (!DecodedDictionary.ContainsKey(data[0]))
                        {
                            DecodedDictionary[data[0]] = new List<int>(lastFindIndexVal);
                            DecodedDictionary[data[0]].Add(DecodedDictionary[data[0]][0]);
                        }
                        else
                        {
                            var newList = new List<int>(lastFindIndexVal) {DecodedDictionary[data[0]][0]};
                            DecodedDictionary[dictionaryLastKeyIndex] = newList;
                        }
                        dictionaryLastKeyIndex++;
                        if ((dictionaryLastKeyIndex & (dictionaryLastKeyIndex - 1)) == 0 && minCodeSize < 12)
                        {
                            minCodeSize++;
                        }
                    }

                    lastDictionaryFindIndex = data[0];
                    indexVal = new BitArray(bits.Skip(bitCursorPosition - minCodeSize).Take(minCodeSize).Reverse().ToArray());
                    indexVal.CopyTo(data, 0);
                    bitCursorPosition -= minCodeSize;
                    indexHistory.Add(data[0]);
                }

                SavePixels(indexHistory);
                return SavedPixels;
            }
        }

        public EncodedResult Encode<T>(T pixels, int paletteSize) where T : IList<Pixel>
        {
            var encodedResult = new EncodedResult();
            var uniquePixels = GifReader.GetUniqueColors(pixels);
            var indexTable = new Dictionary<Pixel, int>();
            if (uniquePixels.Count < paletteSize)
            {
                while (uniquePixels.Count != paletteSize)
                {
                    uniquePixels.Add(new Pixel {Red = 0, Green = 0, Blue = 0, Alpha = 0});
                }
            }
            int minCode = (int) Math.Ceiling(Math.Log(paletteSize, 2)) + 1;
            encodedResult.MinCodeSize = (byte) (minCode - 1);
            for (var pixel = 0; pixel < uniquePixels.Count; pixel++)
            {
                indexTable[uniquePixels[pixel]] = indexTable.ContainsKey(uniquePixels[pixel]) ? Math.Min(indexTable[uniquePixels[pixel]], pixel) : pixel;
            }
            var indexes = new int [pixels.Count];
            for (var pixel = 0; pixel < pixels.Count; pixel++)
            {
                indexes[pixel] = indexTable[pixels[pixel]];
            }
            InitTable(uniquePixels);
            Dictionary<string, int> stringIndexes = new Dictionary<string, int>();
            foreach (var keyValuePair in DecodedDictionary)
            {
                stringIndexes[$"[{keyValuePair.Key}]"] = keyValuePair.Key;
            }
            var clearIndex = DecodedDictionary.Count - 2;
            var endIndex = DecodedDictionary.Count - 1;
            
            var code = "";

            var indexesCodes = new List<int>(5);
            var bits = new List<bool>(100);
            AppendBits(bits, minCode, clearIndex);
            foreach (var index in indexes)
            {
                
                code += $"[{index}]";
                indexesCodes.Add(index);
                if (stringIndexes.ContainsKey(code)) continue;
                
                stringIndexes[code] = DecodedDictionary.Count;

                DecodedDictionary[DecodedDictionary.Count] = indexesCodes;
                AppendBits(bits, minCode, stringIndexes[code.Substring(0, code.Length - $"[{index}]".Length)]);
                if (isPowerOfTwo(DecodedDictionary.Count - 1))
                {
                    minCode++;
                }
                
                indexesCodes = new List<int> {indexesCodes[indexesCodes.Count - 1]};
                code = $"[{indexesCodes[0]}]";
                
                if (DecodedDictionary.Count == 3500)
                {
                    code = "";
                    stringIndexes.Clear();
                    indexesCodes.Clear();
                    
                    minCode = encodedResult.MinCodeSize + 1;
                    InitTable(uniquePixels);
                    AppendBits(bits, minCode, clearIndex);
                    foreach (var keyValuePair in DecodedDictionary)
                    {
                        stringIndexes[$"[{keyValuePair.Key}]"] = keyValuePair.Key;
                    }
                }
            }

            if (code.Length > 0)
            {
                AppendBits(bits, minCode, stringIndexes[code]);
            }
            AppendBits(bits, minCode, endIndex);

            var bitArray = new BitArray(bits.ToArray());
            byte[] byteRes = new byte[bits.Count / 8 + 1];
            
            bitArray.CopyTo(byteRes, 0);

            encodedResult.EncodedData = byteRes.ToList();
            File.WriteAllBytes("test.bin",byteRes);
            return encodedResult;

        }

        private void AppendBits(List<bool> bits, int code_size, int code)
        {
            for (int i = 0; i < code_size; i++) {
                bits.Add(((code >> i) & 0b1) == 1);
            }
        }
        
        private void SavePixels(List<int> indexHistory)
        {
            foreach (var index in indexHistory)
            {
                if (DecodedDictionary[index][0] == EndPixel || DecodedDictionary[index][0] == ClearPixel) continue;

                foreach (var colorIndex in DecodedDictionary[index])
                {
                    if (colorIndex < 0) continue;
                    SavedPixels.Add(Codes[colorIndex]);
                }
            }
        }
        public bool isPowerOfTwo(int val)
        {
            return (val & (val - 1)) == 0;
        }
    }
    
    static class StringExtensions {

        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength) {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            int i;
            for (i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

    }
}