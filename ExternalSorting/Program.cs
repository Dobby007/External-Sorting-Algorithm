using System;
using System.Diagnostics;

namespace ExternalSorting
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(
                    "You need to provide the path to the input file as a first argument. E.g.: prog.exe input.txt.");
                return;
            }

            var filePath = args[0];
            try
            {
                var watch = new Stopwatch();
                watch.Start();
                new SortAlgorithm(filePath).Run();
                Console.WriteLine($"File was sorted in {watch.Elapsed.TotalSeconds} seconds");
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }
    }
}