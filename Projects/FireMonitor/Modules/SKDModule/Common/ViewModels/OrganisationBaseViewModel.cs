using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Localization.SKD.ViewModels;
using SKDModule.Common;
using SKDModule.Events;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace SKDModule.ViewModels
{
	public abstract class OrganisationBaseViewModel<TModel, TFilter, TViewModel, TDetailsViewModel> : ViewPartViewModel, IEditingBaseViewModel, IOrganisationBaseViewModel
		where TViewModel : OrganisationElementViewModel<TViewModel, TModel>, new()
		where TModel : class, IOrganisationElement, new()
		where TDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<TModel>, new()
		where TFilter : OrganisationFilterBase, new()
	{
		protected OrganisationBaseViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RestoreCommand = new RelayCommand(OnRestore, CanRestore);
			ServiceFactoryBase.Events.GetEvent<EditOrganisationEvent>().Unsubscribe(OnEditOrganisation);
			ServiceFactoryBase.Events.GetEvent<EditOrganisationEvent>().Subscribe(OnEditOrganisation);
			ServiceFactoryBase.Events.GetEvent<OrganisationUsersChangedEvent>().Unsubscribe(OnOrganisationUsersChanged);
			ServiceFactoryBase.Events.GetEvent<OrganisationUsersChangedEvent>().Subscribe(OnOrganisationUsersChanged);
			ServiceFactoryBase.Events.GetEvent<RemoveOrganisationEvent>().Unsubscribe(OnRemoveOrganisation);
			ServiceFactoryBase.Events.GetEvent<RemoveOrganisationEvent>().Subscribe(OnRemoveOrganisation);
			ServiceFactoryBase.Events.GetEvent<RestoreOrganisationEvent>().Unsubscribe(OnRestoreOrganisation);
			ServiceFactoryBase.Events.GetEvent<RestoreOrganisationEvent>().Subscribe(OnRestoreOrganisation);
			ServiceFactoryBase.Events.GetEvent<NewOrganisationEvent>().Unsubscribe(OnNewOrganisation);
			ServiceFactoryBase.Events.GetEvent<NewOrganisationEvent>().Subscribe(OnNewOrganisation);
			_filter = new TFilter();
		}

		protected TModel Clipboard;
		protected TFilter _filter;
		protected Guid ClipboardUID;
		protected abstract IEnumerable<TModel> GetModels(TFilter filter);
		protected abstract IEnumerable<TModel> GetModelsByOrganisation(Guid organisationUID);
		protected abstract bool MarkDeleted(TModel model);
		protected abstract bool Add(TModel item);
		protected abstract bool Restore(TModel model);
		protected abstract PermissionType Permission { get; }
		bool IsEditAllowed { get { return FiresecManager.CheckPermission(Permission); } }
		public TFilter Filter { get { return _filter; } }

		protected TModel ShowDetails(Organisation organisation, TModel model = null)
		{
			TModel result;
			var detailsViewModel = new TDetailsViewModel();
			if (detailsViewModel.Initialize(organisation, model, this) && DialogService.ShowModalWindow(detailsViewModel))
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
				if (Organisations.Any(x => x.Organisation.UID == newOrganisation.UID)) return;

				var organisationViewModel = new TViewModel();
				organisationViewModel.InitializeOrganisation(newOrganisation, this);
				Organisations.Add(organisationViewModel);
				var models = GetModelsByOrganisation(newOrganisation.UID);
				if (models != null)
				{
					InitializeModels(models);
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

			if (organisationViewModel == null) return;

			if (IsWithDeleted)
				SetIsDeletedByOrganisation(organisationViewModel);
			else
				Organisations.Remove(organisationViewModel);
		}
		protected virtual void SetIsDeletedByOrganisation(TViewModel organisationViewModel)
		{
			organisationViewModel.GetAllChildren().ForEach(x =>
			{
				x.IsDeleted = true;
				x.IsOrganisationDeleted = true;

				if (string.IsNullOrEmpty(x.RemovalDate))
					x.RemovalDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);

				x.Update();
			});
		}

		protected virtual void OnRestoreOrganisation(Guid organisationUID)
		{
			var isInFilter = (_filter.OrganisationUIDs.Count == 0 || _filter.OrganisationUIDs.Any(x => x == organisationUID));

			if (!isInFilter) return;

			if (IsWithDeleted)
			{
				var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == organisationUID);

				if (organisationViewModel == null) return;

				organisationViewModel.IsDeleted = false;
				organisationViewModel.RemovalDate = string.Empty;
				var children = organisationViewModel.Children.ToList();

				foreach (var child in children)
					organisationViewModel.RemoveChild(child);

				var filter = new TFilter { OrganisationUIDs = new List<Guid> { organisationUID }, LogicalDeletationType = LogicalDeletationType.All };
				var models = GetModels(filter);

				if (models != null)
					InitializeModels(models);

				OnPropertyChanged(() => Organisations);
			}
			else
			{
				var organisation = OrganisationHelper.GetSingle(organisationUID);

				if (organisation == null) return;

				var organisationViewModel = new TViewModel();
				organisationViewModel.InitializeOrganisation(organisation, this);
				var filter = new TFilter { OrganisationUIDs = new List<Guid> { organisationUID }, LogicalDeletationType = LogicalDeletationType.All };
				var models = GetModels(filter);

				if (models != null)
					InitializeModels(models);

				OnPropertyChanged(() => Organisations);
			}
		}

		protected virtual void OnNewOrganisation(Guid organisationUID)
		{
			var isInFilter = (_filter.OrganisationUIDs.Count == 0);

			if (!isInFilter) return;

			var organisation = OrganisationHelper.GetSingle(organisationUID);

			if (organisation == null) return;

			var organisationViewModel = new TViewModel();
			organisationViewModel.InitializeOrganisation(organisation, this);
			Organisations.Add(organisationViewModel);
			OnPropertyChanged(() => Organisations);
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
		protected virtual void OnAdd()
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
			return SelectedItem != null && !SelectedItem.IsDeleted && IsEditAllowed && ParentOrganisation != null && !ParentOrganisation.IsDeleted;
		}

		protected virtual bool IsAddViewModel(TModel model) { return true; }

		protected virtual TViewModel GetParentItem(TModel model)
		{
			return SelectedItem.IsOrganisation ? SelectedItem : SelectedItem.Parent;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion(string.Format(CommonViewModels.Archive, ItemRemovingName)))
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
				SelectedItem.RemovalDate = DateTime.Now.ToString(CultureInfo.CurrentUICulture);
			}
			else
			{
				RemoveSelectedViewModel();
			}
			AfterRemove(model);
		}
		void RemoveSelectedViewModel()
		{
			var organisationViewModel = SelectedItem;
			if (!organisationViewModel.IsOrganisation)
				organisationViewModel = SelectedItem.Parent;

			if (organisationViewModel == null || organisationViewModel.Organisation == null)
				return;

			var index = organisationViewModel.Children.ToList().IndexOf(SelectedItem); //TODO: Remove incorrect logic of choising next Selected item from unsorted list. The sort of backend collection and UI collection is different
			organisationViewModel.RemoveChild(SelectedItem);
			index = Math.Min(index, organisationViewModel.Children.Count() - 1);
			SelectedItem = index > -1
				? organisationViewModel.Children.ToList()[index]
				: organisationViewModel;
		}
		bool CanRemove()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && !SelectedItem.IsOrganisation && IsEditAllowed;
		}
		protected virtual void AfterRemove(TModel model) { }
		protected virtual string ItemRemovingName
		{
			get { return CommonViewModels.Record; }
		}

		public RelayCommand RestoreCommand { get; private set; }
		void OnRestore()
		{
			if (MessageBoxService.ShowQuestion(string.Format(CommonViewModels.Restore, ItemRemovingName)))
			{
				Restore();
			}
		}
		protected virtual void Restore()
		{
			if (!SelectedItem.IsDeleted)
				return;
			var restoreResult = Restore(SelectedItem.Model);
			if (!restoreResult)
				return;
			SelectedItem.IsDeleted = false;
			SelectedItem.RemovalDate = null;
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

		public RelayCommand CopyCommand { get; private set; }
		protected virtual void OnCopy()
		{
			if (SelectedItem.Name.Length > 46)
			{
				MessageBoxService.Show(CommonViewModels.NameOfCopyData);
			}
			else
			{
				Clipboard = CopyModel(SelectedItem.Model);
				ClipboardUID = SelectedItem.Model.UID;
			}
		}
		protected virtual bool CanCopy()
		{
			return SelectedItem != null && !SelectedItem.IsDeleted && !SelectedItem.IsOrganisation && IsEditAllowed;
		}

		public RelayCommand PasteCommand { get; private set; }
		protected virtual void OnPaste()
		{
			var newItem = CopyModel(Clipboard);
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
			return SelectedItem != null
				&& Clipboard != null
				&& ParentOrganisation != null
				&& IsEditAllowed
				&& !SelectedItem.IsDeleted
				&& !ParentOrganisation.IsDeleted;
		}

		protected virtual TModel CopyModel(TModel source)
		{
			var copy = new TModel
			{
				UID = Guid.NewGuid(),
				Name = source.Name,
				Description = source.Description,
				OrganisationUID = ParentOrganisation.Organisation.UID
			};

			return copy;
		}
	}
}