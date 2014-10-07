using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Infrastructure.Common.Windows;
using Microsoft.Win32;

namespace Controls
{
	public partial class PhotoSelectation : UserControl
	{
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(byte[]), typeof(PhotoSelectation),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDataPropertyChanged)));

		public static readonly DependencyProperty CanEditProperty =
			DependencyProperty.Register("CanEdit", typeof(bool), typeof(PhotoSelectation),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnCanEditPropertyChanged)));

		private static void OnDataPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			PhotoSelectation photoSelectation = dp as PhotoSelectation;
			if (photoSelectation != null)
				photoSelectation.UpdatePhoto();
		}

		private static void OnCanEditPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			PhotoSelectation photoSelectation = dp as PhotoSelectation;
			if (photoSelectation != null)
			{

				if ((bool)e.NewValue)
				{
					photoSelectation._stackPanel.Visibility = Visibility.Visible;
				}
				else
				{
					photoSelectation._stackPanel.Visibility = Visibility.Collapsed;
				}
			}
		}

		public PhotoSelectation()
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
					MessageBoxService.ShowExtended("Невозможно загрузить фото");
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
				Filter = "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff"
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
				if (Path.GetExtension(fileName) == "" || System.Drawing.Image.FromFile(fileName) != null)
					MessageBoxService.ShowExtended("В буфере обмена не отсутствуют изображения");
				else
					Data = System.IO.File.ReadAllBytes(fileName);
			}
		}

		void ScannerButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxService.ShowExtended("NOT IMPLEMENTED YET");
		}

		void WebCamButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxService.ShowExtended("NOT IMPLEMENTED YET");
		}
	}
}