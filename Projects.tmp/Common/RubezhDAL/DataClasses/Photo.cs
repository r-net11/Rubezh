using System;
using System.ComponentModel.DataAnnotations;

namespace RubezhDAL.DataClasses
{
	public class Photo
	{
		public Photo() { }

		public static Photo Create(RubezhAPI.SKD.Photo photo) 
		{ 
			if (photo == null) 
				return null; 
			return new Photo { UID = Guid.NewGuid(), Data = photo.Data }; 
		}
		
		public RubezhAPI.SKD.Photo Translate() { return new RubezhAPI.SKD.Photo { UID = UID, Data = Data }; }
		
		[Key]
		public Guid UID { get; set; }

		public byte[] Data { get; set; } 
	}
}
