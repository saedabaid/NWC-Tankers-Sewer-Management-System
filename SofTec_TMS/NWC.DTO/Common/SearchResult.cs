using System.Collections.Generic;

namespace NWC.DTO.Common
{
    public class SearchResult<T>
    {
        public List<T> Result { get; set; }
        public int TotalCount { get; set; }

    }
}
