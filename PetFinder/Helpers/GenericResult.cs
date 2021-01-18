using System.Collections.Generic;

namespace PetFinder.Helpers
{
    public class GenericResult
    {
        public GenericResult()
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; set; }

        public bool Success => Errors.Count == 0;

        public void AddRange(IEnumerable<string> range)
        {
            Errors.AddRange(range);
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }

    public class GenericResult<T> : GenericResult
    {
        public T value { get; set; }
    }
}