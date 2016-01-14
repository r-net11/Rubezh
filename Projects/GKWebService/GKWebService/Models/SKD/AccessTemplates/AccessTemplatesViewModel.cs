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
    public class AccessTemplatesViewModel : OrganisationBaseViewModel<AccessTemplate, AccessTemplateFilter, AccessTemplateViewModel>
    {
        protected override IEnumerable<AccessTemplate> GetModels(AccessTemplateFilter filter)
        {
            return AccessTemplateHelper.Get(filter);
        }
    }
}