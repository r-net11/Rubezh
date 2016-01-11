using System;
using FiresecAPI.SKD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntitiesValidation.UnitTests
{
	[TestClass]
	public class DayIntervalPartValidatorUnitTest
	{
		/// <summary>
		/// Тестирует алгоритм валидации временного интервала дневного графика с нулевой длиной
		/// </summary>
		[TestMethod]
		public void DayIntervalPartWithZeroLengthTest()
		{
			var dayIntervalPart = new DayIntervalPart
			{
				BeginTime = TimeSpan.FromHours(1),
				EndTime = TimeSpan.FromHours(1),
				TransitionType = DayIntervalPartTransitionType.Day
			};

			var validationResult = DayIntervalPartValidator.ValidateNewDayIntervalPartLength(dayIntervalPart);
			
			// Должна быть ошибка при валидации интервала с нулевой длиной
			Assert.IsFalse(validationResult.Result);
		}

		/// <summary>
		/// Тестирует алгоритм валидации временного интевала дневного графика с ненулевой длиной
		/// </summary>
		[TestMethod]
		public void DayIntervalPartWithNoneZeroLengthTest1()
		{
			var dayIntervalPart = new DayIntervalPart
			{
				BeginTime = TimeSpan.FromHours(1),
				EndTime = TimeSpan.FromHours(2),
				TransitionType = DayIntervalPartTransitionType.Day
			};

			var validationResult = DayIntervalPartValidator.ValidateNewDayIntervalPartLength(dayIntervalPart);
			
			// При валидации интервала с ненулевой длинной не должно быть ошибки
			Assert.IsTrue(validationResult.Result);
		}

		/// <summary>
		/// Тестирует алгоритм валидации временного интевала дневного графика с ненулевой длиной и переходом через сутки
		/// </summary>
		[TestMethod]
		public void DayIntervalPartWithNoneZeroLengthTest2()
		{
			var dayIntervalPart = new DayIntervalPart
			{
				BeginTime = TimeSpan.FromHours(1),
				EndTime = TimeSpan.FromHours(1),
				TransitionType = DayIntervalPartTransitionType.Night
			};

			var validationResult = DayIntervalPartValidator.ValidateNewDayIntervalPartLength(dayIntervalPart);
			
			// При валидации интервала с переходом через сутки для одинаковых времени начала и конца ошибки не будет
			Assert.IsTrue(validationResult.Result);
		}
	}
}
