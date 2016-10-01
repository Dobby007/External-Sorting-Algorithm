using System;
using System.Collections.Generic;
using System.Linq;

namespace ExternalSorting
{
    internal class Merger : IRecordSource, IDisposable
    {
        private readonly List<SourceFile> _sourceFiles;

        public Merger(IEnumerable<TargetFile> targetFiles)
        {
            _sourceFiles = targetFiles.Select(targetFile => new SourceFile(targetFile)).ToList();
        }

        public void Dispose()
        {
            foreach (var file in _sourceFiles)
            {
                file.Dispose();
            }
        }

        public bool MoveToNextRunData()
        {
            var hasMoreRuns = false;
            foreach (var file in _sourceFiles)
            {
                if (file.MoveToNextRun())
                {
                    hasMoreRuns = true;
                }
            }
            return hasMoreRuns;
        }

        public long PredictedSize => _sourceFiles.Sum(file => file.FileSize);

        public bool HasMoreRecords => _sourceFiles.Any(file => file.HasMoreRecords);

        public Record Read()
        {
            Record minRecord = null;
            SourceFile minRecordSource = null;
            foreach (var file in _sourceFiles)
            {
                var topRecord = file.Peek();
                if (topRecord == null)
                {
                    continue;
                }
                if (minRecord == null || minRecord.CompareTo(topRecord) > 0)
                {
                    minRecord = topRecord;
                    minRecordSource = file;
                }
            }
            return minRecordSource?.Pop();
        }


        private class SourceFile : IDisposable
        {
            private readonly FileReader _reader;
            private Record _topRecord;

            public SourceFile(TargetFile targetFile)
            {
                _reader = new FileReader(targetFile.TargetFilePath);
            }

            public bool HasMoreRecords => !_reader.EndOfFile;

            public long FileSize => _reader.TotalSizeInBytes;

            public void Dispose()
            {
                _reader?.Dispose();
            }

            public bool MoveToNextRun()
            {
                return _reader.MoveToNextRun();
            }

            public Record Peek()
            {
                if (_topRecord == null)
                {
                    _topRecord = ExtractTop();
                }
                return _topRecord;
            }

            public Record Pop()
            {
                if (_topRecord != null)
                {
                    var resultRecord = _topRecord;
                    _topRecord = ExtractTop();
                    return resultRecord;
                }
                else
                {
                    var resultRecord = ExtractTop();
                    _topRecord = ExtractTop();
                    return resultRecord;
                }
            }

            private Record ExtractTop()
            {
                return _reader.Read();
            }
        }
    }
}