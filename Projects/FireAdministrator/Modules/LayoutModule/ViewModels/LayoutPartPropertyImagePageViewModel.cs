using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Microsoft.Win32;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace LayoutModule.ViewModels
{
	public class LayoutPartPropertyImagePageViewModel : LayoutPartPropertyPageViewModel
	{
		private const string VectorGraphicExtensions = ".svg;.svgx";
		private LayoutPartImageViewModel _layoutPartImageViewModel;
		private WpfDrawingSettings _settings;
		private string _sourceName;
		private bool _imageChanged;
		private DrawingGroup _drawing;

		public LayoutPartPropertyImagePageViewModel(LayoutPartImageViewModel layoutPartImageViewModel)
		{
			_layoutPartImageViewModel = layoutPartImageViewModel;
			_settings = new WpfDrawingSettings()
			{
				IncludeRuntime = false,
				TextAsGeometry = true,
				OptimizePath = true,
			};
			StretchTypes = new ObservableCollection<Stretch>(Enum.GetValues(typeof(Stretch)).Cast<Stretch>());
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture, CanRemovePicture);
		}

		public ObservableCollection<Stretch> StretchTypes { get; private set; }
		private Stretch _stretch;
		public Stretch Stretch
		{
			get { return _stretch; }
			set
			{
				_stretch = value;
				OnPropertyChanged(() => Stretch);
			}
		}
		private ImageSource _imageSource;
		public ImageSource ImageSource
		{
			get { return _imageSource; }
			set
			{
				_imageSource = value;
				OnPropertyChanged(() => ImageSource);
			}
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		private void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Все файлы изображений|*.bmp; *.png; *.jpeg; *.jpg; *.svg|BMP Файлы|*.bmp|PNG Файлы|*.png|JPEG Файлы|*.jpeg|JPG Файлы|*.jpg|SVG Файлы|*.svg";
			if (openFileDialog.ShowDialog().Value)
			{
				_sourceName = openFileDialog.FileName;
				if (VectorGraphicExtensions.Contains(Path.GetExtension(_sourceName)))
				{
					using (FileSvgReader reader = new FileSvgReader(_settings))
						_drawing = reader.Read(openFileDialog.FileName);
					ImageSource = new DrawingImage(_drawing);
				}
				else
				{
					_drawing = null;
					ImageSource = new BitmapImage(new Uri(_sourceName));
				}
				_imageChanged = true;
			}
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		private void OnRemovePicture()
		{
			_drawing = null;
			ImageSource = null;
			_imageChanged = true;
		}
		private bool CanRemovePicture()
		{
			return ImageSource != null;
		}

		public override string Header
		{
			get { return "Изображение"; }
		}
		public override void CopyProperties()
		{
			Stretch = _layoutPartImageViewModel.Stretch;
			_sourceName = null;
			_drawing = null;
			ImageSource = _layoutPartImageViewModel.ImageSource;
			_imageChanged = false;
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartImageProperties)_layoutPartImageViewModel.Properties;
			if (properties.Stretch != Stretch || _imageChanged)
			{
				if (_imageChanged)
				{
					if (properties.ReferenceUID != Guid.Empty)
						ServiceFactoryBase.ContentService.RemoveContent(properties.ReferenceUID);
					if (!string.IsNullOrEmpty(_sourceName))
					{
						if (_drawing == null)
							properties.ReferenceUID = ServiceFactoryBase.ContentService.AddContent(_sourceName);
						else
							properties.ReferenceUID = ServiceFactoryBase.ContentService.AddContent(_drawing);
					}
					else
						properties.ReferenceUID = Guid.Empty;
					properties.IsVectorImage = _drawing != null;
				}
				properties.Stretch = Stretch;
				_layoutPartImageViewModel.UpdateLayoutPart();
				return true;
			}
			return false;
		}

	}
}