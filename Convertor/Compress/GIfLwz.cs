using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Converter;
using LZW;

namespace Convertor.Compress
{
    public class GIfLwz
    {
        public Dictionary<int, Pixel> Codes { get; private set; }
        
        public Dictionary<int, List<int>> DecodedDictionary = new Dictionary<int, List<int>>(10);
        public int ClearPixel { get; } = -1;

        public int EndPixel { get; } = -2;
        
        private void InitTable <T> (T colors) where T : IList<Pixel>
        {
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
            
            using (var writer = new BitReader(bytes.Reverse().ToArray()))
            {
                var bits = writer.ReadAll();
                var indexVal = new BitArray(bits[Range.StartAt(bits.Length - minCodeSize)].Reverse().ToArray());
                int[] data = new int[1];
                indexVal.CopyTo(data, 0);
                
                int dictionaryLastKeyIndex = DecodedDictionary.Count;
                int lastDictionaryFindIndex = data[0];
                int bitCursorPosition = bits.Length - minCodeSize;
                var indexHistory = new List<int>(10) {data[0]};
                
                while (!DecodedDictionary.ContainsKey(data[0]) || DecodedDictionary[data[0]][0] != EndPixel)
                {
                    if (DecodedDictionary[lastDictionaryFindIndex][0] != ClearPixel)
                    {
                        var lastFindIndexVal = DecodedDictionary[lastDictionaryFindIndex];
                        if (!DecodedDictionary.ContainsKey(data[0]))
                        {
                            DecodedDictionary[data[0]] = new List<int>{ lastFindIndexVal[0] };
                            DecodedDictionary[data[0]].AddRange(DecodedDictionary[data[0]]);
                        }
                        else
                        {
                            var newList = new List<int>();
                            newList.AddRange(lastFindIndexVal);
                            newList.Add(DecodedDictionary[data[0]][0]);
                            DecodedDictionary[dictionaryLastKeyIndex] = newList;
                        }
                        dictionaryLastKeyIndex++;
                        if ((dictionaryLastKeyIndex & (dictionaryLastKeyIndex - 1)) == 0 && minCodeSize != 12)
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

                var pixels = new List<Pixel>(10);
                foreach (var index in indexHistory)
                {
                    if (DecodedDictionary[index][0] == EndPixel || DecodedDictionary[index][0] == ClearPixel) continue;

                    foreach (var colorIndex in DecodedDictionary[index])
                    {
                        if (colorIndex < 0) continue;
                        pixels.Add(Codes[colorIndex]);
                    }
                }
                return pixels;
            }
        }

        public void Encode<T>(int[] indexes, T colors, int minCodeSize) where T : IList<Pixel>
        {
            InitTable(colors);
            foreach (var index in indexes)
            {
                var firstNotSecond = DecodedDictionary[index].Except(list2).ToList();
                var secondNotFirst = list2.Except(list1).ToList();
            }
        }
    }
}