using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
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

		[TestMethod]
		public void DayIntervalsWithoutIntersectionTest()
		{
			// Дневной график с интервалом без перехода
			var dayInterval1 = new DayInterval
			{
				DayIntervalParts = new List<DayIntervalPart>
				{
					// 23:00:00-23:59:59
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(23, 0, 0),
						EndTime = new TimeSpan(23, 59, 59),
						TransitionType = DayIntervalPartTransitionType.Day
					}
				}
			};
			// Дневной график с интервалом без перехода
			var dayInterval2 = new DayInterval
			{
				DayIntervalParts = new List<DayIntervalPart>
				{
					// 01:00:00-02:00:00
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(1, 0, 0),
						EndTime = new TimeSpan(2, 0, 0),
						TransitionType = DayIntervalPartTransitionType.Day
					}
				}
			};

			var result = DayIntervalValidator.ValidateIntersection(dayInterval1, dayInterval2);
			
			// Пересечения дневных графиков быть не должно
			Assert.IsTrue(result.Result.Count == 0);
		}

		[TestMethod]
		public void DayIntervalsWith1IntersectionTest()
		{
			// Дневной график с интервалом с переходом
			var dayInterval1 = new DayInterval
			{
				DayIntervalParts = new List<DayIntervalPart>
				{
					// 23:00:00-02:00:00
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(23, 0, 0),
						EndTime = new TimeSpan(2, 0, 0),
						TransitionType = DayIntervalPartTransitionType.Night
					}
				}
			};
			// Дневной график с интервалом без перехода
			var dayInterval2 = new DayInterval
			{
				DayIntervalParts = new List<DayIntervalPart>
				{
					// 01:00:00-02:00:00
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(1, 0, 0),
						EndTime = new TimeSpan(2, 0, 0),
						TransitionType = DayIntervalPartTransitionType.Day
					}
				}
			};

			var result = DayIntervalValidator.ValidateIntersection(dayInterval1, dayInterval2);

			// Дневные графики пересекаются
			Assert.IsTrue(result.Result.Count == 1);
		}

		[TestMethod]
		public void DayIntervalsWith2IntersectionsTest()
		{
			// Дневной график с интервалом с переходом
			var dayInterval1 = new DayInterval
			{
				DayIntervalParts = new List<DayIntervalPart>
				{
					// 23:00:00-05:30:00
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(23, 0, 0),
						EndTime = new TimeSpan(5, 30, 0),
						TransitionType = DayIntervalPartTransitionType.Night
					}
				}
			};
			// Дневной график с интервалом без перехода
			var dayInterval2 = new DayInterval
			{
				DayIntervalParts = new List<DayIntervalPart>
				{
					// 01:00:00-02:00:00
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(1, 0, 0),
						EndTime = new TimeSpan(2, 0, 0),
						TransitionType = DayIntervalPartTransitionType.Day
					},
					// 02:00:00-05:00:00
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(2, 0, 0),
						EndTime = new TimeSpan(5, 0, 0),
						TransitionType = DayIntervalPartTransitionType.Day
					},
					// 06:00:00-14:00:00
					new DayIntervalPart
					{
						BeginTime = new TimeSpan(6, 0, 0),
						EndTime = new TimeSpan(14, 0, 0),
						TransitionType = DayIntervalPartTransitionType.Day
					}
				}
			};

			var result = DayIntervalValidator.ValidateIntersection(dayInterval1, dayInterval2);

			// Дневные графики пересекаются
			Assert.IsTrue(result.Result.Count == 2);
		}
	}
}
