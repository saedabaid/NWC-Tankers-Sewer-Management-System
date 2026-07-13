using NWC.DAL.NWCEntities;
using NWC.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer
{
    public class StateHolder
    {
        public ConcurrentDictionary<long, ReturnResult> StateHolders { get; private set; }

        public StateHolder()
        {
            StateHolders = new ConcurrentDictionary<long, ReturnResult>();
        }

        //public StateHolder AddIncidentState(ReturnResult result)
        //{
        //    StateIncident oldState = null;

        //    if (StateHolders.TryGetValue(newState.ActivityId, out oldState))
        //        StateHolders[newState.ActivityId] = newState;
        //    else
        //        StateHolders.TryAdd(newState.ActivityId, newState);

        //    return this;

        //}
    }
}
