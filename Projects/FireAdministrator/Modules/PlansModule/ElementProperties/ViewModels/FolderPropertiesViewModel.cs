using RubezhAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class FolderPropertiesViewModel : SaveCancelDialogViewModel
	{
		const double MinSize = 10;
		public PlanFolder PlanFolder { get; private set; }

		public FolderPropertiesViewModel(PlanFolder planFolder)
		{
			Title = "Свойства элемента: Папка";
			PlanFolder = planFolder ?? new PlanFolder();
			CopyProperties();
		}

		private void CopyProperties()
		{
			Caption = PlanFolder.Caption;
			Description = PlanFolder.Description;
		}

		string _caption;
		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged(() => Caption);
			}
		}

		string _description;
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
			PlanFolder.Caption = Caption;
			PlanFolder.Description = Description;
			return base.Save();
		}
	}
}