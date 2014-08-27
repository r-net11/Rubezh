using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Common;

namespace SKDModule.ViewModels
{
    public abstract class CartothequeTabItemCopyPasteBase<ModelT, FilterT, ViewModelT, DetailsViewModelT> : CartothequeTabItemBase<ModelT, FilterT, ViewModelT, DetailsViewModelT>
        where ViewModelT : CartothequeTabItemElementBase<ViewModelT, ModelT>, new()
        where ModelT : class, IWithOrganisationUID, IWithUID, IWithName, new()
        where DetailsViewModelT : SaveCancelDialogViewModel, IDetailsViewModel<ModelT>, new()
    {
        protected ModelT _clipboard;
        protected abstract bool Save(ModelT item);

        public CartothequeTabItemCopyPasteBase()
            : base()
        {
            CopyCommand = new RelayCommand(OnCopy, CanCopy);
            PasteCommand = new RelayCommand(OnPaste, CanPaste);
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
                    accessTemplateViewModel.InitializeModel(SelectedItem.Organisation, newAccessTemplate, this);
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
