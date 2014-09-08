using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CartothequeTabItemElementBase<T, ModelT> : TreeNodeViewModel<T>
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
		
		protected ViewPartViewModel ParentViewModel;

		public CartothequeTabItemElementBase() { }

		public virtual void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
			ParentViewModel = parentViewModel;
		}

		public virtual void InitializeModel(Organisation organisation, ModelT model, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			Model = model;
			IsOrganisation = false;
			ParentViewModel = parentViewModel;
		}

		public virtual void Update(ModelT model)
		{
			Model = model;
			Update();
		}

		public virtual void Update(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Update();
		}

		public virtual void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}
	}
}