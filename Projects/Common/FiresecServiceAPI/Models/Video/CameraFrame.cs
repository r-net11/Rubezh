using System;
using System.Drawing;

namespace FiresecAPI.Models
{
	public class CameraFrame : IComparable
	{
		public Bitmap Bitmap { get; private set; }
		public DateTime DateTime { get; private set; }

		public CameraFrame(Bitmap bitmap, DateTime dateTime)
		{
			Bitmap = bitmap;
			DateTime = dateTime;
		}

		int IComparable.CompareTo(object a)
		{
			return Compare(this, a);
		}

		public int Compare(object a, object b)
		{
			var cameraFrame1 = (CameraFrame)a;
			var cameraFrame2 = (CameraFrame)b;
			if (cameraFrame1.DateTime > cameraFrame2.DateTime)
				return 1;
			if (cameraFrame1.DateTime < cameraFrame2.DateTime)
				return -1;
			return 0;
		}
	}
}