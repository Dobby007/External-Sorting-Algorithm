using System;

namespace ExternalSorting
{
    internal class Sorter : IRecordSource, IDisposable
    {
        private static readonly int BatchSize = 100000;
        private readonly FileReader _reader;
        private BinaryHeap<Record> _runRecords;

        public Sorter(FileReader reader)
        {
            _reader = reader;
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }

        public bool MoveToNextRunData()
        {
            return ReadNextRunData();
        }

        public bool HasMoreRecords => !_reader.EndOfFile;

        public Record Read()
        {
            if (_runRecords == null)
            {
                ReadNextRunData();
            }
            if (_runRecords.Count > 0)
            {
                return _runRecords.RemoveRoot();
            }
            return null;
        }

        private bool ReadNextRunData()
        {
            _runRecords = new BinaryHeap<Record>((int) _reader.TotalSizeInBytes/512);
            GC.Collect();

            _reader.ResumeReading();
            while (true)
            {
                for (var i = 0; i < BatchSize; i++)
                {
                    var record = _reader.Read();

                    // we can receive null if reading was ended or it was explicitly paused
                    if (record == null)
                    {
                        goto EndReading;
                    }
                    _runRecords.Insert(record);
                }

                if (
                    MemoryInfo.GetOccupiedMemoryPercent() >= 0.8 && 
                    MemoryInfo.GetFreeMemoryLeft() < 1024 * 1024 * 1024
                )
                {
                    _reader.PauseReading();
                }
            }

            EndReading:
            if (_runRecords.Count < 1)
            {
                return false;
            }
            return true;
        }
    }
}