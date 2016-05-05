using System;
using StrazhAPI.SKD;
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
			get { return IsOrganisation ? Organisation.Name : Model.Name; }
		}
		public string Description
		{
			get { return IsOrganisation ? Organisation.Description : Model.Description; }
		}

		public Guid UID
		{
			get { return IsOrganisation ? Organisation.UID : Model.UID; }
		}

		public Guid OrganisationUID
		{
			get { return IsOrganisation ? Organisation.UID : Model.OrganisationUID; }
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

		bool _isOrganisationDeleted;
		public bool IsOrganisationDeleted
		{
			get { return _isOrganisationDeleted; }
			set
			{
				_isOrganisationDeleted = value;
				OnPropertyChanged(() => IsOrganisationDeleted);
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
			RemovalDate = IsDeleted ? organisation.RemovalDate.ToString() : string.Empty;
		}

		public virtual void InitializeModel(Organisation organisation, ModelT model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			Model = model;
			IsOrganisation = false;
			ParentViewModel = parentViewModel;
			IsDeleted = model.IsDeleted;
			RemovalDate = IsDeleted ? model.RemovalDate.ToString() : string.Empty;
		}

		public virtual void Update(ModelT model)
		{
			Model = model;
			IsDeleted = model.IsDeleted;
			RemovalDate = IsDeleted ? model.RemovalDate.ToString() : string.Empty;
			Update();
		}

		public virtual void Update(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsDeleted = organisation.IsDeleted;
			RemovalDate = IsDeleted ? organisation.RemovalDate.ToString() : string.Empty; 
			Update();
		}

		public virtual void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => IsDeleted);
			OnPropertyChanged(() => RemovalDate);
		}

		public bool IsWithDeleted { get { return (ParentViewModel as IOrganisationBaseViewModel).IsWithDeleted; } }
	}

	public interface IOrganisationElementViewModel
	{
		bool IsDeleted { get; set; }
		bool IsOrganisationDeleted { get; set; }
		bool IsWithDeleted { get; }
		Guid UID { get; }
		Guid OrganisationUID { get; }
		string Name { get; }
		string Description { get; }
	}

	public interface IOrganisationBaseViewModel
	{
		bool IsWithDeleted { get; set; }
	}
}