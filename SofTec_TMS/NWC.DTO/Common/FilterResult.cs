using System.Collections.Generic;

namespace NWC.DTO.Common
{
    public class FilterResult<T>
    {
        public List<T> FirstResult { get; set; }
        public List<T> SecondResult { get; set; }

        public int TotalCount { get; set; }

    }
}
