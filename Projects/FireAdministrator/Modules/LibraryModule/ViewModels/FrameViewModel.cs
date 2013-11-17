using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

namespace LibraryModule.ViewModels
{
	public class FrameViewModel : BaseViewModel
	{
		public static readonly string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";

		public FrameViewModel(LibraryFrame libraryFrame)
		{
			Frame = libraryFrame;
			ImportSvgCommand = new RelayCommand(OnImportSvg);
			ExportSvgCommand = new RelayCommand(OnExportSvg);
		}

		public LibraryFrame Frame { get; private set; }

		public string Image
		{
			get { return Frame.Image; }
			set
			{
				Frame.Image = value;
				OnPropertyChanged("Image");
				ServiceFactory.SaveService.XLibraryChanged = true;
				Refresh();
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
					ServiceFactory.SaveService.LibraryChanged = true;
				}
			}
		}

		private Canvas _xamlOfImage;
		public Canvas XamlOfImage
		{
			get
			{
				if (_xamlOfImage == null)
					_xamlOfImage = ImageConverters.Xml2Canvas(Frame.Image) ?? ImageConverters.Xml2Canvas(ErrorFrame);
				return _xamlOfImage;
			}
		}
		private CancellationTokenSource tockenSource;
		private void Refresh()
		{
			if (tockenSource != null)
				tockenSource.Cancel();
			tockenSource = new CancellationTokenSource();
			TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			Task.Factory.StartNew((tocken) =>
				{
					Thread.Sleep(1000);
					return tocken;
				}, (object)tockenSource.Token, tockenSource.Token, TaskCreationOptions.None, TaskScheduler.Default).
				ContinueWith((task) =>
				{
					if (!((CancellationToken)task.AsyncState).IsCancellationRequested)
					{
						System.Console.WriteLine("REFRESH !!!");
						_xamlOfImage = null;
						OnPropertyChanged(() => XamlOfImage);
						LibraryViewModel.Current.InvalidatePreview();
					}
				}, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, uiScheduler);
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
				var result = ImageConverters.Svg2Xaml2(openFileDialog.FileName);
				if (result == null)
				{
					result = ImageConverters.Svg2Xaml(openFileDialog.FileName);
					if (result == null)
					{
						MessageBoxService.ShowError("Ошибка при конвертировании файла");
						return;
					}
				}
				Frame.Image = result;
				OnPropertyChanged("XamlOfImage");
				LibraryViewModel.Current.SelectedState = LibraryViewModel.Current.SelectedState;
				ServiceFactory.SaveService.LibraryChanged = true;
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