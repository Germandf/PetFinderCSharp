using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetFinder.Helpers
{
    public class GenericResult
    {
        public List<string> Errors { get; set; }

        public GenericResult()
        {
            this.Errors = new List<string>();
        }

        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        public void AddRange(IEnumerable<string> range)
        {
            Errors.AddRange(range);
        }
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }
    }
    public class GenericResult<T>: GenericResult
    {
        public T value { get; set; }
    }
}
