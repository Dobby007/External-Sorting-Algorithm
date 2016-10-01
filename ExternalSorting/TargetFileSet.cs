using System.Collections;
using System.Collections.Generic;

namespace ExternalSorting
{
    internal sealed class TargetFileSet : IEnumerable<TargetFile>
    {
        private readonly List<TargetFile> _files = new List<TargetFile>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TargetFile> GetEnumerator()
        {
            return ((IEnumerable<TargetFile>) _files).GetEnumerator();
        }


        public void Add(TargetFile file)
        {
            _files.Add(file);
        }
    }
}