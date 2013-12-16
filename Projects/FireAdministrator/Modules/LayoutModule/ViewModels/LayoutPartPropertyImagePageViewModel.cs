using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Services.Layout;
using System.Collections.ObjectModel;
using System.Windows;
using SharpVectors.Renderers.Wpf;
using Infrastructure.Common;
using Microsoft.Win32;
using SharpVectors.Converters;
using Infrastructure.Common.Services;
using System.IO;
using System.Windows.Media;
using FiresecAPI.Models.Layouts;
using Common;
using Infrastructure.Common.Windows;
using System.Windows.Media.Imaging;

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
			UpdateLayoutPart();
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
			var properties = (LayoutPartImageProperties)_layoutPartImageViewModel.Properties;
			Stretch = properties.Stretch;
			_sourceName = null;
			_drawing = null;
			ImageSource = GetImage(properties.SourceUID, properties.IsVectorImage);
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
					if (properties.SourceUID != Guid.Empty)
						ServiceFactoryBase.ContentService.RemoveContent(properties.SourceUID);
					if (!string.IsNullOrEmpty(_sourceName))
					{
						if (_drawing == null)
							properties.SourceUID = ServiceFactoryBase.ContentService.AddContent(_sourceName);
						else
							properties.SourceUID = ServiceFactoryBase.ContentService.AddContent(_drawing);
					}
					else
						properties.SourceUID = Guid.Empty;
					properties.IsVectorImage = _drawing != null;
				}
				properties.Stretch = Stretch;
				UpdateLayoutPart();
				return true;
			}
			return false;
		}

		private void UpdateLayoutPart()
		{
			var properties = (LayoutPartImageProperties)_layoutPartImageViewModel.Properties;
			_layoutPartImageViewModel.Stretch = properties.Stretch;
			_layoutPartImageViewModel.ImageSource = GetImage(properties.SourceUID, properties.IsVectorImage);
		}
		private ImageSource GetImage(Guid uid, bool isVector)
		{
			ImageSource imageSource = null;
			if (uid != Guid.Empty)
				try
				{
					if (isVector)
						imageSource = new DrawingImage(ServiceFactoryBase.ContentService.GetDrawing(uid));
					else
						imageSource = ServiceFactoryBase.ContentService.GetBitmapContent(uid);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове LayoutPartPropertyImagePageViewModel.UpdateImage");
					MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
				}
			return imageSource;
		}
	}
}
