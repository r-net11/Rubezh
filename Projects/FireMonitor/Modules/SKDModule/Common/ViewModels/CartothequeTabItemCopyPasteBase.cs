using System;
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
        where FilterT: OrganisationFilterBase
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
        protected virtual void OnCopy()
        {
            _clipboard = CopyModel(SelectedItem.Model, false);
        }
        protected virtual bool CanCopy()
        {
            return SelectedItem != null && !SelectedItem.IsOrganisation;
        }

        public RelayCommand PasteCommand { get; private set; }
        protected virtual void OnPaste()
        {
            var newItem = CopyModel(_clipboard);
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

        protected virtual ModelT CopyModel(ModelT source, bool newName = true)
        {
            var copy = new ModelT();
            copy.UID = Guid.NewGuid();
            copy.Name = newName ? CopyHelper.CopyName(source.Name, ParentOrganisation.Children.Select(item => item.Name)) : source.Name;
            copy.OrganisationUID = ParentOrganisation.Organisation.UID;
            return copy;
        }
    }
}
