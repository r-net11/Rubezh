using Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans;
using Localization.SKD.Errors;
using ReactiveUI;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SKDModule.Properties;

namespace SKDModule.PassCardDesigner.Model
{
	public class PreviewImage : BaseViewModel
	{
		#region Fields
		private Color _backgroundColor;
		private Color _borderColor;
		private double _borderThickness;
		private double _previewWidth;
		private double _previewHeight;
		private TileBrush _imageBrush;
		private SolidColorBrush _borderBrush;
		private bool _isUseImage;
		#endregion

		#region Properties
		public double PreviewWidth
		{
			get { return _previewWidth; }
			set
			{
				_previewWidth = value;
				OnPropertyChanged(() => PreviewWidth);
			}
		}

		public double PreviewHeight
		{
			get { return _previewHeight; }
			set
			{
				_previewHeight = value;
				OnPropertyChanged(() => PreviewHeight);
			}
		}

		public bool IsUseImage
		{
			get { return _isUseImage; }
			set
			{
				if (_isUseImage == value) return;

				_isUseImage = value;
				OnPropertyChanged(() => IsUseImage);
			}
		}

		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				if (_backgroundColor == value) return;

				_backgroundColor = value;
				OnPropertyChanged(() => BackgroundColor);
			}
		}

		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				if (_borderColor == value) return;

				_borderColor = value;
				OnPropertyChanged(() => BorderColor);
			}
		}

		public double BorderThickness
		{
			get { return _borderThickness; }
			set
			{
				_borderThickness = value;
				OnPropertyChanged(() => BorderThickness);
			}
		}

		public TileBrush ImageBrush
		{
			get { return _imageBrush; }
			set
			{
				_imageBrush = value;
				OnPropertyChanged(() => ImageBrush);
			}
		}

		public SolidColorBrush BorderBrush
		{
			get { return _borderBrush; }
			set
			{
				if (Equals(_borderBrush, value)) return;

				_borderBrush = value;
				OnPropertyChanged(() => BorderBrush);
			}
		}
		#endregion

		public PreviewImage()
		{
			this.WhenAny(x => x.BorderColor, x => x.Value)
			.Subscribe(value =>
			{
				BorderBrush = new SolidColorBrush(value);
			});

			this.WhenAny(x => x.BackgroundColor, x => x.Value)
				.Subscribe(value =>
				{
					if (!IsUseImage)
						ImageBrush = new ImageBrush(ImageBuilderHelper.CreateBitmapSource(BackgroundColor));
				});

			this.WhenAny(x => x.ImageBrush, x => x.Value)
				.Subscribe(value =>
				{
					if (value == null)
						ImageBrush = new ImageBrush(ImageBuilderHelper.CreateBitmapSource(BackgroundColor));
				});
		}

		public PreviewImage(StrazhAPI.SKD.PreviewImage preview) : this()
		{
			if (preview == null)
			{
				BackgroundColor = Colors.White;
				return;
			}

			BackgroundColor = preview.BackgroundColor.ToWindowsColor();
			BorderColor = preview.BorderColor.GetValueOrDefault().ToWindowsColor();
			BorderThickness = preview.BorderThickness;
		}

		public void SetPreviewProportions(int maxWidth, int maxHeight, double currentWidth, double currentHeight)
		{
			PreviewWidth = ProportionHelper.CalculateProportionWidth(maxWidth, maxHeight, currentWidth, currentHeight);
			PreviewHeight = ProportionHelper.CalculateProportionHeight(maxWidth, maxHeight, currentWidth, currentHeight);
		}

		public bool SetImageFrom(Uri uri)
		{
			try
			{
				ImageBrush = new ImageBrush(new BitmapImage(uri));
				IsUseImage = true;
			}
			catch (Exception e)
			{
				MessageBoxService.ShowError(CommonErrors.ImageNotFound_Error);
				Logger.Error(e);
				return false;
			}

			return true;
		}

		public void ClearImage()
		{
			ImageBrush = null;
			IsUseImage = false;
		}

		public StrazhAPI.SKD.PreviewImage ToDTO()
		{
			return new StrazhAPI.SKD.PreviewImage
			{
				BackgroundColor = BackgroundColor.ToStruzhColor(),
				BorderColor = BorderColor.ToStruzhColor(),
				BorderThickness = BorderThickness
			};
		}
	}
}
