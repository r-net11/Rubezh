using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Designer.ElementProperties.ViewModels;
using Infrastructure.Plans;
using RubezhAPI.SKD;
using Color = System.Windows.Media.Color;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplatePropertiesViewModel : SaveCancelDialogViewModel
	{
		const double MinSize = 10;
		public PassCardTemplate PassCardTemplate { get; private set; }
		public ImagePropertiesViewModel ImagePropertiesViewModel { get; private set; }

		public PassCardTemplatePropertiesViewModel(PassCardTemplate passCardTemplate)
		{
			Title = "Свойства элемента: Шаблон пропуска";
			PassCardTemplate = passCardTemplate;
			ImagePropertiesViewModel = new ImagePropertiesViewModel(PassCardTemplate);
			CopyProperties();
		}

		void CopyProperties()
		{
			BackgroundColor = PassCardTemplate.BackgroundColor.ToWindowsColor();
			BorderColor = PassCardTemplate.BorderColor.ToWindowsColor();
			BorderThickness = PassCardTemplate.BorderThickness;
			Caption = PassCardTemplate.Caption;
			Description = PassCardTemplate.Description;
			Width = PassCardTemplate.Width;
			Height = PassCardTemplate.Height;
		}

		Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
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

		double _width;
		public double Width
		{
			get { return _width; }
			set
			{
				_width = value;
				OnPropertyChanged(() => Width);
			}
		}

		double _height;
		public double Height
		{
			get { return _height; }
			set
			{
				_height = value;
				OnPropertyChanged(() => Height);
			}
		}

		Color _borderColor;
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}

		double _borderThickness;
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
			PassCardTemplate.BackgroundColor = BackgroundColor.ToRubezhColor();
			PassCardTemplate.BorderThickness = BorderThickness;
			PassCardTemplate.BorderColor = BorderColor.ToRubezhColor();
			ImagePropertiesViewModel.Save();
			return base.Save();
		}
	}
}