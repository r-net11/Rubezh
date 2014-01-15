using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using XFiresecAPI;

namespace SkudModule.ViewModels
{
	public class FrameViewModel : BaseViewModel
	{
		public static readonly string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";

		public FrameViewModel(SKDLibraryFrame skdLibraryFrame)
		{
			Frame = skdLibraryFrame;
			ImportSvgCommand = new RelayCommand(OnImportSvg);
			ExportSvgCommand = new RelayCommand(OnExportSvg);
		}

		public SKDLibraryFrame Frame { get; private set; }

		public string Image
		{
			get { return Frame.Image; }
			set
			{
				Frame.Image = value;
				OnPropertyChanged("Image");
				ServiceFactory.SaveService.SKDLibraryChanged = true;
			}
		}

		public int Duration
		{
			get { return Frame.Duration; }
			set
			{
				if (value != Frame.Duration)
				{
					Frame.Duration = value;
					ServiceFactory.SaveService.SKDLibraryChanged = true;
				}
			}
		}

		public Canvas XamlOfImage
		{
			get
			{
				try { return ImageConverters.Xml2Canvas(Frame.Image); }
				catch { return ImageConverters.Xml2Canvas(ErrorFrame); }
			}
		}

		public RelayCommand ImportSvgCommand { get; private set; }
		void OnImportSvg()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Svg Files (.svg)|*.svg"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				Frame.Image = ImageConverters.Svg2Xaml(openFileDialog.FileName);
				OnPropertyChanged("XamlOfImage");
				LibraryViewModel.Current.SelectedState = LibraryViewModel.Current.SelectedState;
				ServiceFactory.SaveService.SKDLibraryChanged = true;
			}
		}

		public RelayCommand ExportSvgCommand { get; private set; }
		void OnExportSvg()
		{
			var saveFileDialog = new SaveFileDialog()
			{
				Filter = "Svg Files (.svg)|*.svg",
				RestoreDirectory = true,
				DefaultExt = "Svg Files (.svg)|*.svg",
				ValidateNames = true
			};

			if (saveFileDialog.ShowDialog() == true)
				ImageConverters.Xaml2Svg(Frame.Image, saveFileDialog.FileName);
		}
	}
}