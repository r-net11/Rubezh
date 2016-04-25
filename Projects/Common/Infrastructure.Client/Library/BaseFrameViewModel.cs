using Infrastructure.Client.Converters;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using RubezhAPI.Plans.Devices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Infrastructure.Client.Library
{
	public abstract class BaseFrameViewModel<TFrame> : BaseViewModel
		where TFrame : ILibraryFrame
	{
		public static readonly string ErrorFrame = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n<Border BorderBrush=\"Red\" BorderThickness=\"5\" Padding=\"20\">\n<TextBlock Text=\"Error Xaml Code\" FontSize=\"60\" />\n</Border>\n</Canvas>";

		public BaseFrameViewModel(TFrame libraryFrame)
		{
			Frame = libraryFrame;
			ImportSvgCommand = new RelayCommand(OnImportSvg);
			ExportSvgCommand = new RelayCommand(OnExportSvg);
		}

		public TFrame Frame { get; private set; }

		public string Image
		{
			get { return Frame.Image; }
			set
			{
				Frame.Image = value;
				OnPropertyChanged(() => Image);
				OnChanged();
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
					OnChanged();
				}
			}
		}

		Canvas _xamlOfImage;
		public Canvas XamlOfImage
		{
			get
			{
				if (_xamlOfImage == null)
					_xamlOfImage = SVGConverters.Xml2Canvas(Frame.Image) ?? SVGConverters.Xml2Canvas(ErrorFrame);
				return _xamlOfImage;
			}
		}

		CancellationTokenSource tockenSource;

		void Refresh()
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
						InvalidatePreview();
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
				var result = SVGConverters.Svg2Xaml2(openFileDialog.FileName);
				if (result == null)
				{
					result = SVGConverters.Svg2Xaml(openFileDialog.FileName);
					if (result == null)
					{
						MessageBoxService.ShowError("Ошибка при конвертировании файла");
						return;
					}
				}
				Frame.Image = result;
				OnPropertyChanged(() => XamlOfImage);
				InvalidatePreview();
				OnChanged();
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
				SVGConverters.Xaml2Svg(Frame.Image, saveFileDialog.FileName);
		}

		protected abstract void OnChanged();
		protected abstract void InvalidatePreview();
	}
}