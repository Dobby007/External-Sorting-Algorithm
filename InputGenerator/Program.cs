using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InputGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            const string loremIpsum =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            var strArray = new[]
            {
                "Something something something",
                "Cherry is the best",
                "Banana is yellow",
                "Apple",
                "Blueberry is better than the best. It's a fact. Nobody can argue with that",
                "Strawberry is very delicious",
                "Pineapple is sweet and tasty",
                "What you see is what you get"
            };
            if (args.Length < 2)
            {
                Console.WriteLine("Not enough arguments. Provide the filename as a first argument and its size in gigabytes as a second. E.g.: InputGenerator.exe input.txt 8 or InputGenerator.exe input.txt 4 or InputGenerator.exe input.txt 0.2");
                return;
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            var fileName = args[0];
            var fileSize = double.Parse(args[1]) * 1024L * 1024 * 1024;
            var sqlMode = args.Length >= 3 && args[2] == "sql";
            
            var randomInt = new Random();

            long totalBytes = 0;
            using (var writer = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192)))
            {
                while (totalBytes < fileSize)
                {
                    var str = strArray[randomInt.Next(strArray.Length)];
                    var number = randomInt.Next(100);
                    var fileStr = !sqlMode 
                        ? string.Concat(number, ". ", str, ": ", loremIpsum)
                        : string.Concat("INSERT INTO sorting VALUES(", number, ", '", str.Replace("'", "''"), ": ", loremIpsum, "')");
                    writer.WriteLine(fileStr);
                    totalBytes += fileStr.Length + 2;
                }
                
            }
        }
    }
}

