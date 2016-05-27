using System.Windows.Media;
using Infrustructure.Plans;
using StrazhAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ElementProperties.ViewModels;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplatePropertiesViewModel : SaveCancelDialogViewModel
	{
		private const double MinSize = 10;
		public PassCardTemplate PassCardTemplate { get; private set; }
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public PassCardTemplatePropertiesViewModel(PassCardTemplate passCardTemplate)
		{
			Title = "Свойства элемента: Шаблон пропуска";
			PassCardTemplate = passCardTemplate;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(PassCardTemplate);
			CopyProperties();
		}

		private void CopyProperties()
		{
			if (PassCardTemplate == null) return;

			BackgroundColor = PassCardTemplate.BackgroundColor.ToWindowsColor();
			BorderColor = PassCardTemplate.BorderColor.ToWindowsColor();
			BorderThickness = PassCardTemplate.BorderThickness;
			Caption = PassCardTemplate.Caption;
			Description = PassCardTemplate.Description;
			Width = PassCardTemplate.Width;
			Height = PassCardTemplate.Height;
		}

		private Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
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

		private double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		private double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		private Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}

		private double _borderThickness;
		public double BorderThickness
		{
			get { return _borderThickness; }
			set
			{
				_borderThickness = value;
				OnPropertyChanged(() => BorderThickness);
			}
		}


		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Caption);
		}
		protected override bool Save()
		{
			PassCardTemplate.Caption = Caption;
			PassCardTemplate.Description = Description;
			PassCardTemplate.Width = Width;
			PassCardTemplate.Height = Height;
			PassCardTemplate.BackgroundColor = BackgroundColor.ToStruzhColor();
			PassCardTemplate.BorderThickness = BorderThickness;
			PassCardTemplate.BorderColor = BorderColor.ToStruzhColor();
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}