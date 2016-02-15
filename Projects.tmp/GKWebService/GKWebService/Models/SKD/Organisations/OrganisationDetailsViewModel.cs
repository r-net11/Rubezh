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

        public bool IsChiefSelected { get; set; }

        public ShortEmployeeModel SelectedChief { get; set; }

        public bool IsHRChiefSelected { get; set; }

        public ShortEmployeeModel SelectedHRChief { get; set; }

        public string PhotoData { get; set; }

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
                PhotoData = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(Organisation.Photo.Data));
                Organisation.Photo.Data = null;
            }

            var filter = new EmployeeFilter { LogicalDeletationType = LogicalDeletationType.All, UIDs = new List<Guid> { Organisation.ChiefUID }, IsAllPersonTypes = true };
            var chiefOperationResult = EmployeeHelper.Get(filter);
            IsChiefSelected = chiefOperationResult.Any();
            SelectedChief = chiefOperationResult.Select(e => ShortEmployeeModel.CreateFromModel(e)).FirstOrDefault() ?? new ShortEmployeeModel();

            filter.UIDs = new List<Guid> {Organisation.HRChiefUID};
            var hrChiefOperationResult = EmployeeHelper.Get(filter);
            IsHRChiefSelected = hrChiefOperationResult.Any();
            SelectedHRChief = hrChiefOperationResult.Select(e => ShortEmployeeModel.CreateFromModel(e)).FirstOrDefault() ?? new ShortEmployeeModel();
        }

        public bool Save(bool isNew)
        {
            if (IsChiefSelected)
            {
                Organisation.ChiefUID = SelectedChief.UID;
            }
            if (IsHRChiefSelected)
            {
                Organisation.HRChiefUID = SelectedHRChief.UID;
            }

            if ((PhotoData != null && PhotoData.Length > 0) || Organisation.Photo != null)
            {
                Organisation.Photo = new Photo();
                byte[] data = null;
                if (PhotoData != null)
                {
                    data = Convert.FromBase64String(PhotoData.Remove(0, "data:image/gif;base64,".Length));
                }
                Organisation.Photo.Data = data;
            }
            var result = OrganisationHelper.Save(Organisation, isNew);
            return result;
        }
    }
}