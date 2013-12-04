using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Client.Layout.ViewModels;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Infrastructure.Common;
using Microsoft.Win32;
using SharpVectors.Renderers.Wpf;
using Infrastructure.Common.Services;
using Common;
using Infrastructure.Common.Windows;
using System.IO;
using SharpVectors.Converters;

namespace LayoutModule.ViewModels
{
	public class LayoutPartImageViewModel : BaseLayoutPartViewModel
	{
		private const string VectorGraphicExtensions = ".svg;.svgx";
		private WpfDrawingSettings _settings;
		private bool _initialized;

		private LayoutPartImageProperties _properties;
		public LayoutPartImageViewModel()
		{
			_imageSource = null;
			_initialized = false;
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

		public override ILayoutProperties GetProperties()
		{
			return _properties;
		}
		public override void SetProperties(ILayoutProperties properties)
		{
			_properties = properties as LayoutPartImageProperties ?? new LayoutPartImageProperties();
			Stretch = _properties.Stretch;
			UpdateImage();
			_initialized = true;
		}

		public ObservableCollection<Stretch> StretchTypes { get; private set; }
		private Stretch _stretch;
		public Stretch Stretch
		{
			get { return _stretch; }
			set
			{
				_stretch = value;
				_properties.Stretch = value;
				OnPropertyChanged(() => Stretch);
				if (_initialized)
					LayoutDesignerViewModel.Instance.LayoutConfigurationChanged(false);
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
				if (_properties.SourceUID != Guid.Empty)
					ServiceFactoryBase.ContentService.RemoveContent(_properties.SourceUID);
				_properties.IsVectorImage = VectorGraphicExtensions.Contains(Path.GetExtension(openFileDialog.FileName));
				if (_properties.IsVectorImage)
				{
					DrawingGroup drawing = null;
					using (FileSvgReader reader = new FileSvgReader(_settings))
						drawing = reader.Read(openFileDialog.FileName);
					_properties.SourceUID = ServiceFactoryBase.ContentService.AddContent(drawing);
				}
				else
					_properties.SourceUID = ServiceFactoryBase.ContentService.AddContent(openFileDialog.FileName);
				UpdateImage();
				LayoutDesignerViewModel.Instance.LayoutConfigurationChanged(false);
			}
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		private void OnRemovePicture()
		{
			if (_properties.SourceUID != Guid.Empty)
				ServiceFactoryBase.ContentService.RemoveContent(_properties.SourceUID);
			_properties.SourceUID = Guid.Empty;
			_properties.IsVectorImage = false;
			UpdateImage();
			LayoutDesignerViewModel.Instance.LayoutConfigurationChanged(false);
		}
		private bool CanRemovePicture()
		{
			return _properties.SourceUID != Guid.Empty;
		}

		private void UpdateImage()
		{
			try
			{
				if (_properties.SourceUID == Guid.Empty)
					ImageSource = null;
				else
				{
					if (_properties.IsVectorImage)
						ImageSource = new DrawingImage(ServiceFactoryBase.ContentService.GetDrawing(_properties.SourceUID));
					else
						ImageSource = ServiceFactoryBase.ContentService.GetBitmapContent(_properties.SourceUID);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове LayoutPartImageViewModel.UpdateImage");
				MessageBoxService.ShowWarning("Возникла ошибка при загрузке изображения");
			}
		}
	}
}
