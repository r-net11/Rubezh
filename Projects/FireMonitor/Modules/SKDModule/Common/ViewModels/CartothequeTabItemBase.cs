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
using SKDModule.Common.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
    public abstract class CartothequeTabItemBase<ModelT, FilterT, ViewModelT, DetailsViewModelT> : ViewPartViewModel
        where ViewModelT : CartothequeTabItemElementBase<ViewModelT, ModelT>, new()
        where ModelT : OrganisationElementBase, IWithName, new()
        where DetailsViewModelT : SaveCancelDialogViewModel, IDetailsViewModel<ModelT>, new()
    {
        protected ModelT _clipboard;

        public CartothequeTabItemBase()
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
        }

        protected abstract IEnumerable<ModelT> GetModels(FilterT filter);
        protected abstract IEnumerable<ModelT> GetModelsByOrganisation(Guid organisauinUID);
        protected abstract bool MarkDeleted(Guid uid);
        protected abstract bool Save(ModelT item);

        protected ModelT ShowDetails(Organisation organisation, ModelT model = null)
        {
            ModelT result = null;
            var detailsViewModel = new DetailsViewModelT();
            detailsViewModel.Initialize(SelectedItem.Organisation, SelectedItem.Model);
            if (DialogService.ShowModalWindow(detailsViewModel))
            {
                result = detailsViewModel.Model;
            }
            return result;
        }

        protected void OnAdd()
        {
            var model = ShowDetails(SelectedItem.Organisation);
            if (model == null)
                return;
            var itemViewModel = new ViewModelT();
            itemViewModel.InitializeModel(SelectedItem.Organisation, model);
            ViewModelT organisationViewModel = SelectedItem;
            if (!organisationViewModel.IsOrganisation)
                organisationViewModel = SelectedItem.Parent;
            if (organisationViewModel == null || organisationViewModel.Organisation == null)
                return;
            organisationViewModel.AddChild(itemViewModel);
            SelectedItem = itemViewModel;
        }

        protected void OnEdit()
        {
            var model = ShowDetails(SelectedItem.Organisation);
            if (model != null)
                SelectedItem.Update(model);
        }

        public void Initialize(FilterT filter)
        {
            var organisations = OrganisationHelper.GetByCurrentUser();
            if (organisations == null)
                return;
            var models = GetModels(filter);
            if (models == null)
                return;
            Organisations = new ObservableCollection<ViewModelT>();
            foreach (var organisation in organisations)
            {
                var organisationViewModel = new ViewModelT();
                organisationViewModel.InitializeOrganisation(organisation);
                Organisations.Add(organisationViewModel);
                if (models != null)
                {
                    foreach (var model in models)
                    {
                        if (model.OrganisationUID == organisation.UID)
                        {
                            var itemViewModel = new ViewModelT();
                            itemViewModel.InitializeModel(organisation, model);
                            organisationViewModel.AddChild(itemViewModel);
                        }
                    }
                }
            }
            OnPropertyChanged(() => Organisations);
            SelectedItem = Organisations.FirstOrDefault();
        }

        void OnEditOrganisation(Organisation newOrganisation)
        {
            var organisation = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
            if (organisation != null)
            {
                organisation.Update(newOrganisation);
            }
            OnPropertyChanged(() => Organisations);
        }

        void OnOrganisationUsersChanged(Organisation newOrganisation)
        {
            if (newOrganisation.UserUIDs.Any(x => x == FiresecManager.CurrentUser.UID))
            {
                var organisationViewModel = new ViewModelT();
                organisationViewModel.InitializeOrganisation(newOrganisation);
                Organisations.Add(organisationViewModel);
                var models = GetModelsByOrganisation(newOrganisation.UID);
                if (models == null)
                    return;
                foreach (var model in models)
                {
                    var itemViewModel = new ViewModelT();
                    itemViewModel.InitializeModel(newOrganisation, model);
                    organisationViewModel.AddChild(itemViewModel);
                }
                OnPropertyChanged(() => Organisations);
            }
            else
            {
                var organisationViewModel = Organisations.FirstOrDefault(x => x.Organisation.UID == newOrganisation.UID);
                if (organisationViewModel != null)
                {
                    Organisations.Remove(organisationViewModel);
                    OnPropertyChanged(() => Organisations);
                }
            }
        }

        public ObservableCollection<ViewModelT> Organisations { get; private set; }

        ViewModelT _selectedItem;
        public ViewModelT SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (value != null)
                    value.ExpandToThis();
                OnPropertyChanged(() => SelectedItem);
            }
        }

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

        bool CanAdd()
        {
            return SelectedItem != null;
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
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

        public RelayCommand EditCommand { get; private set; }
        bool CanEdit()
        {
            return SelectedItem != null && SelectedItem.Parent != null && !SelectedItem.IsOrganisation;
        }

        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            _clipboard = CopyModel(SelectedItem.Model, false);
        }
        bool CanCopy()
        {
            return SelectedItem != null && !SelectedItem.IsOrganisation;
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            if (ParentOrganisation != null)
            {
                var newAccessTemplate = CopyModel(_clipboard);
                if (Save(newAccessTemplate))
                {
                    var accessTemplateViewModel = new ViewModelT();
                    accessTemplateViewModel.InitializeModel(SelectedItem.Organisation, newAccessTemplate);
                    ParentOrganisation.AddChild(accessTemplateViewModel);
                    SelectedItem = accessTemplateViewModel;
                }
            }
        }
        bool CanPaste()
        {
            return SelectedItem != null && _clipboard != null && ParentOrganisation != null && ParentOrganisation.Organisation.UID == _clipboard.OrganisationUID;
        }

        protected virtual ModelT CopyModel(ModelT source, bool newName = true)
        {
            var copy = new ModelT();
            copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
            copy.OrganisationUID = ParentOrganisation.Organisation.UID;
            return copy;
        }
    }
}
