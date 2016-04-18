using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using GKWebService.Models.SKD.Common;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Organisations
{
    public class OrganisationDetailsViewModel
    {
        public OrganisationDetails Organisation { get; set; }

        public ShortEmployeeModel SelectedChief { get; set; }

        public ShortEmployeeModel SelectedHRChief { get; set; }

        public string photoData { get; set; }

        public void Initialize(Guid? id)
        {
            var isNew = id == null;
            if (isNew)
            {
                Organisation = new OrganisationDetails()
                {
                    Name = "Организация"
                };
                Organisation.UserUIDs.Add(ClientManager.CurrentUser.UID);

            }
            else
            {
                Organisation = OrganisationHelper.GetDetails(id.Value);
            }

            if (Organisation.Photo != null && Organisation.Photo.Data != null)
            {
                photoData = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(Organisation.Photo.Data));
                Organisation.Photo.Data = null;
            }

            var filter = new EmployeeFilter { LogicalDeletationType = LogicalDeletationType.All, UIDs = new List<Guid> { Organisation.ChiefUID }, IsAllPersonTypes = true };
            var chiefOperationResult = EmployeeHelper.Get(filter);
            SelectedChief = chiefOperationResult.Select(e => ShortEmployeeModel.CreateFromModel(e)).FirstOrDefault();

            filter.UIDs = new List<Guid> {Organisation.HRChiefUID};
            var hrChiefOperationResult = EmployeeHelper.Get(filter);
            SelectedHRChief = hrChiefOperationResult.Select(e => ShortEmployeeModel.CreateFromModel(e)).FirstOrDefault();
        }

        public bool Save(bool isNew)
        {
            if (SelectedChief != null)
            {
                Organisation.ChiefUID = SelectedChief.UID;
            }
            if (SelectedHRChief != null)
            {
                Organisation.HRChiefUID = SelectedHRChief.UID;
            }

            if ((photoData != null && photoData.Length > 0) || Organisation.Photo != null)
            {
                Organisation.Photo = new Photo();
                byte[] data = null;
                if (photoData != null)
                {
                    data = Convert.FromBase64String(photoData.Remove(0, "data:image/gif;base64,".Length));
                }
                Organisation.Photo.Data = data;
            }
            var result = OrganisationHelper.Save(Organisation, isNew);
            return result;
        }
    }
}