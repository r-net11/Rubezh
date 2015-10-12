using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.SKD;

namespace GKWebService.Models
{
    public class ShortEmployeeModel
    {
        public Guid UID { get; set; }
        public Guid OrganisationUID { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public int Level { get; set; }
        public bool IsLeaf { get; set; }
        public bool IsOrganisation { get; set; }
        public bool IsExpanded { get; set; }
        public bool IsDeleted { get; set; }
        public string RemovalDate { get; set; }

        public static ShortEmployeeModel CreateFromOrganisation(Organisation organisation)
        {
            var model = new ShortEmployeeModel();
            model.InitializeOrganisation(organisation);
            return model;
        }

        public static ShortEmployeeModel CreateFromModel(ShortEmployee employee, IEnumerable<ShortEmployeeModel> organisations)
        {
            var model = new ShortEmployeeModel();
            model.InitializeModel(employee, organisations);
            return model;
        }

        private void InitializeOrganisation(Organisation organisation)
        {
            UID = organisation.UID;
            Name = organisation.Name;
            IsOrganisation = true;
            IsExpanded = true;
            IsDeleted = organisation.IsDeleted;
            RemovalDate = IsDeleted ? organisation.RemovalDate.ToString("d MMM yyyy") : "";
            Level = 0;
            IsLeaf = true;
        }

        private void InitializeModel(ShortEmployee employee, IEnumerable<ShortEmployeeModel> organisations)
        {
            UID = employee.UID;
            OrganisationUID = employee.OrganisationUID;
            IsOrganisation = false;
            Name = employee.FIO;
            DepartmentName = employee.DepartmentName;
            Level = 1;
            IsLeaf = true;
            IsExpanded = false;
            var organisation = organisations.FirstOrDefault(o => o.UID == employee.OrganisationUID);
            if (organisation != null)
            {
                organisation.IsLeaf = false;
            }
        }
    }
}