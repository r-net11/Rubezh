using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;

namespace LayoutModule.ViewModels
{
	public class LayoutPropertiesViewModel : SaveCancelDialogViewModel
	{
		public Layout Layout { get; private set; }

		public LayoutPropertiesViewModel(Layout layout)
		{
			Title = "Свойства элемента: Шаблон интерфейса ОЗ";
			Layout = layout ?? new Layout();
			CopyProperties();
		}

		private void CopyProperties()
		{
			Caption = Layout.Caption;
			Description = Layout.Description;
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
			Layout.Caption = Caption;
			Layout.Description = Description;
			return base.Save();
		}
	}
}