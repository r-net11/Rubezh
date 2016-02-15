using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Employees
{
    public class EmployeeDetailsViewModel
    {
        public Employee Employee { get; set; }

        public string PhotoData { get; set; }

        public void Initialize(Guid? id)
        {
            if (id.HasValue)
            {
                var operationResult = EmployeeHelper.GetDetails(id.Value);
                Employee = operationResult;
            }
            else
            {
                Employee = new Employee();
                Employee.BirthDate = DateTime.Now;
                Employee.CredentialsStartDate = DateTime.Now;
                Employee.DocumentGivenDate = DateTime.Now;
                Employee.DocumentValidTo = DateTime.Now;
                Employee.RemovalDate = DateTime.Now;
                Employee.ScheduleStartDate = DateTime.Now;
            }
            if (Employee.Photo != null && Employee.Photo.Data != null)
            {
                PhotoData = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(Employee.Photo.Data));
                Employee.Photo.Data = null;
            }
            Employee.AdditionalColumns.ForEach(c => c.Photo = null);
        }

        public bool Save(bool isNew)
        {
            if ((PhotoData != null && PhotoData.Length > 0) || Employee.Photo != null)
            {
                Employee.Photo = new Photo();
                byte[] data = null;
                if (PhotoData != null)
                {
                    data = Convert.FromBase64String(PhotoData.Remove(0, "data:image/gif;base64,".Length));
                }
                Employee.Photo.Data = data;
            }
            var operationResult = EmployeeHelper.Save(Employee, isNew);
            return operationResult;
        }
    }
}