
namespace Infrastructure.Common
{
    public class SaveCancelDialogContent : DialogContent
    {
		public string SaveText { get; set; }
		public string CancelText { get; set; }
		
		public SaveCancelDialogContent()
        {
            SaveCommand = new RelayCommand(OnSave, CanSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        protected virtual void Save(ref bool cancel)
        {
        }

        protected virtual bool CanSave()
        {
            return true;
        }

        void OnSave()
        {
            bool cancel = false;
            Save(ref cancel);
            if (cancel == false)
            {
                Close(true);
            }
        }

        void OnCancel()
        {
            Close(false);
        }
    }
}