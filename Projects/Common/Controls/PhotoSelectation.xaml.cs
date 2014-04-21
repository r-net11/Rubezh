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

		private static void OnDataPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
		{
			PhotoSelectation photoSelectation = dp as PhotoSelectation;
			if (photoSelectation != null)
				photoSelectation.UpdatePhoto();
		}
		
		public PhotoSelectation()
		{
			InitializeComponent();
			UpdatePhoto();
		}

		public void UpdatePhoto()
		{
			if (Data != null)
			{
				try
				{
					PhotoImage.Source = BitmapFrame.Create(new MemoryStream(Data));
					PhotoImage.Stretch = System.Windows.Media.Stretch.UniformToFill;
				}
				catch(Exception)
				{
					MessageBoxService.Show("Не могу загрузить фото");
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

		void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Jpg files (.jpg)|*.jpg"
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
			MessageBoxService.Show("NOT IMPLEMENTED YET");
			//IDataObject data = Clipboard.GetDataObject();
		}

		void ScannerButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxService.Show("NOT IMPLEMENTED YET");
		}

		void WebCamButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxService.Show("NOT IMPLEMENTED YET");
		}
	}
}
