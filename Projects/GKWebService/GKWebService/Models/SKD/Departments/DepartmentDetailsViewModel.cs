using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using GKWebService.Models.SKD.Common;
using GKWebService.Utils;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Departments
{
    public class DepartmentDetailsViewModel
    {
        public Department Department { get; set; }

        public ShortDepartment SelectedDepartment { get; set; }

        public ShortEmployeeModel SelectedChief { get; set; }

        public string photoData { get; set; }

        public void Initialize(Guid? organisationId, Guid? id, Guid? parentDepartmentId)
        {
            if (id.HasValue)
            {
                var departmentDetailsResult = DepartmentHelper.GetDetails(id.Value);
                Department = departmentDetailsResult;
            }
            else
            {
                Department = new Department
                {
                    Name = "Новое подразделение",
                    ParentDepartmentUID = parentDepartmentId ?? Guid.Empty,
                    OrganisationUID = organisationId.Value
                };
            }

            var filter = new DepartmentFilter();
            filter.UIDs.Add(Department.ParentDepartmentUID);
            var departmentListResult = DepartmentHelper.Get(filter);
            SelectedDepartment = departmentListResult.FirstOrDefault();

            var employeeFilter = new EmployeeFilter { LogicalDeletationType = LogicalDeletationType.All, UIDs = new List<Guid> { Department.ChiefUID }, IsAllPersonTypes = true };
            var chiefResult = EmployeeHelper.Get(employeeFilter);
            SelectedChief = chiefResult.Select(e => ShortEmployeeModel.CreateFromModel(e)).FirstOrDefault();

            if (Department.Photo != null && Department.Photo.Data != null)
            {
                photoData = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(Department.Photo.Data));
                Department.Photo.Data = null;
            }
        }

        public bool Save(bool isNew)
        {
            string error = DetailsValidateHelper.Validate(Department);

            if (!string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException(error);
            }

            Department.ParentDepartmentUID = (SelectedDepartment != null) ? SelectedDepartment.UID : Guid.Empty;
            Department.ChiefUID = (SelectedChief != null) ? SelectedChief.UID : Guid.Empty;

            if ((photoData != null && photoData.Length > 0) || Department.Photo != null)
            {
                Department.Photo = new Photo();
                byte[] data = null;
                if (photoData != null)
                {
                    data = Convert.FromBase64String(photoData.Remove(0, "data:image/gif;base64,".Length));
                }
                Department.Photo.Data = data;
            }

            var operationResult = DepartmentHelper.Save(Department, isNew);

            return operationResult;
        }

		public static void Paste(List<DepartmentDetailsViewModel> departments)
		{
			foreach (var department in departments)
			{
				var childDepartments = departments.Where(d => d.Department.ParentDepartmentUID == department.Department.UID).ToList();
				department.Department.UID = Guid.NewGuid();
				childDepartments.ForEach(d => d.SelectedDepartment.UID = department.Department.UID);
				department.Department.ChildDepartmentUIDs = new List<Guid>();
				department.Save(true);
			}
		}
	}
}