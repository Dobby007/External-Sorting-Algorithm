using System;
using System.Linq;
using System.Threading;

namespace ExternalSorting
{
    internal class SortAlgorithm
    {
        private TargetFileSet _inputTargetFileSet;
        private TargetFileSet _outputTargetFileSet;
        private readonly string _sourceFilePath;

        public SortAlgorithm(string sourceFilePath)
        {
            _sourceFilePath = sourceFilePath;
            _outputTargetFileSet = new TargetFileSet {new TargetFile("tb1"), new TargetFile("tb2")};
            _inputTargetFileSet = new TargetFileSet {new TargetFile("ta1"), new TargetFile("ta2")};
        }

        public void Run()
        {
            MemoryInfo.Reset();
            using (var sorter = new Sorter(new FileReader(_sourceFilePath)))
            using (var writer = new Writer(sorter))
            {
                foreach (var targetFile in GetOutputTargetFileSet())
                {
                    writer.AddTarget(targetFile);
                }
                var writtenCount = writer.Write();
                Console.WriteLine($"{string.Join(", ", GetOutputTargetFileSet().Select(file => file.TargetFilePath))}: Total number of written records: {writtenCount}");
            }

            GC.Collect();
            while (GetOutputTargetFileSet().Sum(file => file.RunsCount) > 1)
            {
                SwitchTargetRoles();
                using (var mrg = new Merger(GetInputTargetFileSet()))
                using (var writer = new Writer(mrg))
                {
                    foreach (var targetFile in GetOutputTargetFileSet())
                    {
                        writer.AddTarget(targetFile);
                    }
                    var writtenCount = writer.Write();
                    Console.WriteLine($"Total number of written records: {writtenCount}");
                }
            }

            var finalOutputFile = GetOutputTargetFileSet().FirstOrDefault(file => file.RunsCount == 1) ??
                                  GetOutputTargetFileSet().First();

            finalOutputFile.MakeFinal();
        }

        private TargetFileSet GetInputTargetFileSet()
        {
            return _inputTargetFileSet;
        }

        private TargetFileSet GetOutputTargetFileSet()
        {
            return _outputTargetFileSet;
        }

        private void SwitchTargetRoles()
        {
            _outputTargetFileSet = Interlocked.Exchange(ref _inputTargetFileSet, _outputTargetFileSet);
        }
    }
}