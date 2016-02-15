using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using GKWebService.Models.SKD.Common;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Positions
{
    public class PositionsViewModel : OrganisationBaseViewModel<ShortPosition, PositionFilter, PositionViewModel>
    {
        protected override IEnumerable<ShortPosition> GetModels(PositionFilter filter)
        {
            return PositionHelper.Get(filter);
        }
    }
}