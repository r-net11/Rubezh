using System.Windows.Media;
using FiresecAPI.Models;
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
			Plan = plan ?? new Plan()
			{
				Width = 400,
				Height = 400,
				//BackgroundColor = Colors.Transparent
			};
			CopyProperties();
		}

		void CopyProperties()
		{
			BackgroundColor = Plan.BackgroundColor;
			ImagePropertiesViewModel.BackgroundPixels = Plan.BackgroundPixels;
			ImagePropertiesViewModel.UpdateImage();
			Caption = Plan.Caption;
			Description = Plan.Description;
			Width = Plan.Width;
			Height = Plan.Height;
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

		string _caption;
		public string Caption
		{
			get { return _caption; }
			set
			{
				_caption = value;
				OnPropertyChanged("Caption");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged("Height");
			}
		}

		double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged("Width");
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Caption);
		}

		protected override bool Save()
		{
			Plan.Caption = Caption;
			Plan.Description = Description;
			Plan.Height = Height;
			Plan.Width = Width;
			Plan.BackgroundColor = BackgroundColor;
			Plan.BackgroundPixels = ImagePropertiesViewModel.BackgroundPixels;
			return base.Save();
		}
	}
}
