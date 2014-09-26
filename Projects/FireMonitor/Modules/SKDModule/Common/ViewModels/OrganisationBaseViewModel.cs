using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;
using SKDModule.Events;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationBaseViewModel<ModelT, FilterT, ViewModelT, DetailsViewModelT> : ViewPartViewModel, IEditingBaseViewModel
		where ViewModelT : CartothequeTabItemElementBase<ViewModelT, ModelT>, new()
        where ModelT : class, IOrganisationElement, new()
		where DetailsViewModelT : SaveCancelDialogViewModel, IDetailsViewModel<ModelT>, new()
		where FilterT : OrganisationFilterBase
	{
        public OrganisationBaseViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
            CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Unsubscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Subscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Unsubscribe(OnOrganisationUsersChanged);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Subscribe(OnOrganisationUsersChanged);
            ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Unsubscribe(OnRemoveOrganisation);
            ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Subscribe(OnRemoveOrganisation);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }

        protected ModelT _clipboard;
		protected Guid _clipboardUID;
        protected abstract IEnumerable<ModelT> GetModels(FilterT filter);
		protected abstract IEnumerable<ModelT> GetModelsByOrganisation(Guid organisauinUID);
		protected abstract bool MarkDeleted(Guid uid);
        protected abstract bool Save(ModelT item);

		protected ModelT ShowDetails(Organisation organisation, ModelT model = null)
		{
			ModelT result = null;
			var detailsViewModel = new DetailsViewModelT();
			if (detailsViewModel.Initialize(organisation, model, this) && DialogService.ShowModalWindow(detailsViewModel))
				result = detailsViewModel.Model;
			return result;
		}

		public virtual void Initialize(FilterT filter)
		{
			var result = InitializeOrganisations(filter);
            if (result)
            {
                var models = GetModels(filter);
                if (models != null)
                {
                    InitializeModels(models);
                    OnPropertyChanged(() => Organisations);
                    SelectedItem = Organisations.FirstOrDefault();
                }
            }
		}

		protected virtual bool InitializeOrganisations(FilterT filter)
		{
			var organisationFilter = new OrganisationFilter { UIDs = filter.OrganisationUIDs, UserUID = FiresecManager.CurrentUser.UID };
			var organisations = OrganisationHelper.Get(organisationFilter);
			if (organisations == null)
				return false;
			Organisations = new ObservableCollection<ViewModelT>();
			foreach (var organisation in organisations) 
			{
				var organisationViewModel = new ViewModelT();
				organisationViewModel.InitializeOrganisation(organisation, this);
				Organisations.Add(organisationViewModel);
			}
			return true;
		}

		protected virtual void InitializeModels(IEnumerable<ModelT> models)
		{
			foreach (var organisation in Organisations)
			{
				foreach (var model in models)
				{
					if (model.OrganisationUID == organisation.Organisation.UID)
					{
						var itemViewModel = new ViewModelT();
						itemViewModel.InitializeModel(organisation.Organisation, model, this);
						organisation.AddChild(itemViewModel);
					}
				}
			}
		}

		protected virtual void OnEditOrganisation(Organisation newOrganisation)
		{
			var organisation = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
			if (organisation != null)
			{
				organisation.Update(newOrganisation);
			}
		}

		protected virtual void OnOrganisationUsersChanged(Organisation newOrganisation)
		{
            if (newOrganisation.UserUIDs.Any(x => x == FiresecManager.CurrentUser.UID))
            {
                if (!Organisations.Any(x => x.Organisation.UID == newOrganisation.UID))
                {
                    var organisationViewModel = new ViewModelT();
                    organisationViewModel.InitializeOrganisation(newOrganisation, this);
                    Organisations.Add(organisationViewModel);
                    var models = GetModelsByOrganisation(newOrganisation.UID);
                    if (models != null)
                    {
                        InitializeModels(models);
                    }
                }
            }
            else
            {
                var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
                if (organisationViewModel != null)
                {
                    Organisations.Remove(organisationViewModel);
                }
            }
        }

        protected virtual void OnRemoveOrganisation(Guid organisationUID)
        {
            var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == organisationUID);
            if (organisationViewModel != null)
            {
                Organisations.Remove(organisationViewModel);
            }
        }

        ObservableCollection<ViewModelT> _organisations;
        public ObservableCollection<ViewModelT> Organisations 
        {
            get { return _organisations; } 
            private set
            {
                _organisations = value;
                OnPropertyChanged(() => Organisations);
            }
        }

		ViewModelT _selectedItem;
		public ViewModelT SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				if (value != null)
				{
					value.ExpandToThis();
					UpdateSelected();
				}
				OnPropertyChanged(() => SelectedItem);
			}
		}

		protected virtual void UpdateSelected() { }

		public ViewModelT ParentOrganisation
		{
			get
			{
				ViewModelT organisationViewModel = SelectedItem;
				if (!organisationViewModel.IsOrganisation)
					organisationViewModel = SelectedItem.Parent;
                if (organisationViewModel.Organisation != null)
					return organisationViewModel;
                return null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		protected void OnAdd()
		{
			var model = ShowDetails(SelectedItem.Organisation);
			if (model == null)
				return;
			var itemViewModel = new ViewModelT();
			itemViewModel.InitializeModel(SelectedItem.Organisation, model, this);
			var parentViewModel = GetParentItem();
			parentViewModel.AddChild(itemViewModel);
			SelectedItem = itemViewModel;
		}
		bool CanAdd()
		{
			return SelectedItem != null;
		}

		protected virtual ViewModelT GetParentItem()
		{
			return SelectedItem.IsOrganisation ? SelectedItem : SelectedItem.Parent;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion2(string.Format("Вы уверены, что хотите удалить {0}?", ItemRemovingName)))
			{
				Remove();
			}
		}
		protected virtual void Remove()
		{
			ViewModelT OrganisationViewModel = SelectedItem;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedItem.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedItem);
			var model = SelectedItem.Model;
			var removeResult = MarkDeleted(model.UID);
			if (!removeResult)
				return;
			OrganisationViewModel.RemoveChild(SelectedItem);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedItem = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedItem = OrganisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedItem != null && !SelectedItem.IsOrganisation;
		}
		protected virtual string ItemRemovingName
		{
			get { return "запись"; }
		}

		public RelayCommand EditCommand { get; private set; }
		protected void OnEdit()
		{
			var model = ShowDetails(SelectedItem.Organisation, SelectedItem.Model);
			if (model != null)
			{
				SelectedItem.Update(model);
				UpdateSelected();
			}
		}
		bool CanEdit()
		{
			return SelectedItem != null && SelectedItem.Parent != null && !SelectedItem.IsOrganisation;
		}

        public RelayCommand CopyCommand { get; private set; }
		protected virtual void OnCopy()
		{
			_clipboard = CopyModel(SelectedItem.Model);
			_clipboardUID = SelectedItem.Model.UID;
		}
		protected virtual bool CanCopy()
		{
			return SelectedItem != null && !SelectedItem.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		protected virtual void OnPaste()
		{
			var newItem = _clipboard;
            newItem.Name = CopyHelper.CopyName(newItem.Name, ParentOrganisation.Children.Select(x => x.Name));
			newItem.OrganisationUID = ParentOrganisation.Organisation.UID;
			if (Save(newItem))
			{
				var itemVireModel = new ViewModelT();
				itemVireModel.InitializeModel(SelectedItem.Organisation, newItem, this);
				ParentOrganisation.AddChild(itemVireModel);
				SelectedItem = itemVireModel;
			}
		}
		protected virtual bool CanPaste()
		{
			return SelectedItem != null && _clipboard != null && ParentOrganisation != null;
		}

		protected virtual ModelT CopyModel(ModelT source)
		{
			var copy = new ModelT();
			copy.UID = Guid.NewGuid();
			copy.Name = source.Name;
            copy.Description = source.Description;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			return copy;
		}
	}
}