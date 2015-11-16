using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResursNetwork.Incotex.Models;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;

namespace UinitTestResursNetwork.MpowerTests
{
	[TestClass]
	public class MpowerTest
	{
		[TestMethod]
		public void ToArrayByFloatValueTest()
		{
			// arrange
			var converter = Mpower.GetValueConveter();
			float y1 = 75.43f;
			float y2 = 63.385f;
			float y3 = 99.721f;
			float y4 = 01.389f;

			// act
			var result1 = converter.ToArray(y1);
			var result2 = converter.ToArray(y2);
			var result3 = converter.ToArray(y3);
			var result4 = converter.ToArray(y4);

			// assert
			Assert.AreEqual(2, result1.Length);
			Assert.AreEqual(0x75, result1[0]);
			Assert.AreEqual(0x43, result1[1]);

			Assert.AreEqual(0x63, result2[0]);
			Assert.AreEqual(0x38, result2[1]);

			Assert.AreEqual(0x99, result3[0]);
			Assert.AreEqual(0x72, result3[1]);

			Assert.AreEqual(0x01, result4[0]);
			Assert.AreEqual(0x39, result4[1]);
		}

		public void ToArrayByMpowerTest()
		{
			// Arrange
			
			// Act

			// Assert
		}
	}
}
