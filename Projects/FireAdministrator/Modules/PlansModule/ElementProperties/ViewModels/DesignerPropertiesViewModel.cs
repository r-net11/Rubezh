using System.Windows.Media;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class DesignerPropertiesViewModel : SaveCancelDialogViewModel
	{
		public Plan Plan { get; private set; }
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public DesignerPropertiesViewModel(Plan plan)
		{
			Title = "Свойства элемента: План";
			ImagePropertiesViewModel = new ViewModels.ImagePropertiesViewModel();
			Plan = plan;
			CopyProperties();
		}

		void CopyProperties()
		{
			BackgroundColor = Plan.BackgroundColor;
			ImagePropertiesViewModel.BackgroundPixels = Plan.BackgroundPixels;
			ImagePropertiesViewModel.UpdateImage();
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged("BackgroundColor");
			}
		}

		protected override bool Save()
		{
			Plan.BackgroundColor = BackgroundColor;
			Plan.BackgroundPixels = ImagePropertiesViewModel.BackgroundPixels;
			return base.Save();
		}
	}
}
