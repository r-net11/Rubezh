﻿using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class Photo
	{
		public Photo() { }

		public static Photo Create(FiresecAPI.SKD.Photo photo) 
		{ 
			if (photo == null) 
				return null; 
			return new Photo { UID = photo.UID, Data = photo.Data }; 
		}
		
		public FiresecAPI.SKD.Photo Translate() { return new FiresecAPI.SKD.Photo { UID = UID, Data = Data }; }
		
		[Key]
		public Guid UID { get; set; }

		public byte[] Data { get; set; } 
	}
}
