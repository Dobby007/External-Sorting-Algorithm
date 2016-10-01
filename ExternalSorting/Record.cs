using System;

namespace ExternalSorting
{
    internal sealed class Record : IComparable<Record>
    {
        public Record(uint number, string text)
        {
            Number = number;
            Text = text;
        }

        public uint Number { get; }
        public string Text { get; }

        public int CompareTo(Record other)
        {
            var compareResult = string.Compare(Text, other.Text, StringComparison.InvariantCulture);
            if (compareResult == 0)
            {
                return Number.CompareTo(other.Number);
            }
            return compareResult;
        }

        public override string ToString()
        {
            return string.Concat(Number.ToString(), ". ", Text);
        }
    }
}