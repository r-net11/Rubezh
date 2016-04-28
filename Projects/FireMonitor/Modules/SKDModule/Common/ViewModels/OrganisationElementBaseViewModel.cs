using System;
using RubezhAPI.SKD;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class OrganisationElementViewModel<T, ModelT> : TreeNodeViewModel<T>
		where T : TreeNodeViewModel<T>
		where ModelT : class, IOrganisationElement, IHRListItem, new()
	{
		public Organisation Organisation { get; protected set; }
		public ModelT Model { get; protected set; }
		public bool IsOrganisation { get; protected set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Guid UID { get; private set; }
		public Guid OrganisationUID { get; private set; }
		public string ImageSource { get { return IsOrganisation ? Organisation.ImageSource : Model.ImageSource; } }

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
			CopyProperties(organisation);
		}

		public virtual void InitializeModel(Organisation organisation, ModelT model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			Model = model;
			IsOrganisation = false;
			ParentViewModel = parentViewModel;
			CopyProperties(model);
		}

		void CopyProperties(IOrganisationElement model)
		{
			UID = model.UID;
			Name = model.Name;
			Description = model.Description;
			OrganisationUID = model.OrganisationUID;
			IsDeleted = model.IsDeleted;
			RemovalDate = IsDeleted ? model.RemovalDate.ToString("d MMM yyyy") : "";
		}

		void CopyProperties(Organisation organisation)
		{
			UID = organisation.UID;
			Name = organisation.Name;
			Description = organisation.Description;
			OrganisationUID = organisation.UID;
			IsDeleted = organisation.IsDeleted;
			RemovalDate = IsDeleted ? organisation.RemovalDate.ToString("d MMM yyyy") : "";
		}

		public virtual void Update(ModelT model)
		{
			Model = model;
			IsDeleted = model.IsDeleted;
			RemovalDate = IsDeleted ? model.RemovalDate.ToString("d MMM yyyy") : "";
			Update();
		}

		public virtual void Update(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsDeleted = organisation.IsDeleted;
			RemovalDate = IsDeleted ? organisation.RemovalDate.ToString("d MMM yyyy") : ""; 
			Update();
		}

		public virtual void Update()
		{
			if (IsOrganisation)
				CopyProperties(Organisation);
			else
				CopyProperties(Model);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			OnPropertyChanged(() => IsDeleted);
			OnPropertyChanged(() => RemovalDate);
		}

		public bool IsWithDeleted { get { return (ParentViewModel as IOrganisationBaseViewModel).IsWithDeleted; } }
	}

	public interface IOrganisationBaseViewModel
	{
		bool IsWithDeleted { get; set; }
	}
}