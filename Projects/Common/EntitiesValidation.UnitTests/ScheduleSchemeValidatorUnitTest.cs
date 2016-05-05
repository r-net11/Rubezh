using System;
using System.Collections.Generic;
using StrazhAPI.SKD;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntitiesValidation.UnitTests
{
	[TestClass]
	public class ScheduleSchemeValidatorUnitTest
	{
		/// <summary>
		/// Проверяет пересечение 2х дневных графиков работы
		/// Дневной график 1: 23:00:00-22:00:00
		/// Дневной график 2: 20:00:00-02:00:00
		/// </summary>
		[TestMethod]
		public void SlideScheduleSchemeWithDayIntervalIntersectionTest()
		{
			// Создаем дневные графики
			var dayIntervals = new List<DayInterval>
			{
				// Дневной график 1
				new DayInterval
				{
					DayIntervalParts = new List<DayIntervalPart>
					{
						// 23:00:00-22:00:00
						new DayIntervalPart
						{
							BeginTime = new TimeSpan(23, 0, 0),
							EndTime = new TimeSpan(22, 0, 0),
							TransitionType = DayIntervalPartTransitionType.Night
						}
					}
				},

				// Дневной график 2
				new DayInterval
				{
					DayIntervalParts = new List<DayIntervalPart>
					{
						// 20:00:00-02:00:00
						new DayIntervalPart
						{
							BeginTime = new TimeSpan(20, 0, 0),
							EndTime = new TimeSpan(02, 0, 0),
							TransitionType = DayIntervalPartTransitionType.Night
						}
					}
				}
			};

			// Валидируем дневные графики на пересечение
			var validationResult = ScheduleSchemeValidator.ValidateDayIntervalsIntersecion(dayIntervals);
			
			// Есть 1 пересечение
			Assert.IsTrue(validationResult.HasError);
			Assert.IsTrue(validationResult.Result.Count == 1);
		}

		/// <summary>
		/// Проверяет отсутствие пересечения 2х дневных графиков работы
		/// Дневной график 1: 23:00:00-19:00:00
		/// Дневной график 2: 20:00:00-02:00:00
		/// </summary>
		[TestMethod]
		public void SlideScheduleSchemeWithoutDayIntervalIntersectionTest1()
		{
			// Создаем дневные графики
			var dayIntervals = new List<DayInterval>
			{
				// Дневной график 1
				new DayInterval
				{
					Name = "Ночь",
					DayIntervalParts = new List<DayIntervalPart>
					{
						// 23:00:00-19:00:00
						new DayIntervalPart
						{
							BeginTime = new TimeSpan(23, 0, 0),
							EndTime = new TimeSpan(19, 0, 0),
							TransitionType = DayIntervalPartTransitionType.Night
						}
					}
				},

				// Дневной график 2
				new DayInterval
				{
					Name = "День",
					DayIntervalParts = new List<DayIntervalPart>
					{
						// 20:00:00-02:00:00
						new DayIntervalPart
						{
							BeginTime = new TimeSpan(20, 0, 0),
							EndTime = new TimeSpan(02, 0, 0),
							TransitionType = DayIntervalPartTransitionType.Night
						}
					}
				}
			};

			// Валидируем дневные графики на пересечение
			var validationResult = ScheduleSchemeValidator.ValidateDayIntervalsIntersecion(dayIntervals);

			// Нет пересечения
			Assert.IsFalse(validationResult.HasError);
			Assert.IsTrue(validationResult.Result.Count == 0);
		}

		/// <summary>
		/// Проверяет отсутствие пересечения 3х дневных графиков работы
		/// Дневной график 1: 23:00:00-22:00:00
		/// Дневной график 2: Никогда
		/// Дневной график 3: 20:00:00-02:00:00
		/// </summary>
		[TestMethod]
		public void SlideScheduleSchemeWithoutDayIntervalIntersectionTest2()
		{
			// Создаем дневные графики
			var dayIntervals = new List<DayInterval>
			{
				// Дневной график 1
				new DayInterval
				{
					DayIntervalParts = new List<DayIntervalPart>
					{
						// 23:00:00-22:00:00
						new DayIntervalPart
						{
							BeginTime = new TimeSpan(23, 0, 0),
							EndTime = new TimeSpan(22, 0, 0),
							TransitionType = DayIntervalPartTransitionType.Night
						}
					}
				},

				// Дневной график 2
				null,
	
				// Дневной график 3
				new DayInterval
				{
					DayIntervalParts = new List<DayIntervalPart>
					{
						// 20:00:00-02:00:00
						new DayIntervalPart
						{
							BeginTime = new TimeSpan(20, 0, 0),
							EndTime = new TimeSpan(02, 0, 0),
							TransitionType = DayIntervalPartTransitionType.Night
						}
					}
				}
			};

			// Валидируем дневные графики на пересечение
			var validationResult = ScheduleSchemeValidator.ValidateDayIntervalsIntersecion(dayIntervals);

			// Нет пересечения
			Assert.IsFalse(validationResult.HasError);
			Assert.IsTrue(validationResult.Result.Count == 0);
		}
	}
}
