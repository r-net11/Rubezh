using System;
using System.ComponentModel.DataAnnotations;

namespace RubezhDAL.DataClasses
{
	public class Photo
	{
		public Photo() { }

		public static Photo Create(FiresecAPI.SKD.Photo photo) 
		{ 
			if (photo == null) 
				return null; 
			return new Photo { UID = Guid.NewGuid(), Data = photo.Data }; 
		}
		
		public FiresecAPI.SKD.Photo Translate() { return new FiresecAPI.SKD.Photo { UID = UID, Data = Data }; }
		
		[Key]
		public Guid UID { get; set; }

		public byte[] Data { get; set; } 
	}
}
