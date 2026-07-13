using NWC.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer
{
    public interface IStateDenormalizer
    {
        DescriptiveResponse<Boolean> ApplyEvent(long eventID, DescriptiveResponse<Boolean> result);
    }
}
