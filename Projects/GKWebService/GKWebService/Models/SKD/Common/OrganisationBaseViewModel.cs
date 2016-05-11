using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using GKWebService.DataProviders.SKD;
using RubezhAPI.SKD;
using RubezhClient;

namespace GKWebService.Models.SKD.Common
{
    public abstract class OrganisationBaseViewModel<TModel, TFilter, TViewModel>
        where TViewModel : OrganisationElementViewModel<TViewModel, TModel>, new()
        where TModel : class, IOrganisationElement, new()
        where TFilter : OrganisationFilterBase, new()
    {
        protected TModel _clipboard;

        protected Guid _clipboardUID;

        public List<TViewModel> Organisations { get; private set; }

        public TFilter Filter { get; set; }

        public bool IsWithDeleted { get; set; }

        protected abstract IEnumerable<TModel> GetModels(TFilter filter);

        protected virtual void InitializeModels(IEnumerable<TModel> models)
        {
            for (int index = 0; index < Organisations.Count; index++)
            {
                var organisation = Organisations[index];
                foreach (var model in models)
                {
                    if (model.OrganisationUID == organisation.Organisation.UID)
                    {
                        var itemViewModel = new TViewModel();
                        itemViewModel.InitializeModel(organisation.Organisation, model);
                        itemViewModel.Level = 1;
                        itemViewModel.ParentUID = model.OrganisationUID;
                        itemViewModel.IsLeaf = true;
                        itemViewModel.IsExpanded = !itemViewModel.IsLeaf;
                        organisation.IsLeaf = false;
                        organisation.IsExpanded = !organisation.IsLeaf;
                        Organisations.Insert(Organisations.IndexOf(organisation) + 1, itemViewModel);
                        index++;
                    }
                }
            }
        }

        public virtual void Initialize(TFilter filter)
        {
            Filter = filter;
            IsWithDeleted = filter.LogicalDeletationType == LogicalDeletationType.All;
            var result = InitializeOrganisations(Filter);
            if (result)
            {
                var models = GetModels(Filter);
                if (models != null)
                {
                    InitializeModels(models);
                }
            }
        }

        protected virtual bool InitializeOrganisations(TFilter filter)
        {
            var organisationFilter = new OrganisationFilter { UIDs = filter.OrganisationUIDs, User = ClientManager.CurrentUser, LogicalDeletationType = filter.LogicalDeletationType };
            var organisations = OrganisationHelper.Get(organisationFilter);
            if (organisations == null)
                return false;
            Organisations = new List<TViewModel>();
            foreach (var organisation in organisations)
            {
                var organisationViewModel = new TViewModel();
                organisationViewModel.InitializeOrganisation(organisation);
                organisationViewModel.Level = 0;
                organisationViewModel.IsLeaf = true;
                Organisations.Add(organisationViewModel);
            }
            return true;
        }
    }
}