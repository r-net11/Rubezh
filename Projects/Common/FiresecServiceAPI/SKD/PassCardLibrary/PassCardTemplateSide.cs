using Common;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	/// <summary>
	/// Сущность-сторона шаблона пропусков.
	/// </summary>
	[DataContract]
	public class PassCardTemplateSide : OrganisationElementBase
	{
		public PassCardTemplateSide()
		{
			UID = Guid.NewGuid();
			PreviewImage = new PreviewImage();
		}

		[DataMember]
		public PreviewImage PreviewImage { get; set; }

		[DataMember]
		public WatermarkImage WatermarkImage { get; set; }

		[DataMember]
		public byte[] Report { get; set; }
	}

	/// <summary>
	/// Класс, описывающий структуру файла-подложки.
	/// </summary>
	[DataContract]
	public class WatermarkImage
	{
		/// <summary>
		/// Идентификатор оригинального изображения для файла подложки
		/// </summary>
		[DataMember]
		public Guid? OriginalImageGuid { get; set; }

		/// <summary>
		/// Массив байт, представляющий изображение подложки
		/// </summary>
		[DataMember]
		public byte[] ImageContent { get; set; }

		/// <summary>
		/// Идентификатор файла-подложки в папке Content
		/// </summary>
		[DataMember]
		public Guid Guid { get; set; }
	}

	[DataContract]
	public class PreviewImage
	{
		[DataMember]
		public Color BackgroundColor { get; set; }

		[DataMember]
		public Color? BorderColor { get; set; }

		[DataMember]
		public double BorderThickness { get; set; }

		public PreviewImage()
		{
			BackgroundColor = Colors.White;
		}
	}
}
