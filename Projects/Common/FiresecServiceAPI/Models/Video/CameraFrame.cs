using System;
using System.Drawing;

namespace FiresecAPI.Models
{
	public class CameraFrame
	{
		public Bitmap Bitmap { get; private set; }
		public DateTime DateTime { get; private set; }

		public CameraFrame(Bitmap bitmap, DateTime dateTime)
		{
			Bitmap = bitmap;
			DateTime = dateTime;
		}
	}
}