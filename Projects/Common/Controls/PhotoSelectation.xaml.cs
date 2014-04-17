using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Infrastructure.Common.Windows;
using Microsoft.Win32;

namespace Controls
{
	/// <summary>
	/// Логика взаимодействия для PhotoSelectation.xaml
	/// </summary>
	public partial class PhotoSelectation : UserControl
	{
		public static readonly DependencyProperty PhotoProperty =
			DependencyProperty.Register("Photo", typeof(byte[]), typeof(PhotoSelectation),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnPhotoPropertyChanged)));

		private static void OnPhotoPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
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
			if (Photo != null)
			{
				try
				{
					PhotoImage.Source = BitmapFrame.Create(new MemoryStream(Photo));
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

		public byte[] Photo
		{
			get { return (byte[])GetValue(PhotoProperty); }
			set { SetValue(PhotoProperty, value); }
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Jpg files (.jpg)|*.jpg"
			};
			if (openFileDialog.ShowDialog() == true)
			{
				var memoryStream = new MemoryStream();
				openFileDialog.OpenFile().CopyTo(memoryStream);
				Photo = memoryStream.ToArray();
			}
		}
	}
}
