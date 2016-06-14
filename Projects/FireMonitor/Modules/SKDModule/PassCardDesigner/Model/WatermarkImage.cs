using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SKDModule.PassCardDesigner.Model
{
	public class WatermarkImage : BaseViewModel
	{
		private byte[] _image;
		private Guid? _guid;

		public Guid? OriginalImageGuid { get; set; }

		public byte[] Image
		{
			get { return _image; }
			set
			{
				_image = value;
				OnPropertyChanged(() => Image);
			}
		}

		public Guid? Guid
		{
			get { return _guid; }
			private set
			{
				_guid = value;
				OnPropertyChanged(() => Guid);
			}
		}

		public WatermarkImage()
		{
			Guid = System.Guid.NewGuid();
		}

		public WatermarkImage(StrazhAPI.SKD.WatermarkImage image)
		{
			if (image == null) return;

			Image = image.ImageContent;
			Guid = image.Guid;
			OriginalImageGuid = image.OriginalImageGuid;
		}

		public void RemovePreviousImage()
		{
			if (Guid.HasValue && Guid.Value != System.Guid.Empty)
				ServiceFactoryBase.ContentService.RemoveContent(Guid.Value);
		}

		public StrazhAPI.SKD.WatermarkImage ToDTO()
		{
			if (!Guid.HasValue) return null;

			return new StrazhAPI.SKD.WatermarkImage
			{
				ImageContent = Image,
				Guid = Guid.Value,
				OriginalImageGuid = OriginalImageGuid
			};
		}
	}
}
