
using System;
using System.IO;

namespace Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArgs = ArgParser.Parse(args);

            IImageReader reader;
            IImageWriter writer;
            
            if (!parsedArgs.ContainsKey("source") || !parsedArgs.ContainsKey("goal-format"))
            {
                var sourceErrString = parsedArgs.ContainsKey("source") ? "" : "--source";
                var goalFormatErrString = parsedArgs.ContainsKey("goal-format") ? "" : "--goal-format";
                
                throw new Exception($"Args not contains {sourceErrString} {goalFormatErrString}");
            }
            
            var sourceName = Path.GetFileName(parsedArgs["source"]).Split('.');
            if (sourceName.Length < 2) throw new Exception("Source must have extension");
            if (!File.Exists(parsedArgs["source"]))
            {
                throw new Exception("File not exists!");
            }
            switch (sourceName[1])
            {
                case "png":
                    reader = new PngReader();
                    break;
                case "ppm":
                    reader = new PpmReader();
                    break;
                case "gif":
                    reader = new GifReader();
                    break;
                case "bmp":
                    reader = new BmpReader();
                    break;
                default:
                    throw new Exception($"Format {sourceName[1]} not supported!");
            }

            var goalFormat = parsedArgs["goal-format"];
            switch (goalFormat)
            {
                case "png":
                    writer = new PngWriter();
                    break;
                case "ppm":
                    writer = new PpmWriter();
                    break;
                case "bmp":
                    writer = new BmpWriter();
                    break;
                default:
                    throw new Exception($"Format {goalFormat} not supported!");
            }

            var image = reader.ReadImage(parsedArgs["source"]);
            writer.WriteImage(image, parsedArgs.ContainsKey("output") ? parsedArgs["output"] : $"./convertedImage.{goalFormat}");
            Console.WriteLine("Done");
        }
    }
}