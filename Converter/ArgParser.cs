using System;
using System.Collections.Generic;

namespace Converter
{
    public class ArgParser
    {
        public static Dictionary<string, string> Parse(string[] args)
        {
            var result = new Dictionary<string, string>(4) ;

            if (args.Length < 4)
            {
                throw new Exception("You must pass at least 2 args --source and --goal-format");
            }
            
            for (var arg = 0; arg < args.Length; arg+=2)
            {
                if (args[arg] == "--source")
                {
                    if (args[arg + 1] == "--goal-format" || args[arg + 1] == "--output")
                    {
                        throw new Exception("Wrong input");
                    }

                    result["source"] = args[arg + 1].ToLower();
                }

                if (args[arg] == "--output")
                {
                    if (args[arg + 1] == "--goal-format" || args[arg + 1] == "--source")
                    {
                        throw new Exception("Wrong input");
                    }
                    result["output"] = args[arg + 1];
                }
                
                if (args[arg] == "--goal-format")
                {
                    if (args[arg + 1] == "--output" || args[arg + 1] == "--source")
                    {
                        throw new Exception("Wrong input");
                    }
                    result["goal-format"] = args[arg + 1].ToLower();
                }
            }
            return result;
        }
    }
}