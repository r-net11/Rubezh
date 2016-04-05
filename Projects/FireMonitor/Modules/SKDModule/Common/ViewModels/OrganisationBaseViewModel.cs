using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;
using SKDModule.Events;
using RubezhAPI;
using System.Diagnostics;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationBaseViewModel<TModel, TFilter, TViewModel, TDetailsViewModel> : ViewPartViewModel, IEditingBaseViewModel, IOrganisationBaseViewModel
		where TViewModel : OrganisationElementViewModel<TViewModel, TModel>, new()
		where TModel : class, IOrganisationElement, new()
		where TDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<TModel>, new()
		where TFilter : OrganisationFilterBase, new()
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
			Organisations = new ObservableCollection<TViewModel>();
			_filter = new TFilter();
		}

		public virtual void Unsubscribe()
		{
			ServiceFactory.Events.GetEvent<EditOrganisationEvent>().Unsubscribe(OnEditOrganisation);
			ServiceFactory.Events.GetEvent<OrganisationUsersChangedEvent>().Unsubscribe(OnOrganisationUsersChanged);
			ServiceFactory.Events.GetEvent<RemoveOrganisationEvent>().Unsubscribe(OnRemoveOrganisation);
			ServiceFactory.Events.GetEvent<RestoreOrganisationEvent>().Unsubscribe(OnRestoreOrganisation);
		}

		protected TModel _clipboard;
		protected TFilter _filter;
		protected Guid _clipboardUID;
		protected abstract IEnumerable<TModel> GetModels(TFilter filter);
		protected abstract IEnumerable<TModel> GetModelsByOrganisation(Guid organisationUID);
		protected abstract bool MarkDeleted(TModel model);
		protected abstract bool Add(TModel item);
		protected abstract bool Restore(TModel model);
		protected abstract PermissionType Permission { get; }
		bool IsEditAllowed { get { return ClientManager.CheckPermission(Permission); } }
		public TFilter Filter { get { return _filter; } }
		public IEnumerable<TModel> Models { get { return Organisations.SelectMany(x => x.GetAllChildren(false)).Select(x => x.Model); } }
		
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

		protected TModel ShowDetails(Organisation organisation, TModel model = null)
		{
			TModel result = null;
			var detailsViewModel = new TDetailsViewModel();
			if (detailsViewModel.Initialize(organisation, model, this) && ServiceFactory.DialogService.ShowModalWindow(detailsViewModel))
				result = detailsViewModel.Model;
			else
				return null;
			return result;
		}

		public virtual void Initialize(TFilter filter)
		{
			_filter = filter;
			IsWithDeleted = filter.LogicalDeletationType == LogicalDeletationType.All;
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
		}

		bool _isWithDeleted;
		public bool IsWithDeleted
		{
			get { return _isWithDeleted; }
			set
			{
				_isWithDeleted = value;
				OnPropertyChanged(() => IsWithDeleted);
			}
		}

		protected virtual bool InitializeOrganisations(TFilter filter)
		{
			var organisationFilter = new OrganisationFilter 
				{ 
					UIDs = filter.OrganisationUIDs, 
					UserUID = ClientManager.CurrentUser.UID, 
					LogicalDeletationType = filter.LogicalDeletationType 
				};
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
			if (newOrganisation.UserUIDs.Any(x => x == ClientManager.CurrentUser.UID))
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
					SetIsDeletedByOrganisation(organisationViewModel);
				}
				else
				{
					Organisations.Remove(organisationViewModel);
				}
			}
		}
		protected virtual void SetIsDeletedByOrganisation(TViewModel organisationViewModel)
		{
			organisationViewModel.GetAllChildren().ForEach(x =>
			{
				x.IsDeleted = true;
				x.IsOrganisationDeleted = true;
				if (x.RemovalDate == null || x.RemovalDate == "")
					x.RemovalDate = DateTime.Now.ToString("d MMM yyyy");
				x.Update();
			});
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
						organisationViewModel.RemovalDate = "";
						var children = organisationViewModel.Children.ToList();
						foreach (var child in children)
						{
							organisationViewModel.RemoveChild(child);
						}
						var filter = new TFilter 
							{ 
								OrganisationUIDs = new List<Guid> { organisationUID }, 
								LogicalDeletationType = LogicalDeletationType.All 
							};
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
						var filter = new TFilter 
							{ 
								OrganisationUIDs = new List<Guid> { organisationUID }, 
								LogicalDeletationType = LogicalDeletationType.All 
							};
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

		public ObservableCollection<TViewModel> Organisations { get; private set; }

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
		protected virtual void UpdateParent() { }

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
			if (IsAddViewModel(model))
			{
				var itemViewModel = new TViewModel();
				itemViewModel.InitializeModel(SelectedItem.Organisation, model, this);
				var parentViewModel = GetParentItem(model);
				parentViewModel.AddChild(itemViewModel);
				SelectedItem = itemViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && IsEditAllowed;
		}

		protected virtual bool IsAddViewModel(TModel model) { return true; }

		protected virtual TViewModel GetParentItem(TModel model)
		{
			return SelectedItem.IsOrganisation ? SelectedItem : SelectedItem.Parent;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (ShowRemovingQuestion())
			{
				Remove();
			}
		}
		protected virtual void Remove()
		{
			var model = SelectedItem.Model;
			var removeResult = MarkDeleted(model);
			if (!removeResult)
				return;
			if (IsWithDeleted)
			{
				SelectedItem.IsDeleted = true;
				SelectedItem.RemovalDate = DateTime.Now.ToString("d MMM yyyy");
			}
			else
			{
				RemoveSelectedViewModel();
			}
			AfterRemove(model);
		}
		protected virtual bool ShowRemovingQuestion()
		{
			return ServiceFactory.MessageBoxService.ShowQuestion(string.Format("Вы уверены, что хотите архивировать {0}?", ItemRemovingName));
		}
		void RemoveSelectedViewModel()
		{
			TViewModel OrganisationViewModel = SelectedItem;
			if (!OrganisationViewModel.IsOrganisation)
				OrganisationViewModel = SelectedItem.Parent;

			if (OrganisationViewModel == null || OrganisationViewModel.Organisation == null)
				return;

			var index = OrganisationViewModel.Children.ToList().IndexOf(SelectedItem);
			OrganisationViewModel.RemoveChild(SelectedItem);
			index = Math.Min(index, OrganisationViewModel.Children.Count() - 1);
			if (index > -1)
				SelectedItem = OrganisationViewModel.Children.ToList()[index];
			else
				SelectedItem = OrganisationViewModel;
		}
		protected virtual bool CanRemove()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && !SelectedItem.IsOrganisation && IsEditAllowed;
		}
		protected virtual void AfterRemove(TModel model) { }
		protected virtual string ItemRemovingName
		{
			get { return "запись"; }
		}

		public RelayCommand RestoreCommand { get; private set; }
		void OnRestore()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion(string.Format("Вы уверены, что хотите восстановить {0}?", ItemRemovingName)))
			{
				Restore();
			}
		}
		protected virtual void Restore()
		{
			if (!SelectedItem.IsDeleted)
				return;
			if (SelectedItem.Parent.Children.Any(x => x.Name == SelectedItem.Name && !x.IsDeleted))
			{
				ServiceFactory.MessageBoxService.Show("Существует неудалённый элемент с таким именем");
				return;
			}
			var restoreResult = Restore(SelectedItem.Model);
			if (!restoreResult)
				return;
			SelectedItem.IsDeleted = false;
			SelectedItem.RemovalDate = "";
			AfterRestore(SelectedItem.Model);

		}
		bool CanRestore()
		{
			return SelectedItem != null && SelectedItem.IsDeleted && !SelectedItem.IsOrganisation && ParentOrganisation != null && !ParentOrganisation.IsDeleted && IsEditAllowed;
		}
		protected virtual void AfterRestore(TModel model) { }

		public RelayCommand EditCommand { get; private set; }
		protected virtual void OnEdit()
		{
			var model = ShowDetails(SelectedItem.Organisation, SelectedItem.Model);
			if (model != null)
			{
				if (IsAddViewModel(model))
				{
					SelectedItem.Update(model);
					UpdateSelected();
					UpdateParent();
					AfterEdit(model);
				}
				else
				{
					RemoveSelectedViewModel();
				}
			}
		}
		protected virtual bool CanEdit()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && SelectedItem.Parent != null && !SelectedItem.IsOrganisation && IsEditAllowed;
		}
		protected virtual void AfterEdit(TModel model) { }

		public RelayCommand CopyCommand { get; private set; }
		protected virtual void OnCopy()
		{
			if (SelectedItem.Name.Length > 46)
			{
				ServiceFactory.MessageBoxService.Show("Название копируемой записи должно быть короче 47 символов");
			}
			else
			{
				_clipboard = CopyModel(SelectedItem.Model);
				_clipboardUID = SelectedItem.Model.UID;
			}
		}
		protected virtual bool CanCopy()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && !SelectedItem.IsOrganisation && IsEditAllowed;
		}

		public RelayCommand PasteCommand { get; private set; }
		protected virtual void OnPaste()
		{
			var newItem = CopyModel(_clipboard);
			newItem.Name = CopyHelper.CopyName(newItem.Name, ParentOrganisation.Children.Select(x => x.Name));
			newItem.OrganisationUID = ParentOrganisation.Organisation.UID;
			if (Add(newItem))
			{
				var viewModel = new TViewModel();
				viewModel.InitializeModel(SelectedItem.Organisation, newItem, this);
				ParentOrganisation.AddChild(viewModel);
				SelectedItem = viewModel;
			}
		}
		protected virtual bool CanPaste()
		{
			return SelectedItem != null && _clipboard != null && ParentOrganisation != null && IsEditAllowed;
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

		public bool ShowFromJournal(Guid uid)
		{
			var items = Organisations.SelectMany(x => x.GetAllChildren(false));
			var selectedItem = items.FirstOrDefault(x => x.UID == uid);
			if(selectedItem != null)
			{
				SelectedItem = selectedItem;
				return true;
			}
			return false;
		}
	}
}