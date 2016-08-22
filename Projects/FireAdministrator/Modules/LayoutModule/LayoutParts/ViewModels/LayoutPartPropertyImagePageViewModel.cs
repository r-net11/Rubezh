using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using Localization.Layout.ViewModels;
using StrazhAPI.Models.Layouts;
using Infrastructure.Client.Converters;
using Infrastructure.Client.Images;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Microsoft.Win32;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartPropertyImagePageViewModel : LayoutPartPropertyPageViewModel
	{
		private LayoutPartImageViewModel _layoutPartImageViewModel;
		private string _sourceName;
		private bool _imageChanged;
		private DrawingGroup _drawing;
		private WMFImage _wmf;

		public LayoutPartPropertyImagePageViewModel(LayoutPartImageViewModel layoutPartImageViewModel)
		{
			_layoutPartImageViewModel = layoutPartImageViewModel;
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
				UpdateStretch();
				OnPropertyChanged(() => Stretch);
			}
		}
		private TileBrush _imageBrush;
		public TileBrush ImageBrush
		{
			get { return _imageBrush; }
			set
			{
				_imageBrush = value;
				UpdateStretch();
				OnPropertyChanged(() => ImageBrush);
			}
		}
		private void UpdateStretch()
		{
			if (ImageBrush != null)
				ImageBrush.Stretch = Stretch;
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		private void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = ImageExtensions.GraphicFilter;
			if (!openFileDialog.ShowDialog().Value) return;

			_sourceName = openFileDialog.FileName;
			if (ImageExtensions.IsSVGGraphics(_sourceName))
			{
				_drawing = SVGConverters.ReadDrawing(_sourceName);
				_wmf = null;
				ImageBrush = new DrawingBrush(_drawing);
			}
			else if (ImageExtensions.IsWMFGraphics(_sourceName))
			{
				_wmf = WMFConverter.ReadWMF(_sourceName);
				_drawing = _wmf == null ? null : _wmf.ToDrawing();
				if (_drawing == null)
					ImageBrush = new VisualBrush(_wmf.Canvas);
				else
				{
					_wmf = null;
					ImageBrush = new DrawingBrush(_drawing);
				}
			}
			else
			{
				_drawing = null;
				_wmf = null;
				ImageBrush = new ImageBrush(new BitmapImage(new Uri(_sourceName)));
			}
			_imageChanged = true;
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		private void OnRemovePicture()
		{
			_drawing = null;
			_wmf = null;
			ImageBrush = null;
			_imageChanged = true;
		}
		private bool CanRemovePicture()
		{
			return ImageBrush != null;
		}

		public override string Header
		{
			get { return CommonViewModels.LayoutPartPropertyImagePage_Header; }
		}
		public override void CopyProperties()
		{
			_sourceName = null;
			_drawing = null;
			_wmf = null;
			var properties = (LayoutPartImageProperties)_layoutPartImageViewModel.Properties;
			ImageBrush = ImageHelper.GetResourceBrush(properties.ReferenceUID, properties.ImageType);
			Stretch = properties.Stretch;
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
						if (_drawing != null)
						{
							properties.ReferenceUID = ServiceFactoryBase.ContentService.AddContent(_drawing);
							properties.ImageType = ResourceType.Drawing;
						}
						else if (_wmf != null)
						{
							properties.ReferenceUID = ServiceFactoryBase.ContentService.AddContent(_wmf.Canvas);
							properties.ImageType = ResourceType.Visual;
						}
						else
						{
							properties.ReferenceUID = ServiceFactoryBase.ContentService.AddContent(_sourceName);
							properties.ImageType = ResourceType.Image;
						}
					}
					else
						properties.ReferenceUID = Guid.Empty;
				}
				properties.Stretch = Stretch;
				_layoutPartImageViewModel.ImageBrush = ImageBrush;
				return true;
			}
			return false;
		}
	}
}