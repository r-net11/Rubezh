using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infrastructure.Common.TreeList;
using RubezhAPI.SKD;

namespace GKWebService.Models.SKD.Common
{
    public class OrganisationElementViewModel<T, ModelT>
        where ModelT : class, IOrganisationElement, new()
    {
        public ModelT Model { get; set; }
        public Organisation Organisation { get; set; }
        public bool IsOrganisation { get; set; }
        public bool IsDeleted { get; set; }
        public Guid UID { get; set; }
        public Guid OrganisationUID { get; set; }
        public string RemovalDate { get; set; }
        public string Name { get; set; }
        public string NameData { get; set; }
        public string Description { get; set; }
        public bool IsExpanded { get; set; }
        public int Level { get; set; }
        public Guid ParentUID { get; set; }
        public bool IsLeaf { get; set; }

        public virtual void InitializeOrganisation(Organisation organisation)
        {
            Organisation = organisation;
            OrganisationUID = organisation.UID;
            UID = organisation.UID;
            Name = organisation.Name;
            NameData = organisation.Name;
            Description = organisation.Description;
            IsOrganisation = true;
            IsExpanded = true;
            IsDeleted = organisation.IsDeleted;
            RemovalDate = IsDeleted ? organisation.RemovalDate.ToString("d MMM yyyy") : "";
        }

        public virtual void InitializeModel(Organisation organisation, ModelT model)
        {
            Organisation = organisation;
            OrganisationUID = model.OrganisationUID;
            Model = model;
            UID = model.UID;
            Name = model.Name;
            NameData = model.Name;
            Description = model.Description;
            IsOrganisation = false;
            IsDeleted = model.IsDeleted;
            RemovalDate = IsDeleted ? model.RemovalDate.ToString("d MMM yyyy") : "";
        }
    }
}