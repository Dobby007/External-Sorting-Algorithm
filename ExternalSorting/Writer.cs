using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExternalSorting
{
    internal sealed class Writer : IDisposable
    {
        private int _currentTarget;
        private readonly List<Target> _targets = new List<Target>();

        public Writer(IRecordSource recordSource)
        {
            RecordSource = recordSource;
        }

        public IRecordSource RecordSource { get; set; }

        public int TotalRunsCount
        {
            get { return _targets.Sum(target => target.RunsCount); }
        }

        public void Dispose()
        {
            foreach (var target in _targets)
            {
                target.Dispose();
            }
        }

        public void AddTarget(TargetFile targetFile)
        {
            _targets.Add(new Target(targetFile));
        }

        public List<TargetFile> GetUsedTargetFiles()
        {
            return _targets.Where(target => target.RunsCount > 0)
                .Select(target => target.TargetFile).ToList();
        }

        public long Write()
        {
            if (_targets.Count < 1)
            {
                throw new IOException("No targets were set");
            }
            var writeCount = 0L;
            while (RecordSource.MoveToNextRunData())
            {
                var target = _targets[_currentTarget];
                var writer = target.BeginWrite();

                Record record;
                while ((record = RecordSource.Read()) != null)
                {
                    writer.Write(record.Number + ". ");
                    writer.WriteLine(record.Text);
                    writeCount += 1;
                }
                target.EndWrite();

                _currentTarget++;
                if (_currentTarget >= _targets.Count)
                {
                    _currentTarget = 0;
                }
            }
            return writeCount;
        }


        private class Target : IDisposable
        {
            public Target(TargetFile targetFile)
            {
                TargetFile = targetFile;
                var stream = new FileStream(targetFile.TargetFilePath, FileMode.Create, FileAccess.Write,
                    FileShare.None, 1 << 18, FileOptions.SequentialScan);
                Writer = new StreamWriter(stream);
                RunsCount = 0;
            }

            public int RunsCount
            {
                get { return TargetFile.RunsCount; }
                private set { TargetFile.RunsCount = value; }
            }

            private TextWriter Writer { get; }
            public TargetFile TargetFile { get; }

            public void Dispose()
            {
                Writer?.Close();
            }


            public TextWriter BeginWrite()
            {
                if (RunsCount >= 1)
                {
                    Writer.WriteLine("===");
                }
                return Writer;
            }

            public void EndWrite()
            {
                RunsCount++;
            }
        }
    }
}