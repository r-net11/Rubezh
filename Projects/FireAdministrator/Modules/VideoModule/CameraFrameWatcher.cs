using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VideoModule
{
	public class CameraFrameWatcher : IComparable
	{
		public Bitmap Bitmap { get; private set; }
		public DateTime DateTime { get; private set; }
		public CameraFrameWatcher(Bitmap bitmap, DateTime dateTime)
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
			var cameraFrameWather1 = (CameraFrameWatcher) a;
			var cameraFrameWather2 = (CameraFrameWatcher) b;
			if (cameraFrameWather1.DateTime > cameraFrameWather2.DateTime)
				return 1;
			if (cameraFrameWather1.DateTime < cameraFrameWather2.DateTime)
				return -1;
			return 0;
		}
	}
}
