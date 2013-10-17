using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutFolderPropertiesViewModel : SaveCancelDialogViewModel
	{
		public LayoutFolder Folder { get; private set; }

		public LayoutFolderPropertiesViewModel(LayoutFolder folder)
		{
			Title = "Свойства элемента: Папка";
			Folder = folder ?? new LayoutFolder();
			CopyProperties();
		}

		private void CopyProperties()
		{
			Caption = Folder.Caption;
			Description = Folder.Description;
		}

		private string _caption;
		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged(() => Caption);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Caption);
		}
		protected override bool Save()
		{
			Folder.Caption = Caption;
			Folder.Description = Description;
			return base.Save();
		}
	}
}
