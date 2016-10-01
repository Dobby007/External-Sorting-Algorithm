using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace ExternalSorting
{
    internal sealed class FileReader : IDisposable
    {
        private const int BatchSize = 100000;
        private readonly ManualResetEvent _moveToNextRunEvent;
        private readonly ManualResetEvent _pauseReadingEvent;
        private readonly StreamReader _reader;
        private volatile bool _pauseReading;
        private Thread _readingThread;

        public FileReader(string filePath)
        {
            var fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 18, FileOptions.SequentialScan);
            TotalSizeInBytes = fstream.Length;
            _reader = new StreamReader(fstream);
            _pauseReadingEvent = new ManualResetEvent(true);
            _moveToNextRunEvent = new ManualResetEvent(true);
            Queue = new BlockingCollection<Record>(BatchSize*2);
            StartReading();
        }

        public long TotalSizeInBytes { get; private set; }

        public bool EndOfFile
        {
            get { return Queue.IsAddingCompleted; }
        }

        private BlockingCollection<Record> Queue { get; }

        public void Dispose()
        {
            _reader?.Dispose();
            _pauseReadingEvent?.Dispose();
            _readingThread?.Abort();
        }

        public bool MoveToNextRun()
        {
            if (!EndOfFile)
            {
                _moveToNextRunEvent.Set();
                return true;
            }
            return false;
        }

        private void StartReading()
        {
            if (_readingThread != null)
            {
                return;
            }

            _readingThread = new Thread(() =>
            {
                while (true)
                {
                    WaitHandle.WaitAll(new WaitHandle[] {_moveToNextRunEvent, _pauseReadingEvent});
                    for (var i = 0; i < BatchSize; i++)
                    {
                        var line = _reader.ReadLine();

                        if (line == "===")
                        {
                            _moveToNextRunEvent.Reset();
                            break;
                        }

                        if (string.IsNullOrEmpty(line))
                        {
                            if (_reader.EndOfStream)
                            {
                                break;
                            }
                            continue;
                        }
                        
                        var separatorPos = line.IndexOf(". ", StringComparison.InvariantCulture);
                        // ReSharper disable once MethodSupportsCancellation
                        Queue.Add(new Record(
                            uint.Parse(line.Substring(0, separatorPos)),
                            line.Substring(separatorPos + 2)
                            ));
                    }

                    if (_reader.EndOfStream)
                    {
                        break;
                    }
                }
                
                if (_reader.EndOfStream)
                    Queue.CompleteAdding();
            });
            _readingThread.Priority = ThreadPriority.AboveNormal;
            _readingThread.Start();
        }

        public void ResumeReading()
        {
            _pauseReadingEvent.Set();
            _pauseReading = false;
        }

        public void PauseReading()
        {
            _pauseReadingEvent.Reset();
            _pauseReading = true;
        }

        public Record Read()
        {
            if (!Queue.IsCompleted)
            {
                try
                {
                    var spinWait = new SpinWait();
                    Record record = null;
                    while (!Queue.IsCompleted && !Queue.TryTake(out record))
                    {
                        if (_pauseReading || !_moveToNextRunEvent.WaitOne(0))
                        {
                            return null;
                        }
                        spinWait.SpinOnce();
                    }

                    return record;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
            return null;
        }
    }
}