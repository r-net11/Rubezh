using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using NUnit.Framework;

namespace FiresecMonitorUnitTests.FireMonitorTests
{
	[TestFixture]
	public class AppDataFolderHelperTests
	{
		[Test]
		public void CheckForDoesNotThrowAnyExceptionsWhenTakeFile()
		{
			string folderName = null;
			string fileName = null;

			Assert.DoesNotThrow(() => AppDataFolderHelper.GetFileInFolder(folderName, fileName));
		}
	}
}
