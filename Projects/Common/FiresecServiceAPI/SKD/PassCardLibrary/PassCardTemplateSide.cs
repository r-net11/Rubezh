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
		public WatermarkImage WatermarkImage { get; set; } //TODO: try to replace by Attachment type

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

	//[DataContract]
	//public struct PreviewImage
	//{
	//	private readonly StrazhAPI.Color _backgroundColor;
	//	private readonly StrazhAPI.Color _borderColor;
	//	private readonly int _borderThikness;

	//	public Color BackgroundColor { get { return _backgroundColor; } }
	//	public Color BorderColor { get { return _borderColor; } }
	//	public int BorderThikness { get { return _borderThikness; } }

	//	public PreviewImage(Color backgroundColor, Color borderColor, int borderThikness)
	//	{
	//		_backgroundColor = backgroundColor;
	//		_borderColor = borderColor;
	//		_borderThikness = borderThikness;
	//	}

	//	public PreviewImage SetBackgroundColor(Color backgroundColor)
	//	{
	//		return new PreviewImage(backgroundColor, _borderColor, _borderThikness);
	//	}

	//	public PreviewImage SetBorderColor(Color borderColor)
	//	{
	//		return new PreviewImage(_backgroundColor, borderColor, _borderThikness);
	//	}

	//	public PreviewImage SetBorderThikness(int borderThikness)
	//	{
	//		return new PreviewImage(_backgroundColor, _borderColor, borderThikness);
	//	}

	//	public static bool operator ==(PreviewImage x, PreviewImage y)
	//	{
	//		return x.BackgroundColor == y.BackgroundColor
	//			   && x.BorderColor == y.BorderColor
	//			   && x.BorderThikness == y.BorderThikness;
	//	}
	//	public static bool operator !=(PreviewImage x, PreviewImage y)
	//	{
	//		return !(x == y);
	//	}

	//	public override bool Equals(object obj)
	//	{
	//		if (! (obj is PreviewImage))
	//			return false;

	//		var image = (PreviewImage) obj;

	//		return (BackgroundColor == image.BackgroundColor)
	//			&& (BorderColor == image.BorderColor)
	//			&& (BorderThikness == image.BorderThikness);
	//	}

	//	public override int GetHashCode()
	//	{
	//		return BorderColor.A
	//			^ BorderColor.B
	//			^ BorderColor.G
	//			^ BorderColor.R
	//			^ BackgroundColor.A
	//			^ BackgroundColor.B
	//			^ BackgroundColor.G
	//			^ BackgroundColor.R
	//			^ BorderThikness;
	//	}
	//}
}
