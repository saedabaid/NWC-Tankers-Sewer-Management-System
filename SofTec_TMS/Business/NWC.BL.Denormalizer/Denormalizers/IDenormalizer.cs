using NWC.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer.Denormalizers
{
    public interface IDenormalizer
    {
        DescriptiveResponse<Boolean> DenormalizeStates(List<long> eventIDs);
    }
}
