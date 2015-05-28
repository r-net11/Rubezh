﻿using System;
using FiresecAPI.SKD;
using NUnit.Framework;
using NSubstitute;
using SKDModule.ViewModels;

namespace UnitTestsProject.FireMonitorTests
{
	[TestFixture]
	public class BaseTests
	{
		private DayTimeTrack DayTimeTrack;
		private ShortEmployee ShortEmployee;
		private TimeTrackDetailsViewModel TimeTrackDetailsViewModel;

		[SetUp]
		public void SetUp()
		{
			DayTimeTrack = new DayTimeTrack();
			ShortEmployee = new ShortEmployee();
		}

		[Test]
		public void ValidationTimeTrack_IsValid_ReturnTrue()
		{
			//Arrange
			TimeTrackPartDetailsViewModel enterTimeMoreThenExitTime = new TimeTrackPartDetailsViewModel(new TimeTrackDetailsViewModel(new DayTimeTrack(), new ShortEmployee()), new TimeSpan(2, 2, 2, 2), new TimeSpan(3, 3, 3, 3));
			TimeTrackPartDetailsViewModel enterTimeLessThenExitTime = new TimeTrackPartDetailsViewModel(new TimeTrackDetailsViewModel(new DayTimeTrack(), new ShortEmployee()), new TimeSpan(2, 2, 2, 2), new TimeSpan(1, 1, 1, 1));
			TimeTrackPartDetailsViewModel enterTimeEqualsExitTime = new TimeTrackPartDetailsViewModel(new TimeTrackDetailsViewModel(new DayTimeTrack(), new ShortEmployee()), new TimeSpan(2, 2, 2, 2), new TimeSpan(2, 2, 2, 2));
			
			//Act
			bool resultForEnterTimeMoreThenExitTime = enterTimeMoreThenExitTime.Validate();
			bool resultForEnterTimeLessThenExitTime = enterTimeLessThenExitTime.Validate();
			bool resultForEnterTimeEqualsExitTime = enterTimeEqualsExitTime.Validate();

			bool condition = resultForEnterTimeMoreThenExitTime 
								&& !resultForEnterTimeLessThenExitTime 
								&& !resultForEnterTimeEqualsExitTime;

			//Assert
			Assert.IsTrue(condition, "It is OK");
		}
	}
}
