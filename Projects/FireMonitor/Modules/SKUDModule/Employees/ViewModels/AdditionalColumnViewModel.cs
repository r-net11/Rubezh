using System.Windows.Media.Imaging;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using System;
using System.IO;

namespace SKDModule.ViewModels
{

	public class AdditionalColumnViewModel : BaseViewModel
	{
		AdditionalColumn AdditionalColumn;
		public string Name { get; private set; }
		public bool IsGraphicsData { get; private set; }
		public BitmapSource Bitmap { get; private set; }
		public string Text
		{
			get
			{
				if (IsGraphicsData)
					return "";
				return AdditionalColumn.TextData;
			}
		}
		
		public AdditionalColumnViewModel(AdditionalColumn additionalColumn)
		{
			AdditionalColumn = additionalColumn;
			Name = AdditionalColumn.AdditionalColumnType.Name;
			IsGraphicsData = AdditionalColumn.AdditionalColumnType.DataType == AdditionalColumnDataType.Graphics;
			EditCommand = new RelayCommand(OnEdit);
			if (IsGraphicsData)
				Bitmap = PhotoHelper.GetBitmapSource(additionalColumn.Photo);
			//else
			//	Text = AdditionalColumn.TextData;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			if (!IsGraphicsData)
			{
				var editTextViewModel = new EditTextViewModel(Name, Text);
				if (DialogService.ShowModalWindow(editTextViewModel))
				{
					AdditionalColumn.TextData = editTextViewModel.Text;
					OnPropertyChanged(() => Text);
				}
			}
			else
			{
				var openFileDialog = new OpenFileDialog()
				{
					Filter = "Jpg files (.jpg)|*.jpg"
				};
				if (openFileDialog.ShowDialog() == true)
				{
					var fileStream = openFileDialog.OpenFile();
					var memoryStream = new MemoryStream();
					fileStream.CopyTo(memoryStream);
					byte[] bytes = memoryStream.ToArray();
				}
			}
		}
	}
}
