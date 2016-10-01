namespace ExternalSorting
{
    internal interface IRecordSource
    {
        bool HasMoreRecords { get; }
        bool MoveToNextRunData();
        Record Read();
    }
}