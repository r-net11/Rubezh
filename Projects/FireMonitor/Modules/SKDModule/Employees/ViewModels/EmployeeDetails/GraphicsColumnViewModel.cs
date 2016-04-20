using RubezhAPI.SKD;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class GraphicsColumnViewModel : BaseViewModel, IGraphicsColumnViewModel
	{
		Employee Employee;
		public AdditionalColumn AdditionalColumn { get; private set; }
		public AdditionalColumnType AdditionalColumnType { get; private set; }
		public string Name
		{
			get { return AdditionalColumnType.Name; }
		}
		bool HasPhoto { get { return AdditionalColumn.Photo != null && AdditionalColumn.Photo.Data != null; } }
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

		public GraphicsColumnViewModel(AdditionalColumnType additionalColumnType, Employee employee, AdditionalColumn additionalColumn = null)
		{
			AdditionalColumnType = additionalColumnType;
			Employee = employee;
			AdditionalColumn = additionalColumn != null ? additionalColumn :
				new AdditionalColumn { AdditionalColumnTypeUID = AdditionalColumnType.UID, ColumnName = additionalColumnType.Name, EmployeeUID = Employee.UID, Photo = new Photo() };
		}
	}

	public class PhotoColumnViewModel : BaseViewModel, IGraphicsColumnViewModel
	{
		public string Name { get; private set; }
		public bool HasPhoto { get { return Photo != null && Photo.Data != null; } }
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
			OnPropertyChanged(() => Data);
			OnPropertyChanged(() => HasPhoto);
		}
	}

	public interface IGraphicsColumnViewModel
	{
		string Name { get; }
		byte[] Data { get; set; }
	}
}