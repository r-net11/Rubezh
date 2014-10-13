using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationBaseViewModel<TModel, TFilter, TViewModel, TDetailsViewModel> : ViewPartViewModel, IEditingBaseViewModel
		where TViewModel : OrganisationElementViewModel<TViewModel, TModel>, new()
		where TModel : class, IOrganisationElement, new()
		where TDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<TModel>, new()
		where TFilter : OrganisationFilterBase
	{
		public OrganisationBaseViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RestoreCommand = new RelayCommand(OnRestore, CanRestore);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Unsubscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Subscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Unsubscribe(OnOrganisationUsersChanged);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Subscribe(OnOrganisationUsersChanged);
			ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Unsubscribe(OnRemoveOrganisation);
			ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Subscribe(OnRemoveOrganisation);
			ServiceFactory.Events.GetEvent<RestoreOrganisationEvent>().Unsubscribe(OnRestoreOrganisation);
			ServiceFactory.Events.GetEvent<RestoreOrganisationEvent>().Subscribe(OnRestoreOrganisation);
		}

		protected TModel _clipboard;
		protected TFilter _filter;
		protected Guid _clipboardUID;
		protected abstract IEnumerable<TModel> GetModels(TFilter filter);
		protected abstract IEnumerable<TModel> GetModelsByOrganisation(Guid organisationUID);
		protected abstract bool MarkDeleted(Guid uid);
		protected abstract bool Save(TModel item);
		protected abstract bool Restore(Guid uid);

		protected TModel ShowDetails(Organisation organisation, TModel model = null)
		{
			TModel result = null;
			var detailsViewModel = new TDetailsViewModel();
			if (detailsViewModel.Initialize(organisation, model, this) && DialogService.ShowModalWindow(detailsViewModel))
				result = detailsViewModel.Model;
			return result;
		}

		public virtual void Initialize(TFilter filter)
		{
			_filter = filter;
			var result = InitializeOrganisations(_filter);
			if (result)
			{
				var models = GetModels(_filter);
				if (models != null)
				{
					InitializeModels(models);
					OnPropertyChanged(() => Organisations);
					SelectedItem = Organisations.FirstOrDefault();
				}
			}
			IsWithDeleted = filter.LogicalDeletationType == LogicalDeletationType.All;
		}

		public bool IsWithDeleted { get; private set;}

		protected virtual bool InitializeOrganisations(TFilter filter)
		{
			var organisationFilter = new OrganisationFilter { UIDs = filter.OrganisationUIDs, UserUID = FiresecManager.CurrentUser.UID, LogicalDeletationType = filter.LogicalDeletationType };
			var organisations = OrganisationHelper.Get(organisationFilter);
			if (organisations == null)
				return false;
			Organisations = new ObservableCollection<TViewModel>();
			foreach (var organisation in organisations) 
			{
				var organisationViewModel = new TViewModel();
				organisationViewModel.InitializeOrganisation(organisation, this);
				Organisations.Add(organisationViewModel);
			}
			return true;
		}

		protected virtual void InitializeModels(IEnumerable<TModel> models)
		{
			foreach (var organisation in Organisations)
			{
				foreach (var model in models)
				{
					if (model.OrganisationUID == organisation.Organisation.UID)
					{
						var itemViewModel = new TViewModel();
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
					var organisationViewModel = new TViewModel();
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
				if (IsWithDeleted)
				{
					organisationViewModel.GetAllChildren().ForEach(x => x.IsDeleted = true);
				}
				else
				{
					Organisations.Remove(organisationViewModel);
				}
			}
		}

		protected virtual void OnRestoreOrganisation(Guid organisationUID)
		{
			var isInFilter = (_filter.OrganisationUIDs.Count == 0 || _filter.OrganisationUIDs.Any(x => x == organisationUID));
			if (isInFilter)
			{
				if (IsWithDeleted)
				{
					var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == organisationUID);
					if (organisationViewModel != null)
					{
						organisationViewModel.IsDeleted = false;
						var children = organisationViewModel.Children.ToList();
						foreach (var child in children)
						{
							organisationViewModel.RemoveChild(child);
						}
						var filter = _filter;
						filter.OrganisationUIDs = new List<Guid> { organisationUID };
						var models = GetModels(filter);
						if (models != null)
						{
							InitializeModels(models);
						}
						OnPropertyChanged(() => Organisations);
					}
				}
				else
				{
					var organisation = OrganisationHelper.GetSingle(organisationUID);
					if (organisation != null)
					{
						var organisationViewModel = new TViewModel();
						organisationViewModel.InitializeOrganisation(organisation, this);
						var filter = _filter;
						filter.OrganisationUIDs = new List<Guid> { organisationUID };
						var models = GetModels(filter);
						if (models != null)
						{
							InitializeModels(models);
						}
						OnPropertyChanged(() => Organisations);
					}
				}
			}
		}

		ObservableCollection<TViewModel> _organisations;
		public ObservableCollection<TViewModel> Organisations 
		{
			get { return _organisations; } 
			private set
			{
				_organisations = value;
				OnPropertyChanged(() => Organisations);
			}
		}

		TViewModel _selectedItem;
		public TViewModel SelectedItem
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

		public TViewModel ParentOrganisation
		{
			get
			{
				if (SelectedItem == null || SelectedItem.IsOrganisation)
					return SelectedItem;
				return SelectedItem.GetAllParents().FirstOrDefault(x => x.IsOrganisation);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		protected void OnAdd()
		{
			var model = ShowDetails(SelectedItem.Organisation);
			if (model == null)
				return;
			var itemViewModel = new TViewModel();
			itemViewModel.InitializeModel(SelectedItem.Organisation, model, this);
			var parentViewModel = GetParentItem();
			parentViewModel.AddChild(itemViewModel);
			SelectedItem = itemViewModel;
		}
		bool CanAdd()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted;
		}

		protected virtual TViewModel GetParentItem()
		{
			return SelectedItem.IsOrganisation ? SelectedItem : SelectedItem.Parent;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверены, что хотите архивировать {0}?", ItemRemovingName)))
			{
				Remove();
			}
		}
		protected virtual void Remove()
		{
			TViewModel OrganisationViewModel = SelectedItem;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedItem.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedItem);
			var model = SelectedItem.Model;
			var removeResult = MarkDeleted(model.UID);
			if (!removeResult)
				return;
			if (IsWithDeleted)
			{
				SelectedItem.IsDeleted = true;
				SelectedItem.RemovalDate = DateTime.Now.ToString("d MMM yyyy");
			}
			else
			{
				OrganisationViewModel.RemoveChild(SelectedItem);
				index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
				if (index > -1)
					SelectedItem = OrganisationViewModel.Children.ToList()[index];
				else
					SelectedItem = OrganisationViewModel;
			}
		}
		bool CanRemove()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && !SelectedItem.IsOrganisation;
		}
		protected virtual string ItemRemovingName
		{
			get { return "запись"; }
		}

		public RelayCommand RestoreCommand { get; private set; }
		void OnRestore()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверены, что хотите восстановить {0}?", ItemRemovingName)))
			{
				Restore();
			}
		}
		protected virtual void Restore()
		{
			if (!SelectedItem.IsDeleted)
				return;
			var restoreResult = Restore(SelectedItem.Model.UID);
			if (!restoreResult)
				return;
			SelectedItem.IsDeleted = false;
			SelectedItem.RemovalDate = "";
		}
		bool CanRestore()
		{
			return SelectedItem != null && SelectedItem.IsDeleted && !SelectedItem.IsOrganisation && !ParentOrganisation.IsDeleted;
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
			return SelectedItem != null && !SelectedItem.IsDeleted && SelectedItem.Parent != null && !SelectedItem.IsOrganisation;
		}

		public RelayCommand CopyCommand { get; private set; }
		protected virtual void OnCopy()
		{
			_clipboard = CopyModel(SelectedItem.Model);
			_clipboardUID = SelectedItem.Model.UID;
		}
		protected virtual bool CanCopy()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && !SelectedItem.IsOrganisation;
		}

		public RelayCommand PasteCommand { get; private set; }
		protected virtual void OnPaste()
		{
			var newItem = _clipboard;
			newItem.Name = CopyHelper.CopyName(newItem.Name, ParentOrganisation.Children.Select(x => x.Name));
			newItem.OrganisationUID = ParentOrganisation.Organisation.UID;
			if (Save(newItem))
			{
				var viewModel = new TViewModel();
				viewModel.InitializeModel(SelectedItem.Organisation, newItem, this);
				ParentOrganisation.AddChild(viewModel);
				SelectedItem = viewModel;
			}
		}
		protected virtual bool CanPaste()
		{
			return SelectedItem != null && _clipboard != null && ParentOrganisation != null;
		}

		protected virtual TModel CopyModel(TModel source)
		{
			var copy = new TModel();
			copy.UID = Guid.NewGuid();
			copy.Name = source.Name;
			copy.Description = source.Description;
			copy.OrganisationUID = ParentOrganisation.Organisation.UID;
			return copy;
		}
	}
}