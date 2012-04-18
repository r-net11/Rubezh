
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

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            bool cancel = false;
            Save(ref cancel);
            if (cancel == false)
            {
                Close(true);
            }
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}