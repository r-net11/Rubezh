using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Infrastructure.Common.Windows.Windows;
using Microsoft.Win32;
using SKDModule.Views;
using SKDModule.ViewModels;
using WPFMediaKit.DirectShow.Interop;

namespace SKDModule
{
	public partial class PhotoSelection : UserControl
	{
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(byte[]), typeof(PhotoSelection),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDataPropertyChanged)));

		public static readonly DependencyProperty CanEditProperty =
			DependencyProperty.Register("CanEdit", typeof(bool), typeof(PhotoSelection),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnCanEditPropertyChanged)));

		private static void OnDataPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			PhotoSelection photoSelection = dp as PhotoSelection;
			if (photoSelection != null)
				photoSelection.UpdatePhoto();
		}

		private static void OnCanEditPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			PhotoSelection photoSelection = dp as PhotoSelection;
			if (photoSelection != null)
			{

				if ((bool)e.NewValue)
				{
					photoSelection._stackPanel.Visibility = Visibility.Visible;
				}
				else
				{
					photoSelection._stackPanel.Visibility = Visibility.Collapsed;
				}
			}
		}

		public PhotoSelection()
		{
			InitializeComponent();
			UpdatePhoto();
		}

		public void UpdatePhoto()
		{
			if (Data != null && Data.Length > 0)
			{
				try
				{
					PhotoImage.Source = BitmapFrame.Create(new MemoryStream(Data));
					PhotoImage.Stretch = System.Windows.Media.Stretch.Uniform;
				}
				catch (Exception)
				{
					MessageBoxService.ShowWarning("Невозможно загрузить фото");
					SetNoPhoto();
				}
			}
			else
				SetNoPhoto();
		}

		void SetNoPhoto()
		{
			PhotoImage.Source = new BitmapImage(new Uri("pack://application:,,,/Controls;component/Images/NoPhoto.png"));
			PhotoImage.Stretch = System.Windows.Media.Stretch.None;
		}

		public byte[] Data
		{
			get { return (byte[])GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}

		public bool CanEdit
		{
			get { return (bool)GetValue(CanEditProperty); }
			set { SetValue(CanEditProperty, value); }
		}

		void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff;*.gif"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				var memoryStream = new MemoryStream();
				openFileDialog.OpenFile().CopyTo(memoryStream);
				Data = memoryStream.ToArray();
			}
		}

		void PasteButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var image = Clipboard.GetImage();
				if (image != null)
				{
					var stream = new MemoryStream();
					var encoder = new JpegBitmapEncoder();
					encoder.QualityLevel = 100;
					encoder.Frames.Add(BitmapFrame.Create(image));
					encoder.Save(stream);
					Data = stream.ToArray();
					stream.Close();
					return;
				}
				string fileName = null;
				foreach (var item in Clipboard.GetFileDropList())
				{
					fileName = item;
					break;
				}
				if (fileName != null)
				{
					if (Path.GetExtension(fileName) == "" || System.Drawing.Image.FromFile(fileName) == null)
						MessageBoxService.Show("В буфере обмена отсутствуют изображения");
					else
						Data = System.IO.File.ReadAllBytes(fileName);
				}
			}
			catch (Exception)
			{
				MessageBoxService.Show("Ошибка при загрузке файла из буфера обмена");
			}
		}

		void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			Data = null;
		}

		void ScannerButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxService.Show("NOT IMPLEMENTED YET");
		}

		void WebCamButton_Click(object sender, RoutedEventArgs e)
		{
			if (WPFMediaKit.DirectShow.Controls.MultimediaUtil.VideoInputDevices.Length > 0)
			{
				var webCameraViewModel = new WebCameraDetailsViewModel();
				if (DialogService.ShowModalWindow(webCameraViewModel))
				{
					Data = webCameraViewModel.Data;
				}
			} 
			else
			{
				MessageBoxService.Show("Устройство не обнаружено. Подключите камеру и перезапустите приложение.");
			}
		}
	}
}