using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class GraphicsColumnViewModel : BaseViewModel, IGraphicsColumnViewModel
	{
		public string Name { get; private set; }
		bool HasPhoto { get { return AdditionalColumn.Photo != null && AdditionalColumn.Photo.Data != null; } }
		public AdditionalColumn AdditionalColumn { get; private set; }

		public byte[] Data
		{
			get
			{
				if (!HasPhoto)
					return null;
				return AdditionalColumn.Photo.Data;
			}
			set
			{
				AdditionalColumn.Photo = new Photo();
				AdditionalColumn.Photo.Data = value;
				OnPropertyChanged(() => Data);
				OnPropertyChanged(() => HasPhoto);
			}
		}

		public GraphicsColumnViewModel(AdditionalColumn additionalColumn)
		{
			AdditionalColumn = additionalColumn;
			Name = AdditionalColumn.AdditionalColumnType.Name;
		}
	}

	public class PhotoColumnViewModel : BaseViewModel, IGraphicsColumnViewModel
	{
		public string Name { get; private set; }
		bool HasPhoto { get { return Photo != null && Photo.Data != null; } }
		public Photo Photo { get; private set; }

		public byte[] Data
		{
			get
			{
				if (!HasPhoto)
					return null;
				return Photo.Data;
			}
			set
			{
				if (Photo == null)
					Photo = new Photo();
				Photo.Data = value;
				OnPropertyChanged(() => Data);
				OnPropertyChanged(() => HasPhoto);
			}
		}

		public PhotoColumnViewModel(Photo photo)
		{
			Photo = photo;
			Name = "Фото";
		}
	}

	public interface IGraphicsColumnViewModel
	{
		string Name { get; }
		byte[] Data { get; set; }
	}
}