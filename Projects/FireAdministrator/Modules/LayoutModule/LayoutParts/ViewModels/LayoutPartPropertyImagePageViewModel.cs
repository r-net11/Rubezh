using Common;
using Infrastructure.Client.Converters;
using Infrastructure.Client.Images;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Layout;
using Microsoft.Win32;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Infrustructure.Plans;
using Stretch = System.Windows.Media.Stretch;

namespace LayoutModule.LayoutParts.ViewModels
{
	public class LayoutPartPropertyImagePageViewModel : LayoutPartPropertyPageViewModel
	{
		LayoutPartImageViewModel _layoutPartImageViewModel;
		string _sourceName;
		bool _imageChanged;
		DrawingGroup _drawing;
		WMFImage _wmf;
		byte[] _svg;

		public LayoutPartPropertyImagePageViewModel(LayoutPartImageViewModel layoutPartImageViewModel)
		{
			_layoutPartImageViewModel = layoutPartImageViewModel;
			StretchTypes = new ObservableCollection<Stretch>(Enum.GetValues(typeof(Stretch)).Cast<Stretch>());
			SelectPictureCommand = new RelayCommand(OnSelectPicture);
			RemovePictureCommand = new RelayCommand(OnRemovePicture, CanRemovePicture);
		}

		public ObservableCollection<Stretch> StretchTypes { get; private set; }
		Stretch _stretch;
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
		TileBrush _imageBrush;
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
		void UpdateStretch()
		{
			if (ImageBrush != null)
				ImageBrush.Stretch = Stretch;
		}

		public RelayCommand SelectPictureCommand { get; private set; }
		void OnSelectPicture()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = ImageExtensions.GraphicFilter;
			if (openFileDialog.ShowDialog().Value)
				using (new WaitWrapper())
				{
					_sourceName = openFileDialog.FileName;
					if (ImageExtensions.IsSVGGraphics(_sourceName))
					{
						_drawing = SVGConverters.ReadDrawing(_sourceName);
						_wmf = null;
						ImageBrush = new DrawingBrush(_drawing);
						_svg = File.ReadAllBytes(_sourceName);
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
		}

		public RelayCommand RemovePictureCommand { get; private set; }
		void OnRemovePicture()
		{
			_drawing = null;
			_wmf = null;
			ImageBrush = null;
			_imageChanged = true;
			_svg = null;
		}
		bool CanRemovePicture()
		{
			return ImageBrush != null;
		}

		public override string Header
		{
			get { return "Изображение"; }
		}
		public override void CopyProperties()
		{
			_sourceName = null;
			_drawing = null;
			_wmf = null;
			var properties = (LayoutPartImageProperties)_layoutPartImageViewModel.Properties;
			ImageBrush = ImageHelper.GetResourceBrush(properties.ReferenceUID, properties.ImageType);
			Stretch = properties.Stretch.ToWindowsStretch();
			_imageChanged = false;
		}
		public override bool CanSave()
		{
			return true;
		}
		public override bool Save()
		{
			var properties = (LayoutPartImageProperties)_layoutPartImageViewModel.Properties;
			if (properties.Stretch != Stretch.ToRubezhStretch() || _imageChanged)
			{
				if (_imageChanged)
					using (new WaitWrapper())
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
				if (_svg != null)
					properties.ReferenceSVGUID = ServiceFactoryBase.ContentService.AddContent(_svg);
				else
					properties.ReferenceSVGUID = null;
				properties.Stretch = Stretch.ToRubezhStretch();
				_layoutPartImageViewModel.ImageBrush = ImageBrush;
				return true;
			}
			return false;
		}
	}
}