using System;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationElementViewModel<T, ModelT> : TreeNodeViewModel<T>, IOrganisationElementViewModel
		where T : TreeNodeViewModel<T>
		where ModelT : class, IOrganisationElement, new()
	{
		public Organisation Organisation { get; protected set; }
		public ModelT Model { get; protected set; }
		public bool IsOrganisation { get; protected set; }

		public string Name
		{
			get
			{
				if (IsOrganisation)
					return Organisation.Name;
				else
					return Model.Name;
			}
		}
		public string Description
		{
			get
			{
				if (IsOrganisation)
					return Organisation.Description;
				else
					return Model.Description;
			}
		}

		public Guid UID
		{
			get
			{
				if (IsOrganisation)
					return Organisation.UID;
				else
					return Model.UID;
			}
		}

		public Guid OrganisationUID
		{
			get
			{
				if (IsOrganisation)
					return Organisation.UID;
				else
					return Model.OrganisationUID;
			}
		}

		bool _isDeleted;
		public bool IsDeleted
		{
			get { return _isDeleted; }
			set
			{
				_isDeleted = value;
				OnPropertyChanged(() => IsDeleted);
			}
		}

		string _removalDate;
		public string RemovalDate
		{
			get { return _removalDate; }
			set
			{
				_removalDate = value;
				OnPropertyChanged(() => RemovalDate);
			}
		}

		protected ViewPartViewModel ParentViewModel;

		public OrganisationElementViewModel() { }

		public virtual void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
			ParentViewModel = parentViewModel;
			IsDeleted = organisation.IsDeleted;
			RemovalDate = IsDeleted ? organisation.RemovalDate.ToString() : ""; 
		}

		public virtual void InitializeModel(Organisation organisation, ModelT model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			Model = model;
			IsOrganisation = false;
			ParentViewModel = parentViewModel;
			IsDeleted = model.IsDeleted;
			RemovalDate = IsDeleted ? model.RemovalDate.ToString() : "";
		}

		public virtual void Update(ModelT model)
		{
			Model = model;
			IsDeleted = model.IsDeleted;
			RemovalDate = IsDeleted ? model.RemovalDate.ToString() : "";
			Update();
		}

		public virtual void Update(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsDeleted = organisation.IsDeleted;
			RemovalDate = IsDeleted ? organisation.RemovalDate.ToString() : ""; 
			Update();
		}

		public virtual void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => IsDeleted);
			OnPropertyChanged(() => RemovalDate);
		}
	}

	public interface IOrganisationElementViewModel
	{
		bool IsDeleted { get; set; }
		Guid UID { get; }
		Guid OrganisationUID { get; } 
	}
}