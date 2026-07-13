using Infrastructure;
using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC_CCB_Integration.DTO.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer.Denormalizers
{
    public class Denormalizer : IDenormalizer
    {
        public List<Tuple<Func<NWC_Event, bool>, Func<NWC_Event, ReturnResult, ReturnResult>>> StateFunctions;
        //private NWC.BLL.Services.LoggedInUserService loggedInService;

        #region Constructors
        public Denormalizer()
        {

        }

        //public Denormalizer(NWC.BLL.Services.LoggedInUserService loggedInService)
        //{
        //    this.loggedInService = loggedInService;
        //}
        #endregion

        public DescriptiveResponse<Boolean> DenormalizeStates(List<long> eventIDs)
        {
            var result = new DescriptiveResponse<Boolean>();

            try
            {
                eventIDs.ForEach(e =>
                {
                    var taskList =
                    GetDenormalizers().Select(d =>
                    {
                        return Task.Run(() =>
                        {
                            result = d.ApplyEvent(e, result);
                        });
                    }).
                    ToArray();

                    Task.WaitAll(taskList);
                });
                return result;
            }
            catch (Exception ex)
            {
                LoggerManager.LogMsg(c => c.Log(ex, "Denormalizer => DenormalizeStates: "));
                return null;
            }
        }

        private List<IStateDenormalizer> GetDenormalizers()
        {
            var Denormalizers = new List<IStateDenormalizer>();

            Denormalizers.Add(new StateWorkOrder());
            //Denormalizers.Add(new StateWorkOrderVehicle());
            Denormalizers.Add(new StateVehicle());

            return Denormalizers;
        }
    }
}
