using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows.Media;

namespace LayoutModule.ViewModels
{
	public class LayoutPropertiesViewModel : SaveCancelDialogViewModel
	{
		public Layout Layout { get; private set; }
		public LayoutUsersViewModel LayoutUsersViewModel { get; private set; }

		public LayoutPropertiesViewModel(Layout layout, LayoutUsersViewModel layoutUsersViewModel)
		{
			Title = "Свойства элемента: Шаблон интерфейса ОЗ";
			Layout = layout ?? new Layout();
			LayoutUsersViewModel = layoutUsersViewModel;
			LayoutUsersViewModel.Update(Layout);
			CopyProperties();
		}

		private void CopyProperties()
		{
			Caption = Layout.Caption;
			Description = Layout.Description;
			SplitterColor = Layout.SplitterColor;
			SplitterSize = Layout.SplitterSize;
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

		private Color _splitterColor;
		public Color SplitterColor
		{
			get { return _splitterColor; }
			set
			{
				_splitterColor = value;
				OnPropertyChanged(() => SplitterColor);
			}
		}
		private int _splitterSize;
		public int SplitterSize
		{
			get { return _splitterSize; }
			set
			{
				_splitterSize = value;
				OnPropertyChanged(() => SplitterSize);
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
			LayoutUsersViewModel.Save();
			Layout.SplitterColor = SplitterColor;
			Layout.SplitterSize = SplitterSize;
			return base.Save();
		}
	}
}