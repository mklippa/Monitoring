using System.Collections.Generic;

namespace Emulator.Models
{
    public class Report
    {
        public Report(IEnumerable<Error> errors)
        {
            Errors = errors;
        }

        public IEnumerable<Error> Errors { get; }
    }
}