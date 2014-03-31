using System.Windows.Media.Imaging;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{

	public class AdditionalColumnViewModel : BaseViewModel
	{
		AdditionalColumn AdditionalColumn;
		public string Name { get; private set; }
		public bool IsGraphicsData { get; private set; }
		public BitmapSource Bitmap { get; private set; }
		public string Text { get; private set; }

		public AdditionalColumnViewModel(AdditionalColumn additionalColumn)
		{
			AdditionalColumn = additionalColumn;
			Name = AdditionalColumn.AdditionalColumnType.Name;
			IsGraphicsData = AdditionalColumn.AdditionalColumnType.DataType == AdditionalColumnDataType.Graphics;
			if (IsGraphicsData)
				Bitmap = PhotoHelper.GetBitmapSource(additionalColumn.Photo);
			else
				Text = AdditionalColumn.TextData;
		}
	}
}
