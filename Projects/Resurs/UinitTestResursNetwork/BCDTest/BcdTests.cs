using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResursNetwork.BCD;
using Moq;

namespace UinitTestResursNetwork.BCDTest
{
    [TestClass]
    public class BcdTests
    {
        [TestMethod]
        public void IsValidTrueTest()
        {
            //arrange
            var x = Byte.Parse("87");

            //act
            var result = BcdConverter.IsValid(x);
            
            //assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsValidFalseTest()
        {
            //arrange
            var x = Byte.Parse("197");

            //act
            var result = BcdConverter.IsValid(x);

            //assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ToByteTest()
        {
            //arrange
            var digit = @"99";
            var x = Byte.Parse(digit);

            //act
            var result = BcdConverter.ToByte(x);

            //assert
            Assert.AreEqual(63, result);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "")]
        public void ToByteExceptionTest()
        {
            //arrange
            var x = Byte.Parse("197"); // Ошибка!!!

            //act
            var result = BcdConverter.ToByte(x);

            //assert
            // Ожидаем исключение
        }

        [TestMethod]
        public void ToBcdByteTest()
        {
            //arrange
            var digit = @"16";
            var x = Byte.Parse(digit);

            //act
            var result = BcdConverter.ToBcdByte(x);

            //assert
            Assert.AreEqual((Byte)0x16, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException), "")]
        public void ToBcdByteExceptionTest()
        {
            //arrange
            var digit = @"100";
            var x = Byte.Parse(digit);

            //act
            var result = BcdConverter.ToBcdByte(x);

            //assert
            // Ожидаем исключение
        }

		[TestMethod]
		public void ToBcdUIn16Test()
		{
			//arrange
			var digit = @"1234";
			var x = ushort.Parse(digit);

			//act
			var result = BcdConverter.ToBcdUInt16(x);

			//assert
			Assert.AreEqual((ushort)0x1234, result);
		}

		[TestMethod]
		public void ToBcdUIn32Test()
		{
			//arrange
			var digit = @"12345678";
			var x = uint.Parse(digit);

			//act
			var result = BcdConverter.ToBcdUInt32(x);

			//assert
			Assert.AreEqual((uint)0x12345678, result);
		}
    }
}
